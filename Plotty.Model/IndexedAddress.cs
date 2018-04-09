namespace Plotty.Model
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

        public override int GetAddress(IPlottyMachine plottyMachine)
        {
            return plottyMachine.Registers[BaseRegister.Id] + Offset.GetValue(plottyMachine);
        }

        public override string ToString()
        {
            if (Offset is ImmediateSource a && a.Immediate == 0)
            {
                return $"{BaseRegister}";
            }

            return $"{BaseRegister} + {Offset}";
        }
    }
}