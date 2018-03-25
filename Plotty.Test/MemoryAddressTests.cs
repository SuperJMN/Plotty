using Plotty.Parser;
using Superpower;
using Xunit;

namespace Plotty.Test
{
    public class MemoryAddressTests
    {
        [Theory]
        [InlineData("R0,R1")]
        [InlineData("R0,R1,R2")]
        [InlineData("R0,R1,#1")]
        public void Add(string source)
        {
            var tokenList = TokenizerFactory.Create().Tokenize(source);
            var address = Parser.Parser.MemoryAddress.Parse(tokenList);
        }
    }
}