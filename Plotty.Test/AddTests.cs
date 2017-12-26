using Superpower;
using Xunit;

namespace Plotty.Test
{
    public class AddTests
    {          
        [Theory]
        //[InlineData("ADD R1,#1,R2")]
        [InlineData("ADD R1,#1")]
        //[InlineData("ADD R1,R2")]
        //[InlineData("ADD R1,R2,R3")]
        public void Add(string source)
        {
            var tokenList = new Tokenizer().Tokenize(source);
            var commands = Parser.Add.Parse(tokenList);
        } 
    }
}