using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate;
using CodeGen.Intermediate.Codes;
using CodeGen.Intermediate.Codes.Common;
using CodeGen.Parsing.Ast;
using Plotty.Model;
using ArithmeticOperator = Plotty.Model.ArithmeticOperator;
using Label = Plotty.Model.Label;

namespace Plotty.CodeGeneration
{
    public class PlottyCodeGenerationVisitor : IIntermediateCodeVisitor
    {
        public const int ReturnRegisterIndex = 5;

        private readonly Register returnRegister = new Register(ReturnRegisterIndex);
        private readonly Register baseRegister = new Register(6);
        private readonly Register stackRegister = new Register(7);

        private readonly List<PendingFixup> fixups = new List<PendingFixup>();
        private readonly List<ILine> lines = new List<ILine>();
        private Dictionary<Reference, int> localAddresses;
        private Scope currentScope;

        public PlottyCodeGenerationVisitor(Scope scope, Func<ICollection<ILine>, ICollection<PendingFixup>, Register, Register, Emitter> emitterFactory)
        {
            CurrentScope = scope;
            RootScope = CurrentScope;
            Emit = emitterFactory(lines, fixups, stackRegister, baseRegister);
        }

        private Emitter Emit { get; }

        public IEnumerable<ILine> Lines => lines.AsReadOnly();

        private Scope CurrentScope
        {
            get => currentScope;
            set
            {
                currentScope = value;
                Allocate(currentScope.Symbols);
            }
        }

        public Scope RootScope { get; }

        public ReadOnlyCollection<PendingFixup> Fixups => fixups.AsReadOnly();

        public void Visit(JumpIfFalse code)
        {
            LoadReference(code.Reference, 1);

            var onFalseLabel = new Label();

            // Comparison with 0 (TRUE). If both values are 0, the condition is true, so the code has to be executed.
            // So we emit
            Emit.Move(0, new Register(0));
            Emit.Branch(BooleanOperator.Equal, new Register(1), new Register(0), onFalseLabel);
            Emit.Move(0, new Register(0));
            Emit.Jump(new Label(code.Label.Name));
            Emit.Label(onFalseLabel);
        }

        public void Visit(BoolConstantAssignment code)
        {
            Emit.Move(code.Value ? 0 : 1, new Register(1));
            StoreReference(code.Target, 1);
        }

        public void Visit(LabelCode code)
        {
            Emit.Label(new Label(code.Label.Name));
        }

        public void Visit(IntegerConstantAssignment code)
        {
            Emit.Move(code.Value, new Register(1));

            StoreReference(code.Target, 1);
        }

        public void Visit(ArithmeticAssignment code)
        {
            LoadReference(code.Left, 1);
            LoadReference(code.Right, 2);

            Emit.Arithmetic(GetOperator(code.Operation), new RegisterSource(2), 1);
            
            StoreReference(code.Target, 1);
        }

        private void LoadReference(Reference reference, Register register, Register addressRegister = null)
        {
            addressRegister = addressRegister ?? new Register(0);
            Emit.Move(GetAddress(reference), addressRegister);
            Emit.Load(register, baseRegister, addressRegister);
        }

        private void StoreReference(Reference reference, Register register, Register addressRegister = null)
        {
            addressRegister = addressRegister ?? new Register(0);
            Emit.Move(GetAddress(reference), addressRegister);
            Emit.Store(register, baseRegister, addressRegister);
        }

        public void Visit(ReferenceAssignment code)
        {
            LoadReference(code.Origin, 1);
            StoreReference(code.Target, 1);
        }

        public void Visit(BoolExpressionAssignment code)
        {
            LoadReference(code.Left, 1);
            LoadReference(code.Right, 2);

            if (code.Operation == BooleanOperation.IsEqual)
            {               
                Emit.Arithmetic(ArithmeticOperator.Substract, new RegisterSource(2), 1);

                StoreReference(code.Target, 1);
            }
            else
            {
                var jumpOnTrue = new Label();
                var endLabel = new Label();

                Emit.Branch(code.Operation.ToOperator(), 1, 2, jumpOnTrue);

                // Sets false
                Emit.Move(1, new Register(1));
                Emit.Jump(endLabel);
                Emit.Label(jumpOnTrue);

                // Sets true
                Emit.Move(0, new Register(1));

                // End
                Emit.Label(endLabel);

                StoreReference(code.Target, 1);
            }
        }

