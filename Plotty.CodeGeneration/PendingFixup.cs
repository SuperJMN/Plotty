using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class PendingFixup
    {
        public PendingFixup(Line line, LineFixer lineFixer)
        {
            Line = line;
            LineFixer = lineFixer;
        }

        public Line Line { get; }
        public LineFixer LineFixer { get; }
    }
}