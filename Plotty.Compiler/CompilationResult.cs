using System.Collections.Generic;
using Plotty.CodeGeneration;

namespace Plotty.Compiler
{
    public class CompilationResult
    {
        public CompilationResult(GenerationResult generationResult, IReadOnlyCollection<string> code)
        {
            Code = code;
            GenerationResult = generationResult;
        }

        public IReadOnlyCollection<string> Code { get;  }
        public GenerationResult GenerationResult { get; }
    }
}