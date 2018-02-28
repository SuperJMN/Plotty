using Plotty.Core;
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
            var tokenList = new Tokenizer().Tokenize(source);
            var address = Parser.MemoryAddress.Parse(tokenList);
        }
    }
}