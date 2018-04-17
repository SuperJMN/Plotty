using System.Collections.Generic;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class GenerationResult
    {
        public List<ILine> Lines { get; }

        public GenerationResult(List<ILine> lines)
        {
            Lines = lines;
        }
    }
}