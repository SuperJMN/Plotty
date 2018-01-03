using Superpower;
using Xunit;

namespace Plotty.Test
{
    public class MemoryAddressTests
    {
        [Theory]
        [InlineData("1")]
        [InlineData("1+1")]
        public void Add(string source)
        {
            var tokenList = new Tokenizer().Tokenize(source);
            var address = Parser.MemoryAddress.Parse(tokenList);
        }
    }
}