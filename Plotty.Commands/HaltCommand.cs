using Plotty.Model;

namespace Plotty.Commands
{
    public class HaltCommand : Command
    {
        public HaltCommand(IPlottyCore plottyCore) : base(plottyCore)
        {            
        }

        public override void Execute()
        {
            PlottyCore.Status = Status.Halted;
        }
    }
}