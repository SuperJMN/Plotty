using Plotty.Model;

namespace Plotty.Uwp
{
    public class LineViewModel
    {
        public int Index { get; }
        public ILine Line { get; }

        public LineViewModel(int index, ILine line)
        {
            Index = index + 1;
            Line = line;
        }

        public string Description => Line.ToString();
    }
}