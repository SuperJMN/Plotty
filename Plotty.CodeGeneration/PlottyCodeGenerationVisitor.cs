using System;
using System.Collections.Generic;
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
        private readonly List<Line> instructions = new List<Line>();
        private Scope currentScope;
        private Dictionary<Reference, int> localAddress;
        public IEnumerable<Line> Lines => instructions.AsReadOnly();

        private Scope CurrentScope
        {
            get => currentScope;
            set
            {
                currentScope = value;
                Allocate(currentScope.Symbols);
            }
        }

        private void Allocate(IReadOnlyDictionary<Reference, Symbol> currentScopeSymbols)
        {
            localAddress = currentScopeSymbols.Select((x, i) => new {x, i}).ToDictionary(x => x.x.Key, x => x.i);
        }

        public PlottyCodeGenerationVisitor(Scope scope)
        {
            CurrentScope = scope;
            RootScope = CurrentScope;
            Emit = new Emitter(this);
        }

        public Scope RootScope { get; }

        private Emitter Emit { get; }

        public void Visit(JumpIfFalse code)
        {
            Emit.Move(GetAddress(code.Reference), new Register(0));
            Emit.Load(new Register(1), new Register(0));

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
            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Move(code.Value ? 0 : 1, new Register(1));
            Emit.Store(new Register(1), new Register(0));
        }

        private int GetAddress(Reference reference)
        {
            return localAddress[reference];
        }

        public void Visit(LabelCode code)
        {
            Emit.Label(new Label(code.Label.Name));
        }

        public void Visit(IntegerConstantAssignment code)
        {
            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Move(code.Value, new Register(1));
            Emit.Store(new Register(1), new Register(0));
        }

        private void Add(Line line)
        {
            instructions.Add(line);
        }

        public void Visit(ArithmeticAssignment code)
        {
            Emit.Move(GetAddress(code.Left), new Register(0));
            Emit.Load(new Register(1), new Register(0));

            Emit.Move(GetAddress(code.Right), new Register(0));
            Emit.Load(new Register(2), new Register(0));

            Emit.Move(GetAddress(code.Target), new Register(0));

            var @operator = GetOperator(code.Operation);

            Add(new Line(new ArithmeticInstruction
            {
                ArithmeticOperator = @operator,
                Left = new Register(1),
                Right = new RegisterSource(new Register(2)),
                Destination = new Register(1),
            }));

            Emit.Store(new Register(1), new Register(0));
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

        public void Visit(ReferenceAssignment code)
        {
            Emit.Move(GetAddress(code.Origin), new Register(0));
            Emit.Load(new Register(1), new Register(0));
            Emit.Move(GetAddress(code.Target), new Register(0));
            Emit.Store(new Register(1), new Register(0));
        }

        public void Visit(BoolExpressionAssignment code)
        {
            if (code.Operation == BooleanOperation.IsEqual)
            {
                Emit.Move(GetAddress(code.Left), new Register(0));
                Emit.Load(new Register(1), new Register(0));

                Emit.Move(GetAddress(code.Right), new Register(0));
                Emit.Load(new Register(2), new Register(0));

                Add(new Line(new ArithmeticInstruction
                {
                    ArithmeticOperator = ArithmeticOperator.Substract,
                    Left = new Register(1),
                    Right = new RegisterSource(new Register(2)),
                    Destination = new Register(1),
                }));

                Emit.Move(GetAddress(code.Target), new Register(0));
                Emit.Store(new Register(1), new Register(0));
            }
            else
            {
                Emit.Move(GetAddress(code.Left), new Register(0));
                Emit.Load(new Register(1), new Register(0));

                Emit.Move(GetAddress(code.Right), new Register(0));
                Emit.Load(new Register(2), new Register(0));

                var jumpOnTrue = new Label();
                var endLabel = new Label();

                Add(new Line(new BranchInstruction()
                {
                    Operator = code.Operation.ToOperator(),
                    One = new Register(1),
                    Another = new Register(2),
                    Target = new LabelTarget(jumpOnTrue),
                }));

                // Sets false
                Emit.Move(1, new Register(1));
                Emit.Jump(endLabel);
                Emit.Label(jumpOnTrue);

                // Sets true
                Emit.Move(0, new Register(1));

                // End
                Emit.Label(endLabel);

                Emit.Move(GetAddress(code.Target), new Register(0));
                Emit.Store(new Register(1), new Register(0));
            }
        }


        public void Visit(Jump code)
        {
            Emit.Jump(new Label(code.Label.Name));
        }

        public void Visit(FunctionDefinitionCode def)
        {
            CurrentScope = RootScope.Children.Single(s => (s.Owner as Function)?.Name == def.Function.Name);
            Emit.Label(new Label(def.Function.Name));
        }

        public void Visit(CallCode code)
        {
            Emit.Jump(new Label(code.FunctionName));
        }

        public void Visit(ReturnCode code)
        {
            CurrentScope = CurrentScope.Parent;
        }

        private class Emitter
        {
            private PlottyCodeGenerationVisitor Visitor { get; }

            public Emitter(PlottyCodeGenerationVisitor visitor)
            {
                Visitor = visitor;
            }

            private void Add(Line line)
            {
                Visitor.instructions.Add(line);
            }

            public void Label(Label label)
            {
                Add(new Line(label, null));
            }

            public void Jump(Label label)
            {
                Add(new Line(new BranchInstruction
                {
                    Operator = BooleanOperator.Equal,
                    One = new Register(0),
                    Another = new Register(0),
                    Target = new LabelTarget(label)
                }));
            }

            public void Store(Register register, Register baseRegister, int offset= 0)
            {
                Add(new Line(new StoreInstruction
                {
                    Source = new RegisterSource(register),
                    MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
                }));
            }

            public void Move(int immediate, Register destination)
            {
                Add(new Line(new MoveInstruction
                {
                    Destination = destination,
                    Source = new ImmediateSource(immediate),
                }));
            }

            public void Load(Register register, Register baseRegister, int offset = 0)
            {
                Add(new Line(new LoadInstruction
                {
                    Destination = register,
                    MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
                }));
            }

            public void Branch(BooleanOperator op, Register r1, Register r2, Label label)
            {
                Add(new Line(new BranchInstruction
                {
                    Operator = op,
                    One = r1,
                    Another = r2,
                    Target = new LabelTarget(label),
                }));
            }
        }
    }
}