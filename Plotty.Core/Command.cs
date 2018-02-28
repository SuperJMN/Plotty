namespace Plotty.Core
{
    
    public abstract class Command
    {
        protected PlottyCore PlottyCore { get; }

        public Command(PlottyCore plottyCore)
        {
            this.PlottyCore = plottyCore;
        }

        public abstract void Execute();
    }
}