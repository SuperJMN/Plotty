namespace Plotty
{
    public class StoreCommand : Command
    {
        public StoreCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            //var registerId = PlottyCore.CurrentInstruction.FirstRegister.Id;
            //var memId = PlottyCore.CurrentInstruction.Address.Address;
            //PlottyCore.Memory[memId] = PlottyCore.Registers[registerId];

            PlottyCore.GoToNext();
        }
    }
}