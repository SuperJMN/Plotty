using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class PendingFixup
    {
        public PendingFixup(ILine line, LineFixer lineFixer)
        {
            Line = line;
            LineFixer = lineFixer;
        }

        public ILine Line { get; }
        public LineFixer LineFixer { get; }
    }
}