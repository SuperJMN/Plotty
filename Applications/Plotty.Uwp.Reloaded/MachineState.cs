using Plotty.Model;

namespace Plotty.Uwp.Reloaded
{
    public class MachineState
    {
        public int[] Registers { get; }
        public int[] Memory { get; }
        public ILine CurrentLine { get; }
        public int ReturnedValue => Registers[5];

        public MachineState(int[] registers, int[] memory, ILine currentLine)
        {
            Registers = registers;
            Memory = memory;
            CurrentLine = currentLine;
        }
    }
}