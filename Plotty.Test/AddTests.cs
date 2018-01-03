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

        [Theory]
        [InlineData("ADD R1,#1,R2\nLOAD R1,#1")]
        [InlineData("LOAD R1,#1\nADD R1,#1,R2\nBRANCH R0,R1,label")]
        public void Line(string source)
        {
            var tokenList = new Tokenizer().Tokenize(source);
            var commands = Parser.Line.Parse(tokenList);
        } 
    }
}