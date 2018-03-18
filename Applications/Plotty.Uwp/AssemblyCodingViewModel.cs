using Plotty.Model;
using Superpower;

namespace Plotty.Uwp
{
    public class AssemblyCodingViewModel : CodingViewModelBase
    {
        protected override string DefaultSourceCode =>
            "\t\tMOVE\tR0,#3\r\n\t\tMOVE\tR1,#4\r\nstart: \tBRANCH\tR0,R2,end\r\n\t\tADD\tR3,R1\r\n\t\tADD\tR2,#1\r\n\t\tBRANCH\tR4,R4,start\r\nend:\tHALT";

        protected override Line[] GeneratePlottyInstructions(string source)
        {
            var tokenList = new Parser.Tokenizer().Tokenize(source);
            return Parser.Parser.AsmParser.Parse(tokenList);
        }

        public override string Name => "Assembly";
    }
}