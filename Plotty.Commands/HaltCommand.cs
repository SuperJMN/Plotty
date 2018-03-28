using Plotty.Model;

namespace Plotty.Commands
{
    public class HaltCommand : Command
    {
        public HaltCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {            
        }

        public override void Execute()
        {
            PlottyMachine.Status = Status.Halted;
        }
    }
}