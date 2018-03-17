using Plotty.Model;

namespace Plotty.Commands
{
    
    public abstract class Command
    {
        protected IPlottyCore PlottyCore { get; }

        public Command(IPlottyCore plottyCore)
        {
            PlottyCore = plottyCore;
        }

        public abstract void Execute();
    }
}