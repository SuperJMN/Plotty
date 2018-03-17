namespace Plotty.Common
{
    public abstract class MemoryAddress
    {
        public abstract int GetAddress(IPlottyCore plottyCore);

        public int GetValue(IPlottyCore plottyCore)
        {
            return plottyCore.Memory[GetAddress(plottyCore)];
        }
    }
}