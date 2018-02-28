namespace Plotty.Core
{
    public abstract class MemoryAddress
    {
        public abstract int GetAddress(PlottyCore plottyCore);

        public int GetValue(PlottyCore plottyCore)
        {
            return plottyCore.Memory[GetAddress(plottyCore)];
        }
    }
}