namespace Plotty
{
    public class StoreCommand : Command
    {
        public StoreCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var inst = (StoreInstruction)PlottyCore.CurrentLine.Instruction;

            int value = inst.Source.GetValue(PlottyCore);
            var address = inst.Address.GetAddress(PlottyCore);

            PlottyCore.Memory[address] = value;
           
            PlottyCore.GoToNext();
        }
    }
}