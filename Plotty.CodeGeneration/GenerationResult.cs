using System.Collections.Generic;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class GenerationResult
    {
        public IList<Line> Lines { get; }

        public GenerationResult(IList<Line> lines)
        {
            Lines = lines;
        }
    }
}