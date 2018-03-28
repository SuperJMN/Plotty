namespace Plotty.Model
{
    public abstract class MemoryAddress
    {
        public abstract int GetAddress(IPlottyMachine plottyMachine);

        public int GetValue(IPlottyMachine plottyMachine)
        {
            return plottyMachine.Memory[GetAddress(plottyMachine)];
        }
    }
}