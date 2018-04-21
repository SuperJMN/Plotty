using System;
using Plotty.Model;

namespace Plotty.Commands
{
    public class BranchCommand : Command
    {
        public BranchCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {
        }

        public override void Execute()
        {
            var instruction = (BranchInstruction)PlottyMachine.CurrentLine.Instruction;
            var r1 = PlottyMachine.Registers[instruction.One.Id];
            var r2 = PlottyMachine.Registers[instruction.Another.Id];

            if (ShouldBranch(r1, r2, instruction.Operator))
            {
                switch (instruction.Target)
                {
                    case LabelTarget jt:
                        PlottyMachine.GoTo(jt.Label.Name);
                        break;
                    case SourceTarget rt:
                        PlottyMachine.GoTo(rt.Target.GetValue(PlottyMachine));
                        break;
                }
            }
            else
            {
                PlottyMachine.GoToNext();
            }
        }

        private bool ShouldBranch(int a, int b, BooleanOperator op)
        {
            if (op == BooleanOperator.Equal)
            {
                return a == b;
            } 
            if (op == BooleanOperator.LessThan)
            {
                return a < b;
            }

            if (op == BooleanOperator.GreaterThan)
            {
                return a > b;
            }

            if (op == BooleanOperator.GreaterOrEqual)
            {
                return a >= b;
            }

            if (op == BooleanOperator.LessThanOrEqual)
            {
                return a <= b;
            }

            if (op == BooleanOperator.And)
            {
                return a == 0 && b  == 0;
            }

            if (op == BooleanOperator.Or)
            {
                return a == 0 || b  == 0;
            }

            throw new ArgumentOutOfRangeException(nameof(op));
        }
    }
}