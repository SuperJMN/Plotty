using Xunit;

namespace Plotty.Test
{
    public class ParserTests
    {
        [Fact]
        public void LoadTest()
        {
            var tokenList = new Tokenizer().Tokenize("LOAD R3,1000\nADD R1,R2,R3");
            var sut = Parser.AsmParser(tokenList);
        }

        [Fact]
        public void RegisterTest()
        {
            var tokenList = new Tokenizer().Tokenize("R3");
            var sut = Parser.Register(tokenList);
        }
    }
}