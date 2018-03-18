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
        public GenerationResult Compile(string source)
        {
            var tokens = TokenizerFactory.Create().Tokenize(source);
            var parsed = Statements.ProgramParser.Parse(tokens);

            var generator = new IntermediateCodeGenerator();
            var codes = generator.Generate(parsed);
            var plottyGenerator = new PlottyCodeGenerator();
            
            return plottyGenerator.Generate(codes.ToList());
        }      
    } 
}