namespace Plotty.Uwp
{
    public class LineViewModel
    {
        public int Index { get; }
        public Line Line { get; }

        public LineViewModel(int index, Line line)
        {
            Index = index + 1;
            Line = line;
        }

        public string Description => Line.ToString();
    }
}