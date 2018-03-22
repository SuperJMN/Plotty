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
            if (r1 != r2)
            {
                switch (instruction.Target)
                {
                    case LabelTarget jt:
                        PlottyCore.GoTo(jt.Label);
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
    }
}