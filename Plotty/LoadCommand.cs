namespace Plotty
{
    public class LoadCommand : Command
    {
        private readonly Register register;
        private readonly LoadParam address;

        public LoadCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public LoadCommand(PlottyCore plottyCore, Register register, LoadParam address) : base(plottyCore)
        {
            this.register = register;
            this.address = address;
        }

        public override void Execute()
        {
            if (address.IsDirect)
            {
                PlottyCore.Registers[register.Number] = address.Value;
            }
            else
            {
                PlottyCore.Registers[register.Number] = PlottyCore.Memory[address.Address];
            }
            
            PlottyCore.Next();
        }
    }
}