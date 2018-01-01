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
            switch (inst.Source)
            {
                case ImmediateSource im:

                    PlottyCore.Memory[inst.Address] = im.Immediate;

                    break;
                case RegisterSource reg:

                    PlottyCore.Memory[inst.Address] = PlottyCore.Registers[reg.Register.Id];

                    break;
            }
            
            PlottyCore.GoToNext();
        }
    }
}