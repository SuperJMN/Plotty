using System;
using System.Collections.Generic;
using CodeGen.Core;
using CodeGen.Intermediate;
using CodeGen.Intermediate.Codes;
using CodeGen.Intermediate.Codes.Common;
using Plotty.Model;
using ArithmeticOperator = Plotty.Model.ArithmeticOperator;
using Label = Plotty.Model.Label;

namespace Plotty.CodeGeneration
{
    public class PlottyCodeGenerationVisitor : IIntermediateCodeVisitor
    {
        private readonly IDictionary<Reference, int> addressMap;
        private readonly List<Line> instructions = new List<Line>();
        public IReadOnlyCollection<Line> Lines => instructions.AsReadOnly();

        public PlottyCodeGenerationVisitor(IDictionary<Reference, int> addressMap)
        {
            this.addressMap = addressMap;
        }

        public void Visit(JumpIfFalse code)
        {
            Add(MoveImmediate(addressMap[code.Reference], new Register(0)));
            Add(Load(new Register(1), new Register(0)));

            var onFalseLabel = new Label();

            // Comparison with 0 (TRUE). If both values are 0, the conditions is true, so the code has to be executed.
            // So we emit
            Add(MoveImmediate(0, new Register(0)));
            Add(new Line(new BranchInstruction
            {
                Operator = BooleanOperator.Equal,
                One = new Register(1),
                Another = new Register(0),
                Target = new LabelTarget(onFalseLabel),
            }));

            Add(MoveImmediate(0, new Register(0)));

            Add(new Line(new BranchInstruction
            {
                Operator = BooleanOperator.Equal,
                Target = new LabelTarget(new Label(code.Label.Name)),
                One = new Register(0),
                Another = new Register(0),
            }));

            Add(new Line(onFalseLabel, null));
        }

        public void Visit(BoolConstantAssignment code)
        {
            Add(MoveImmediate(addressMap[code.Target], new Register(0)));
            Add(MoveImmediate(code.Value ? 0 : 1, new Register(1)));
            Add(Store(new Register(1), new Register(0)));
        }

        public void Visit(LabelCode code)
        {
            Add(new Line(new Label(code.Label.Name), null));
        }

        public void Visit(IntegerConstantAssignment code)
        {
            Add(MoveImmediate(addressMap[code.Target], new Register(0)));
            Add(MoveImmediate(code.Value, new Register(1)));
            Add(Store(new Register(1), new Register(0)));
        }

        private void Add(Line line)
        {
            instructions.Add(line);
        }

        public void Visit(ArithmeticAssignment code)
        {
            Add(MoveImmediate(addressMap[code.Left], new Register(0)));
            Add(Load(new Register(1), new Register(0)));

            Add(MoveImmediate(addressMap[code.Right], new Register(0)));
            Add(Load(new Register(2), new Register(0)));

            Add(MoveImmediate(addressMap[code.Target], new Register(0)));

            var @operator = GetOperator(code.Operation);

            Add(new Line(new ArithmeticInstruction
            {
                ArithmeticOperator = @operator,
                Left = new Register(1),
                Right = new RegisterSource(new Register(2)),
                Destination = new Register(1),
            }));

            Add(Store(new Register(1), new Register(0)));
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
            Add(MoveImmediate(addressMap[code.Origin], new Register(0)));
            Add(Load(new Register(1), new Register(0)));
            Add(MoveImmediate(addressMap[code.Target], new Register(0)));
            Add(Store(new Register(1), new Register(0)));
        }

        public void Visit(BoolExpressionAssignment code)
        {
            if (code.Operation == BooleanOperation.IsEqual)
            {
                Add(MoveImmediate(addressMap[code.Left], new Register(0)));
                Add(Load(new Register(1), new Register(0)));

                Add(MoveImmediate(addressMap[code.Right], new Register(0)));
                Add(Load(new Register(2), new Register(0)));

                Add(new Line(new ArithmeticInstruction
                {
                    ArithmeticOperator = ArithmeticOperator.Substract,
                    Left = new Register(1),
                    Right = new RegisterSource(new Register(2)),
                    Destination = new Register(1),
                }));

                Add(MoveImmediate(addressMap[code.Target], new Register(0)));
                Add(Store(new Register(1), new Register(0)));
            }

            if (code.Operation == BooleanOperation.IsLessThan)
            {
                Add(MoveImmediate(addressMap[code.Left], new Register(0)));
                Add(Load(new Register(1), new Register(0)));

                Add(MoveImmediate(addressMap[code.Right], new Register(0)));
                Add(Load(new Register(2), new Register(0)));

                var jumpOnTrue = new Label();
                var endLabel = new Label();

                Add(new Line(new BranchInstruction()
                {
                    Operator = BooleanOperator.LessThan,
                    One = new Register(1),
                    Another = new Register(2),
                    Target = new LabelTarget(jumpOnTrue),
                }));
                
                // Sets false
                MoveImmediate(1, new Register(1));
                Add(UnconditionalJump(endLabel));
                Add(new Line(jumpOnTrue, null));

                // Sets true
                Add(MoveImmediate(0, new Register(1)));
                
                // End
                Add(new Line(endLabel, null));

                Add(MoveImmediate(addressMap[code.Target], new Register(0)));
                Add(Store(new Register(1), new Register(0)));
            }
        }

        public void Visit(Jump code)
        {
            Add(UnconditionalJump(new Label(code.Label.Name)));
        }

        private static Line UnconditionalJump(Label label)
        {
            return new Line(new BranchInstruction
            {
                Operator = BooleanOperator.Equal,
                One = new Register(0),
                Another = new Register(0),
                Target = new LabelTarget(label)
            });
        }

        private static Line Store(Register register, Register baseRegister, int offset = 0)
        {
            return new Line(new StoreInstruction
            {
                Source = new RegisterSource(register),
                MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
            });
        }

        private static Line Load(Register register, Register baseRegister, int offset = 0)
        {
            return new Line(new LoadInstruction
            {
                Destination = register,
                MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
            });
        }

        private static Line MoveImmediate(int immediate, Register destination)
        {
            return new Line(new MoveInstruction
            {
                Destination = destination,
                Source = new ImmediateSource(immediate),
            });
        }
    }
}