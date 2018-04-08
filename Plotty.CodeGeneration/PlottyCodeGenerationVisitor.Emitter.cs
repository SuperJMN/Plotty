using System.Linq;
using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public partial class PlottyCodeGenerationVisitor
    {
        private class Emitter
        {
            public Emitter(PlottyCodeGenerationVisitor visitor)
            {
                Visitor = visitor;
            }

            private PlottyCodeGenerationVisitor Visitor { get; }

            private void Add(Line line)
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

            public void Store(Register subject, Register baseRegister, int offset = 0)
            {
                Add(new Line(new StoreInstruction
                {
                    Source = new RegisterSource(subject),
                    MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
                }));
            }

            public void Move(int immediate, Register destination)
            {
                Add(new Line(new MoveInstruction
                {
                    Destination = destination,
                    Source = new ImmediateSource(immediate)
                }));
            }

            public void Load(Register subject, Register baseRegister, int offset = 0)
            {
                Add(new Line(new LoadInstruction
                {
                    Destination = subject,
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
                    Target = new LabelTarget(label)
                }));
            }

            public void Arithmetic(ArithmeticOperator @operator, Register register, RegisterSource registerSource,
                Register destination = null)
            {
                Add(new Line(new ArithmeticInstruction
                {
                    ArithmeticOperator = @operator,
                    Left = register,
                    Right = registerSource,
                    Destination = destination ?? register
                }));
            }

            public void Increment(Register register)
            {
                Move(1, 0);
                Arithmetic(ArithmeticOperator.Add, register, new RegisterSource(0));
            }

            public void Decrement(Register register)
            {
                Move(1, 0);
                Arithmetic(ArithmeticOperator.Substract, register, new RegisterSource(0));
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
                Visitor.CodeLog.Add(new CodeLog(code));
            }

            public void Halt()
            {
                Add(new Line(new HaltInstruction()));
            }
        }
    }
}