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

        private readonly Register returnRegister = new Register(ReturnRegisterIndex);
        private readonly Register baseRegister = new Register(6);
        private readonly Register stackRegister = new Register(7);

        private readonly List<PendingFixup> fixups = new List<PendingFixup>();
        private readonly List<ILine> lines = new List<ILine>();
        private Dictionary<Reference, int> localAddresses;

        public PlottyCodeGenerationVisitor(Scope scope)
        {
            PushScope(scope);
            RootScope = CurrentScope;
            Emit = new ContextualizedEmitter(this);
        }

        public IEnumerable<ILine> Lines => lines.AsReadOnly();

        private readonly Stack<Scope> scopes = new Stack<Scope>();

        private void PushScope(Scope scope)
        {
            scopes.Push(scope);
            Allocate(scope.Symbols);
        }

        private void PopScope()
        {
            scopes.Pop();
            Allocate(CurrentScope.Symbols);
        }

        private Scope CurrentScope => scopes.Peek();

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

            PushScope(GetFunctionScope(code.Function.Name));

            Emit.Label(new Label(code.Function.Name));

            PopParamsToFunctionSpace(code.Function.Arguments);
        }

        public void Visit(CallCode code)
        {
            Emit.CurrentCode = code;

            var continuationLabel = new Label();

            GoToNewFrame(code.FunctionName, continuationLabel);

            Emit.CurrentDescription = null;

            Emit.Jump(new Label(code.FunctionName));
            Emit.Label(continuationLabel);

            RestoreAndGoToPreviousFrame(code.FunctionName);

          
            Emit.CurrentDescription = null;

            if (code.Reference != null)
            {
                Emit.Move(GetAddress(code.Reference), 0);
                Emit.Store(returnRegister, baseRegister, 0);
            }
        }

        private void PopParamsToFunctionSpace(ICollection<Argument> arguments)
        {
            Emit.CurrentDescription = "Popping arguments";
            
            if (!arguments.Any())
            {
                Emit.CurrentDescription = null;
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
                Emit.Move(GetAddress(argument.Reference), localAddress);
                Emit.Store(parameter, baseRegister, localAddress);
                Emit.Increment(offset);
            }

            Emit.CurrentDescription = null;
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
        }

        public void Visit(HaltCode code)
        {
            Emit.CurrentCode = code;

            Emit.Halt();
        }

        public void Visit(ParameterCode code)
        {
            var offset = new Register(0);
            var destination = new Register(1);
            
            Emit.CurrentCode = code;
            Emit.CurrentDescription = $"Pushing parameter {code.Reference}";

            Emit.Move(GetAddress(code.Reference), offset);
            Emit.Load(destination, baseRegister, offset);
            Emit.Push(destination);
        }

        private void GoToNewFrame(string functionName, Label label)
        {
            Emit.CurrentDescription = "Go to new frame";

            var symbolCount = CurrentScope.Symbols.Count;

            Emit.Transfer(baseRegister, 0);
            Emit.Transfer(stackRegister, 1);

            // New base register
            //Emit.AddInt(symbolCount, baseRegister);
            Emit.AddRegister(stackRegister, baseRegister);

            // New stack register
            Emit.Move(GetFunctionScope(functionName).Symbols.Count, stackRegister);

            Emit.Push(0);   // Base Register
            Emit.Push(1);   // Stack Register
            Emit.Push(label);
        }

        private void RestoreAndGoToPreviousFrame(string codeFunctionName)
        {
            Emit.CurrentDescription = "Restoring previous frame";

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