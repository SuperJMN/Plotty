using System.Collections.Generic;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using Plotty.CodeGeneration;

namespace Plotty.Compiler
{
    public class CompilationResult
    {
        public CompilationResult(GenerationResult generationResult, IReadOnlyCollection<string> code, SymbolTable symbolTable,
            List<IntermediateCode> intermediateCode)
        {
            Code = code;
            SymbolTable = symbolTable;
            IntermediateCode = intermediateCode;
            GenerationResult = generationResult;
        }

        public IReadOnlyCollection<string> Code { get;  }
        public SymbolTable SymbolTable { get; }
        public List<IntermediateCode> IntermediateCode { get; }
        public GenerationResult GenerationResult { get; }
    }
}