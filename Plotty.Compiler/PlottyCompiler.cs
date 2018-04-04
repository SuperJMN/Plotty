using System.Collections.Generic;
using System.Linq;
using CodeGen.Intermediate;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing;
using CodeGen.Parsing.Ast;
using Plotty.CodeGeneration;
using Superpower;
using TokenizerFactory = CodeGen.Parsing.Tokenizer.TokenizerFactory;

namespace Plotty.Compiler
{
    public class PlottyCompiler
    {
        public CompilationResult Compile(string source)
        {
            Program ast = GenerateAst(source);
            Analize(ast);
            var scope = GetScope(ast);
            var intermediateCode = GenerateIntermediateCode(ast);
            var result = GeneratePlottyCode(intermediateCode, scope);            
            var assemblyCode = GenerateAssemblyCode(result);
            return new CompilationResult(result, assemblyCode);
        }

        private Scope GetScope(ICodeUnit ast)
        {
            var scanner = new ScopeScanner();
            ast.Accept(scanner);
            return scanner.Scope;
        }

        private static IReadOnlyCollection<string> GenerateAssemblyCode(GenerationResult result)
        {
            var plottyAssemblyVisitor = new AssemblyGeneratingVisitor();

            foreach (var line in result.Lines)
            {
                line.Accept(plottyAssemblyVisitor);
            }

            return plottyAssemblyVisitor.Lines;
        }

        private static GenerationResult GeneratePlottyCode(IEnumerable<IntermediateCode> intermediateCode, Scope scope)
        {
            var plottyGenerator = new PlottyCodeGenerator();

            var generationResult = plottyGenerator.Generate(intermediateCode.ToList(), scope);
            return generationResult;
        }

        private static List<IntermediateCode> GenerateIntermediateCode(ICodeUnit ast)
        {
            var generator = new IntermediateCodeGenerator();
            var codes = generator.Generate(ast).ToList();
            return codes;
        }

        private static void Analize(Program program)
        {
            var analizer = new SemanticAnalizer();
            analizer.Verify(program);
        }

        private static Program GenerateAst(string source)
        {
            var tokens = TokenizerFactory.Create().Tokenize(source);
            var ast = Parsers.Program.Parse(tokens);
            return ast;
        }
    }
}