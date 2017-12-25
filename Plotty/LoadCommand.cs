namespace Plotty
{
    public class LoadCommand : Command
    {
        private readonly Register register;
        private readonly int address;

        public LoadCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public LoadCommand(PlottyCore plottyCore, Register register, int address) : base(plottyCore)
        {
            this.register = register;
            this.address = address;
        }

        public override void Execute()
        {
            PlottyCore.Registers[register.Number] = PlottyCore.Memory[address];
            PlottyCore.Next();
        }
    }
}