using System;
using Plotty.Model;

namespace Plotty.Commands
{
    public class BranchCommand : Command
    {
        public BranchCommand(IPlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var instruction = (BranchInstruction)PlottyCore.CurrentLine.Instruction;
            var r1 = PlottyCore.Registers[instruction.One.Id];
            var r2 = PlottyCore.Registers[instruction.Another.Id];

            if (ShouldBranch(r1, r2, instruction.Operator))
            {
                switch (instruction.Target)
                {
                    case LabelTarget jt:
                        PlottyCore.GoTo(jt.Label.Name);
                        break;
                    case SourceTarget rt:
                        PlottyCore.GoTo(rt.Target.GetValue(PlottyCore));
                        break;
                }
            }
            else
            {
                PlottyCore.GoToNext();
            }
        }

        private bool ShouldBranch(int a, int b, BooleanOperator @operator)
        {
            if (@operator == BooleanOperator.Equal)
            {
                return a == b;
            } 
            if (@operator == BooleanOperator.LessThan)
            {
                return a < b;
            } 

            throw new ArgumentOutOfRangeException(nameof(@operator));
        }
    }
}