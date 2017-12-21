using System.Collections.Generic;
using System.Linq;
using Superpower.Model;
using Xunit;

namespace Plotty.Test
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("-", PlottyToken.Hyphen)]
        [InlineData(",", PlottyToken.Comma)]
        [InlineData("@", PlottyToken.At)]
        [InlineData("123", PlottyToken.Number)]
        [InlineData("PLOT", PlottyToken.Plot)]
        [InlineData("MOVE", PlottyToken.Move)]
        public void Single(string ch, PlottyToken tk)
        {
            var textSpan = new TextSpan(ch, new Position(0, 1, 1), ch.Length + 1);

            var sut = new Tokenizer();
            var actual = sut.Tokenize(ch).ToList();

            var expected = new List<Token<PlottyToken>> {new Token<PlottyToken>(tk, textSpan)};
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("PLOT 123", new [] {PlottyToken.Plot, PlottyToken.Whitespace, PlottyToken.Number})]
        [InlineData("PLOT 123 123", new [] {PlottyToken.Plot, PlottyToken.Whitespace, PlottyToken.Number, PlottyToken.Whitespace, PlottyToken.Number})]
        [InlineData("1,2", new [] {PlottyToken.Number, PlottyToken.Comma, PlottyToken.Number})]
        [InlineData("MOVE 1,2", new [] {PlottyToken.Move, PlottyToken.Whitespace, PlottyToken.Number, PlottyToken.Comma, PlottyToken.Number})]
        [InlineData("1", new [] {PlottyToken.Number})]
        [InlineData("MOVE", new [] {PlottyToken.Move})]
        [InlineData("MOVE ", new [] {PlottyToken.Move, PlottyToken.Whitespace})]
        public void TestList(string ch, PlottyToken[] tk)
        {
            var sut = new Tokenizer();
            var actual = sut.Tokenize(ch)
                .Select(t => t.Kind)
                .ToList();

            Assert.Equal(tk, actual);
        }
    }
}
