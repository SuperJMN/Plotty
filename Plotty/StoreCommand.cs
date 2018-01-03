using System;

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

            int value = PlottyCore.GetSourceValue(inst.Source);
            var address = PlottyCore.GetAddress(inst.Address);

            PlottyCore.Memory[address] = value;
           
            PlottyCore.GoToNext();
        }
    }

    public static class PlottyCoreExtensions
    {
        public static int GetSourceValue(this PlottyCore self, Source source)
        {
            switch (source)
            {
                case RegisterSource rs:
                    return self.Registers[rs.Register.Id];
                case ImmediateSource ims:
                    return ims.Immediate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static int GetAddress(this PlottyCore self, MemoryAddress address)
        {
            switch (address)
            {
                case ImmediateAddress rs:
                    return rs.BaseAddress;
                case RelativeAddress ims:
                    return ims.BaseAddress + self.GetSourceValue(ims.Source);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}