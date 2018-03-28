using System.Runtime.CompilerServices;

namespace Plotty.Model
{
    public interface IPlottyMachine
    {
        int[] Registers { get; }
        int[] Memory { get; }
        Status Status { get; set; }
        Line CurrentLine { get; set; }
        void GoToNext();
        void GoTo(string label);
        void GoTo(int instructionOrdinal);
    }
}