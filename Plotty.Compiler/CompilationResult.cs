using System.Collections.Generic;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using Plotty.CodeGeneration;

namespace Plotty.Compiler
{
    public class CompilationResult
    {
        public CompilationResult(GenerationResult generationResult, IReadOnlyCollection<string> code, Scope scope,
            List<IntermediateCode> intermediateCode)
        {
            Code = code;
            Scope = scope;
            IntermediateCode = intermediateCode;
            GenerationResult = generationResult;
        }

        public IReadOnlyCollection<string> Code { get;  }
        public Scope Scope { get; }
        public List<IntermediateCode> IntermediateCode { get; }
        public GenerationResult GenerationResult { get; }
    }
}