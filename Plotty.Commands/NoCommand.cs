using Plotty.Model;

namespace Plotty.Commands
{
    public class NoCommand : Command
    {
        public NoCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {
        }

        public override void Execute()
        {            
            PlottyMachine.GoToNext();
        }
    }
}