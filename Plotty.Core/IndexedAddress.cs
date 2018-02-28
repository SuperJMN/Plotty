namespace Plotty.Core
{
    public class IndexedAddress : MemoryAddress
    {
        public Register BaseRegister { get; }
        public Source Offset { get; }

        public IndexedAddress(Register baseRegister, Source offset)
        {
            BaseRegister = baseRegister;
            Offset = offset;
        }

        public override int GetAddress(PlottyCore plottyCore)
        {
            return plottyCore.Registers[BaseRegister.Id] + Offset.GetValue(plottyCore);
        }

        public override string ToString()
        {
            return $"{BaseRegister} + {Offset}";
        }
    }
}