        public void Visit(Jump code)
        {
            Emit.Jump(new Label(code.Label.Name));
        }

        public void Visit(FunctionDefinitionCode code)
        {
            CurrentScope = GetFunctionScope(code.Function.Name);

            Emit.Label(new Label(code.Function.Name));

            PopParamsToFunctionSpace(code.Function.Arguments);
        }

        public void Visit(CallCode code)
        {
            var continuationLabel = new Label();

            CreateFrame(code.FunctionName, continuationLabel);

            Emit.Jump(new Label(code.FunctionName));
            Emit.Label(continuationLabel);

            RestorePreviousFrame(code.FunctionName);
            
            if (code.Reference != null)
            {
                StoreReference(code.Reference, returnRegister);
            }
        }

        private void PopParamsToFunctionSpace(ICollection<Argument> arguments)
        {
            if (!arguments.Any())
            {
                return;
            }

            var @base = new Register(0);
            var offset = new Register(1);
            var parameter = new Register(2);
            var localAddress = new Register(3);

            Emit.Move(0, offset);
            Emit.Transfer(baseRegister, @base);
            Emit.SubstractInt(arguments.Count, @base);

            foreach (var argument in arguments)
            {
                Emit.Load(parameter, @base, offset);
                StoreReference(argument.Reference, parameter, localAddress);
                Emit.Increment(offset);
            }
        }

        public void Visit(ReturnCode code)
        {
            if (code.Reference != null)
            {
                LoadReference(code.Reference, returnRegister);                
            }

            Emit.Pop(0);
            Emit.Jump(0);
        }

        public void Visit(HaltCode code)
        {
            Emit.Halt();
        }

        public void Visit(ParameterCode code)
        {
            var destination = new Register(1);
            
            LoadReference(code.Reference, destination);
            Emit.Push(destination);
        }

        private void CreateFrame(string functionName, Label label)
        {
            Emit.Transfer(baseRegister, 0);
            Emit.Transfer(stackRegister, 1);

            // New base register
            Emit.AddRegister(stackRegister, baseRegister);

            // New stack register
            Emit.Move(GetFunctionScope(functionName).Symbols.Count, stackRegister);

            Emit.Push(0);   // Base Register
            Emit.Push(1);   // Stack Register
            Emit.Push(label);
        }

        private void RestorePreviousFrame(string codeFunctionName)
        {
            Emit.Pop(0);    // Stack Register
            Emit.Pop(1);    // Base Register

            Emit.Transfer(0, stackRegister);
            Emit.Transfer(1, baseRegister);

            var argumentsCount = GetFunction(codeFunctionName).Arguments.Count;
            if (argumentsCount > 0)
            {
                Emit.SubstractInt(argumentsCount, stackRegister);
            }
        }

        private Function GetFunction(string functionName)
        {
            return RootScope.Children.Select(x => x.Owner).OfType<Function>().Single(x => x.Name == functionName);
        }

        private Scope GetFunctionScope(string functionName)
        {
            return RootScope.Children.Single(s => (s.Owner as Function)?.Name == functionName);
        }

        private void Allocate(IReadOnlyDictionary<Reference, Symbol> currentScopeSymbols)
        {
            localAddresses = currentScopeSymbols.Select((x, i) => new { x, i }).ToDictionary(x => x.x.Key, x => x.i);
        }

        private int GetAddress(Reference reference)
        {
            return localAddresses[reference];
        }

        private ArithmeticOperator GetOperator(CodeGen.Intermediate.Codes.Common.ArithmeticOperator codeOperation)
        {
            if (codeOperation == CodeGen.Intermediate.Codes.Common.ArithmeticOperator.Add)
            {
                return ArithmeticOperator.Add;
            }

            if (codeOperation == CodeGen.Intermediate.Codes.Common.ArithmeticOperator.Substract)
            {
                return ArithmeticOperator.Substract;
            }

            if (codeOperation == CodeGen.Intermediate.Codes.Common.ArithmeticOperator.Mult)
            {
                return ArithmeticOperator.Multiply;
            }

            throw new ArgumentOutOfRangeException(nameof(codeOperation));
        }
    }
}