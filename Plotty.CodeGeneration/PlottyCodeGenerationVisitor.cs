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
        private readonly List<PendingFixup> fixups = new List<PendingFixup>();
        private readonly List<Line> lines = new List<Line>();

        private readonly Register returnRegister = new Register(6);
        private readonly Register baseRegister = new Register(7);

        private Scope currentScope;
        private Dictionary<Reference, int> localAddress;
        

        public PlottyCodeGenerationVisitor(Scope scope)
        {
            CurrentScope = scope;
            RootScope = CurrentScope;
            Emit = new Emitter(this);
        }

        public IEnumerable<Line> Lines => lines.AsReadOnly();

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

        private Emitter Emit { get; }

        private IList<CodeLog> CodeLog { get; } = new List<CodeLog>();

        public ReadOnlyCollection<PendingFixup> Fixups => fixups.AsReadOnly();

        public void Visit(JumpIfFalse code)
        {
            Emit.LogVisitFor(code);

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
            Emit.LogVisitFor(code);

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
            Emit.LogVisitFor(code);

            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Move(code.Value, new Register(1));
            Emit.Store(new Register(1), baseRegister, 0);
        }

        public void Visit(ArithmeticAssignment code)
        {
            Emit.LogVisitFor(code);

            Emit.Move(GetAddress(code.Left), 0);
            Emit.Load(new Register(1), baseRegister, 0);

            Emit.Move(GetAddress(code.Right), 0);
            Emit.Load(new Register(2), baseRegister, 0);

            Emit.Move(GetAddress(code.Target), 0);

            var op = GetOperator(code.Operation);

            Emit.Arithmetic(op, 1, new RegisterSource(2));

            Emit.Store(1, baseRegister, 0);
        }

        public void Visit(ReferenceAssignment code)
        {
            Emit.LogVisitFor(code);

            Emit.Move(GetAddress(code.Origin), new Register(0));
            Emit.Load(new Register(1), baseRegister, 0);
            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Store(new Register(1), baseRegister, 0);
        }

        public void Visit(BoolExpressionAssignment code)
        {
            Emit.LogVisitFor(code);

            if (code.Operation == BooleanOperation.IsEqual)
            {
                Emit.Move(GetAddress(code.Left), new Register(0));
                Emit.Load(new Register(1), baseRegister, new Register(0));

                Emit.Move(GetAddress(code.Right), new Register(0));
                Emit.Load(new Register(2), baseRegister, new Register(0));

                Emit.Arithmetic(ArithmeticOperator.Substract, 1, new RegisterSource(2));

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
            Emit.LogVisitFor(code);

            Emit.Jump(new Label(code.Label.Name));
        }

        public void Visit(FunctionDefinitionCode code)
        {
            Emit.LogVisitFor(code);

            var functionName = code.Function.Name;
            CurrentScope = GetFuntionScope(functionName);

            Emit.Label(new Label(functionName));                        
        }

        public void Visit(CallCode code)
        {
            Emit.LogVisitFor(code);

            var continuationLabel = new Label();

            var symbolsCount = CurrentScope.Symbols.Count;

            Emit.Arithmetic(ArithmeticOperator.Add, baseRegister, new ImmediateSource(symbolsCount));

            PushAddressOfLabel(continuationLabel);
            
            Emit.Jump(new Label(code.FunctionName));
            Emit.Label(continuationLabel);

            Emit.Arithmetic(ArithmeticOperator.Substract, baseRegister, new ImmediateSource(symbolsCount));


            if (code.Reference != null)
            {
                Emit.Move(GetAddress(code.Reference), 0);
                Emit.Store(returnRegister, baseRegister, 0);
            }
        }

        public void Visit(ReturnCode code)
        {
            Emit.LogVisitFor(code);

            if (code.Reference != null)
            {
                Emit.Move(GetAddress(code.Reference), 0);
                Emit.Load(returnRegister, baseRegister, 0);
            }
            
            PopTo(0);
            Emit.Jump(0);
            
            CurrentScope = CurrentScope.Parent;
        }

        public void Visit(HaltCode code)
        {
            Emit.LogVisitFor(code);

            Emit.Halt();
        }

        private Scope GetFuntionScope(string functionName)
        {
            return RootScope.Children.Single(s => (s.Owner as Function)?.Name == functionName);
        }

        private void PushAddressOfLabel(Label label)
        {
            Emit.Move(-1, 0);
            var move = GetLast();
            fixups.Add(new PendingFixup(move, new ReplaceByLabelAddressFixup(label)));
            Emit.Store(0, baseRegister);
            Emit.Increment(baseRegister);
        }

        private void Push(int value)
        {
            Emit.Move(value, 0);
            Emit.Load(0, baseRegister);
            Emit.Increment(baseRegister);
        }

        private void PopTo(Register destination)
        {
            Emit.Decrement(baseRegister);
            Emit.Load(destination, baseRegister);            
        }

        private Line GetLast()
        {
            return lines.Last();
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