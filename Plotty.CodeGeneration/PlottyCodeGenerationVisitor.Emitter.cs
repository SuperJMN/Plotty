using System.Linq;
using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public partial class PlottyCodeGenerationVisitor
    {
        private class Emitter
        {
            protected Emitter(PlottyCodeGenerationVisitor visitor)
            {
                Visitor = visitor;
            }

            protected PlottyCodeGenerationVisitor Visitor { get; }

            protected virtual void Add(Line line)
            {
                Visitor.lines.Add(line);
                Visitor.CodeLog.Last().AddLine(line);
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

            public void Store(Register subject, Register baseRegister, Register offset = null)
            {
                var source = offset == null ? (Source) new ImmediateSource(0) : new RegisterSource(offset);

                Add(new Line(new StoreInstruction
                {
                    Source = new RegisterSource(subject),
                    MemoryAddress = new IndexedAddress(baseRegister, source)
                }));
            }

            public void Load(Register subject, Register baseRegister, Register offset = null)
            {
                Source source;
                if (offset == null)
                {
                    source = new ImmediateSource(0);
                }
                else
                {
                    source = new RegisterSource(offset);
                }

                Add(new Line(new LoadInstruction
                {
                    Destination = subject,
                    MemoryAddress = new IndexedAddress(baseRegister, source)
                }));
            }

            public void Branch(BooleanOperator op, Register r1, Register r2, Label label)
            {
                Add(new Line(new BranchInstruction
                {
                    Operator = op,
                    One = r1,
                    Another = r2,
                    Target = new LabelTarget(label)
                }));
            }

            public void Arithmetic(ArithmeticOperator op, Register register, Source registerSource,
                Register destination = null)
            {
                Add(new Line(new ArithmeticInstruction
                {
                    ArithmeticOperator = op,
                    Left = register,
                    Right = registerSource,
                    Destination = destination ?? register
                }));
            }

            public void Increment(Register register)
            {
                Add(1, register);
            }

            public void Decrement(Register register)
            {
                Substract(register, 1);
            }

            public void Jump(Register register)
            {
                Add(new Line(new BranchInstruction
                {
                    Operator = BooleanOperator.Equal,
                    One = new Register(0),
                    Another = new Register(0),
                    Target = new SourceTarget(new RegisterSource(register))
                }));
            }

            public void LogVisitFor(IntermediateCode code)
            {
                Visitor.CodeLog.Add(new CodeLogItem(code));
            }

            public void Halt()
            {
                Add(new Line(new HaltInstruction()));
            }

            public void Pop(Register target)
            {
                Decrement(Visitor.stackRegister);
                Load(target, Visitor.baseRegister, Visitor.stackRegister);
            }

            public void Push(Register source)
            {
                Store(source, Visitor.baseRegister, Visitor.stackRegister);
                Increment(Visitor.stackRegister);
            }

            public void Push(Label label)
            {
                Move(0, -1);
                var move = GetLast();
                Visitor.fixups.Add(new PendingFixup(move, new ReplaceByLabelAddressFixup(label)));

                Push(0);
            }

            private ILine GetLast()
            {
                return Visitor.lines.Last();
            }

            public void Add(Register register, Register second, Register destination = null)
            {
                Arithmetic(ArithmeticOperator.Add, register, new RegisterSource(second), destination);
            }

            public void Substract(Register first, Register second, Register destination = null)
            {
                Arithmetic(ArithmeticOperator.Substract, first, new RegisterSource(second), destination);
            }

            public void Add(int value, Register register, Register destination = null)
            {
                Arithmetic(ArithmeticOperator.Add, register, new ImmediateSource(value), destination);
            }

            public void Substract(Register first, int value, Register destination = null)
            {
                Arithmetic(ArithmeticOperator.Substract, first, new ImmediateSource(value), destination);
            }

            public void Arithmetic(ArithmeticOperator op, Register register, Register second,
                Register destination = null)
            {
                Arithmetic(op, register, new RegisterSource(second), destination);
            }

            public void Arithmetic(ArithmeticOperator op, Register register, int value, Register destination = null)
            {
                Arithmetic(op, register, new ImmediateSource(value), destination);
            }

            public void Transfer(Register source, Register destination)
            {
                Add(new Line(new MoveInstruction
                {
                    Destination = destination,
                    Source = new RegisterSource(source)
                }));
            }

            public void Move(Register destination, int immediate)
            {
                Add(new Line(new MoveInstruction
                {
                    Destination = destination,
                    Source = new ImmediateSource(immediate)
                }));
            }
        }

        private class ContextualizedEmitter : Emitter
        {
            public ContextualizedEmitter(PlottyCodeGenerationVisitor visitor) : base(visitor)
            {
            }

            protected override void Add(Line line)
            {
                Visitor.lines.Add(new ContextualLine(line, CurrentCode, CurrentDescription));
            }

            public IntermediateCode CurrentCode { get; set; }
            public string CurrentDescription { get; set; }
        }
    }
}