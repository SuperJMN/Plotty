using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace Plotty.Parser
{
    public static class TokenizerFactory
    {
        public static Tokenizer<AsmToken> Create()
        {

            var tokenizerBuilder = new TokenizerBuilder<AsmToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Span.EqualTo("MOVE"), AsmToken.Move, true)
                .Match(Span.EqualTo("LOAD"), AsmToken.Load, true)
                .Match(Span.EqualTo("STORE"), AsmToken.Store, true)
                .Match(Span.EqualTo("ADD"), AsmToken.Add, true)
                .Match(Span.EqualTo("SUB"), AsmToken.Subtract, true)
                .Match(Span.EqualTo("MULT"), AsmToken.Multiply, true)
                .Match(Span.EqualTo("BEQ"), AsmToken.BranchEqual, true)
                .Match(Span.EqualTo("BLT"), AsmToken.BranchLessThan, true)
                .Match(Span.EqualTo("BLE"), AsmToken.BranchLessThanOrEqualTo, true)
                .Match(Character.EqualTo(':'), AsmToken.Colon)
                .Match(Character.EqualTo('+'), AsmToken.Plus)
                .Match(Character.EqualTo('#'), AsmToken.Hash)
                .Match(Character.EqualTo(','), AsmToken.Comma)
                .Match(Character.EqualTo('\n'), AsmToken.NewLine)
                .Match(Span.Regex(@"\d*"), AsmToken.Number)
                .Match(Span.Regex(@"R\d*"), AsmToken.Register, true)
                .Match(Span.Regex(@"\w[\w\d]*"), AsmToken.Text, true);
            

            return tokenizerBuilder.Build();
        }
    } 
}
