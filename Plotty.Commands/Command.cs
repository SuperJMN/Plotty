using Plotty.Model;

namespace Plotty.Commands
{
    
    public abstract class Command
    {
        protected IPlottyMachine PlottyMachine { get; }

        public Command(IPlottyMachine plottyMachine)
        {
            PlottyMachine = plottyMachine;
        }

        public abstract void Execute();
    }
}