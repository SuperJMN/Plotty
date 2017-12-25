using System.Collections.Generic;
using System.Linq;
using Superpower.Model;
using Xunit;

namespace Plotty.Test
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData(",", AsmToken.Comma)]
        [InlineData("R", AsmToken.Register)]
        public void Single(string ch, AsmToken tk)
        {
            var textSpan = new TextSpan(ch, new Position(0, 1, 1), ch.Length + 1);

            var sut = new Tokenizer();
            var actual = sut.Tokenize(ch).ToList();

            var expected = new List<Token<AsmToken>> {new Token<AsmToken>(tk, textSpan)};
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("LOAD 123", new [] {AsmToken.Load, AsmToken.Whitespace, AsmToken.Number})]
        [InlineData("STORE 123 123", new [] {AsmToken.Store, AsmToken.Whitespace, AsmToken.Number, AsmToken.Whitespace, AsmToken.Number})]
        [InlineData("1,2", new [] {AsmToken.Number, AsmToken.Comma, AsmToken.Number})]
        [InlineData("ADD 1,2", new [] {AsmToken.Add, AsmToken.Whitespace, AsmToken.Number, AsmToken.Comma, AsmToken.Number})]
        [InlineData("1", new [] {AsmToken.Number})]
        [InlineData("STORE", new [] {AsmToken.Store})]
        [InlineData("LOAD ", new [] {AsmToken.Load, AsmToken.Whitespace})]
        public void TestList(string ch, AsmToken[] tk)
        {
            var sut = new Tokenizer();
            var actual = sut.Tokenize(ch)
                .Select(t => t.Kind)
                .ToList();

            Assert.Equal(tk, actual);
        }
    }
}
