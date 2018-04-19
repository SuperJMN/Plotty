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
    public partial class PlottyCodeGenerationVisitor : IIntermediateCodeVisitor
    {
        public const int ReturnRegisterIndex = 5;

        private readonly Register baseRegister = new Register(7);
        private readonly Register returnRegister = new Register(5);
        private readonly Register stackRegister = new Register(6);

        private readonly List<PendingFixup> fixups = new List<PendingFixup>();
        private readonly List<ILine> lines = new List<ILine>();
        private Scope currentScope;
        private Dictionary<Reference, int> localAddress;
        
        public PlottyCodeGenerationVisitor(Scope scope)
        {
            CurrentScope = scope;
            RootScope = CurrentScope;
            Emit = new ContextualizedEmitter(this);
        }

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

        private ContextualizedEmitter Emit { get; }

        public CodeLog CodeLog { get; } = new CodeLog();

        public ReadOnlyCollection<PendingFixup> Fixups => fixups.AsReadOnly();

        public void Visit(JumpIfFalse code)
        {
            Emit.CurrentCode = code;

            Emit.Move(GetAddress(code.Reference), new Register(0));
            Emit.Load(new Register(1), baseRegister, 0);

            var onFalseLabel = new Label();

            // Comparison with 0 (TRUE). If both values are 0, the conditions is true, so the code has to be executed.
            // So we emit
            Emit.Move(0, new Register(0));
            Emit.Branch(BooleanOperator.Equal, new Register(1), new Register(0), onFalseLabel);
            Emit.Move(0, new Register(0));
            Emit.Jump(new Label(code.Label.Name));
            Emit.Label(onFalseLabel);
        }

        public void Visit(BoolConstantAssignment code)
        {
            Emit.CurrentCode = code;

            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Move(code.Value ? 0 : 1, new Register(1));
            Emit.Store(new Register(1), baseRegister, 0);
        }

        public void Visit(LabelCode code)
        {
            Emit.LogVisitFor(code);

            Emit.Label(new Label(code.Label.Name));
        }

        public void Visit(IntegerConstantAssignment code)
        {
            Emit.CurrentCode = code;

            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Move(code.Value, new Register(1));
            Emit.Store(new Register(1), baseRegister, 0);
        }

        public void Visit(ArithmeticAssignment code)
        {
            Emit.CurrentCode = code;

            Emit.Move(GetAddress(code.Left), 0);
            Emit.Load(new Register(1), baseRegister, 0);

            Emit.Move(GetAddress(code.Right), 0);
            Emit.Load(new Register(2), baseRegister, 0);

            Emit.Move(GetAddress(code.Target), 0);

            var op = GetOperator(code.Operation);

            Emit.Arithmetic(op, new RegisterSource(2), 1);

            Emit.Store(1, baseRegister, 0);
        }

        public void Visit(ReferenceAssignment code)
        {
            Emit.CurrentCode = code;

            Emit.Move(GetAddress(code.Origin), new Register(0));
            Emit.Load(new Register(1), baseRegister, 0);
            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Store(new Register(1), baseRegister, 0);
        }

        public void Visit(BoolExpressionAssignment code)
        {
            Emit.CurrentCode = code;

            if (code.Operation == BooleanOperation.IsEqual)
            {
                Emit.Move(GetAddress(code.Left), new Register(0));
                Emit.Load(new Register(1), baseRegister, new Register(0));

                Emit.Move(GetAddress(code.Right), new Register(0));
                Emit.Load(new Register(2), baseRegister, new Register(0));

                Emit.Arithmetic(ArithmeticOperator.Substract, new RegisterSource(2), 1);

                Emit.Move(GetAddress(code.Target), new Register(0));
                Emit.Store(new Register(1), baseRegister, new Register(0));
            }
            else
            {
                Emit.Move(GetAddress(code.Left), new Register(0));
                Emit.Load(new Register(1), baseRegister, new Register(0));

                Emit.Move(GetAddress(code.Right), new Register(0));
                Emit.Load(new Register(2), baseRegister, new Register(0));

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

                Emit.Move(GetAddress(code.Target), new Register(0));
                Emit.Store(new Register(1), baseRegister, new Register(0));
            }
        }

        public void Visit(Jump code)
        {
            Emit.CurrentCode = code;

            Emit.Jump(new Label(code.Label.Name));
        }

        public void Visit(FunctionDefinitionCode code)
        {
            Emit.CurrentCode = code;

            CurrentScope = GetFunctionScope(code.Function.Name);

            Emit.Label(new Label(code.Function.Name));
        }

        public void Visit(CallCode code)
        {
            Emit.CurrentCode = code;

            var continuationLabel = new Label();

            GoToNewFrame(code.FunctionName, continuationLabel);

            Emit.CurrentDescription = null;
            
            Emit.Jump(new Label(code.FunctionName));
            Emit.Label(continuationLabel);

            RestoreAndGoToPreviousFrame();

            Emit.CurrentDescription = null;

            if (code.Reference != null)
            {
                Emit.Move(GetAddress(code.Reference), 0);
                Emit.Store(returnRegister, baseRegister, 0);
            }
        }

        private void PopParams(string functionName)
        {
            var func = GetFunction(functionName);
            foreach (var argument in func.Arguments.Reverse())
            {
                Emit.Pop(0);
                Emit.Store(0, GetAddress(argument.Reference));
            }            
        }

        public void Visit(ReturnCode code)
        {
            Emit.CurrentCode = code;

            if (code.Reference != null)
            {
                Emit.Move(GetAddress(code.Reference), 0);
                Emit.Load(returnRegister, baseRegister, 0);
            }

            Emit.Pop(0);
            Emit.Jump(0);

            CurrentScope = CurrentScope.Parent;
        }

        public void Visit(HaltCode code)
        {
            Emit.CurrentCode = code;

            Emit.Halt();
        }

        public void Visit(ParameterCode code)
        {
            Emit.CurrentCode = code;
            Emit.CurrentDescription = $"Pushing parameter {code.Reference}";
            
            Emit.Move(GetAddress(code.Reference), 0);
            Emit.Load(0, baseRegister);
            Emit.Push(0);
        }

        private void GoToNewFrame(string functionName, Label label)
        {
            //PopParams(functionName);

            Emit.CurrentDescription = "Go to new frame";

            var symbolCount = CurrentScope.Symbols.Count;

            Emit.Transfer(baseRegister, 0);
            Emit.Transfer(stackRegister, 1);

            // New base register
            Emit.AddInt(symbolCount, baseRegister);
            Emit.Add(stackRegister, baseRegister);

            // New stack register
            Emit.Move(GetFunctionScope(functionName).Symbols.Count, stackRegister);
            
            Emit.Push(0);   // Base Register
            Emit.Push(1);   // Stack Register
            Emit.Push(label);
        }

        private void RestoreAndGoToPreviousFrame()
        {
            Emit.CurrentDescription = "Restoring previous frame";

            Emit.Pop(0);    // Stack Register
            Emit.Pop(1);    // Base Register

            Emit.Transfer(0, stackRegister);
            Emit.Transfer(1, baseRegister);
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
            localAddress = currentScopeSymbols.Select((x, i) => new {x, i}).ToDictionary(x => x.x.Key, x => x.i);
        }

        private int GetAddress(Reference reference)
        {
            return localAddress[reference];
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