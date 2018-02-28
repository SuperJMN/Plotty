namespace Plotty
{
    public class HaltCommand : Command
    {
        public HaltCommand(PlottyCore plottyCore) : base(plottyCore)
        {            
        }

        public override void Execute()
        {
            PlottyCore.Status = Status.Halted;
        }
    }
}