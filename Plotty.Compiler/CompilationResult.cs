using System.Collections.Generic;
using CodeGen.Parsing.Ast;
using Plotty.CodeGeneration;

namespace Plotty.Compiler
{
    public class CompilationResult
    {
        public CompilationResult(GenerationResult generationResult, IReadOnlyCollection<string> code, Scope scope)
        {
            Code = code;
            Scope = scope;
            GenerationResult = generationResult;
        }

        public IReadOnlyCollection<string> Code { get;  }
        public Scope Scope { get; }
        public GenerationResult GenerationResult { get; }
    }
}