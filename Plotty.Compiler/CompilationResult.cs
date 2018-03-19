using System.Collections.Generic;
using Plotty.CodeGeneration;

namespace Plotty.Compiler
{
    public class CompilationResult
    {
        public CompilationResult(GenerationResult generationResult, IReadOnlyCollection<string> lines)
        {
            Lines = lines;
            GenerationResult = generationResult;
        }

        public IReadOnlyCollection<string> Lines { get;  }
        public GenerationResult GenerationResult { get; }
    }
}