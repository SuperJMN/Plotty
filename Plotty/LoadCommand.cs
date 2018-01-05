namespace Plotty
{
    public class LoadCommand : Command
    {
        public LoadCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var inst = (LoadInstruction)PlottyCore.CurrentLine.Instruction;
            var value = inst.MemoryAddress.GetValue(PlottyCore);
            PlottyCore.Registers[inst.Destination.Id] = value;
           
            PlottyCore.GoToNext();
        }
    }
}