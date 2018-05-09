namespace Plotty.Model
{
    public interface IPlottyMachine
    {
        int[] Registers { get; }
        int[] Memory { get; }
        Status Status { get; set; }
        ILine CurrentLine { get; set; }
        bool CanExecute { get; }
        void GoToNext();
        void GoTo(string label);
        void GoTo(int instructionOrdinal);
        void Execute();
    }
}