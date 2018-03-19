using System.Linq;
using CodeGen.Ast;
using CodeGen.Ast.Parsers;
using CodeGen.Intermediate;
using Plotty.CodeGeneration;
using Superpower;

namespace Plotty.Compiler
{
    public class PlottyCompiler
    {
        public CompilationResult Compile(string source)
        {
            var tokens = TokenizerFactory.Create().Tokenize(source);
            var parsed = Statements.ProgramParser.Parse(tokens);

            var generator = new IntermediateCodeGenerator();
            var codes = generator.Generate(parsed).ToList();
            var plottyGenerator = new PlottyCodeGenerator();

            var generationResult = plottyGenerator.Generate(codes.ToList());

            var plottyAssemblyVisitor = new AssemblyGeneratingVisitor();

            foreach (var line in generationResult.Code)
            {
                line.Accept(plottyAssemblyVisitor);
            }

            return new CompilationResult(generationResult, plottyAssemblyVisitor.Lines);
        }      
    }
}