using System.Linq;
using Plotty.Parser;
using Xunit;

namespace Plotty.Test
{
    public class TokenizerTests
    {
        [Theory]
        [InlineData("LOAD 123", new [] {AsmToken.Load, AsmToken.Number})]
        [InlineData("STORE 123 123", new [] {AsmToken.Store, AsmToken.Number, AsmToken.Number})]
        [InlineData("1,2", new [] {AsmToken.Number, AsmToken.Comma, AsmToken.Number})]
        [InlineData("ADD 1,2", new [] {AsmToken.Add, AsmToken.Number, AsmToken.Comma, AsmToken.Number})]
        [InlineData("1", new [] {AsmToken.Number})]
        [InlineData("STORE", new [] {AsmToken.Store})]
        [InlineData("MOVE ", new [] {AsmToken.Move, })]
        [InlineData("BLT ", new [] {AsmToken.BranchLessThan, })]
        [InlineData("BLE ", new [] {AsmToken.BranchLessThanOrEqualTo, })]
        [InlineData("BEQ ", new [] {AsmToken.BranchEqual, })]
        public void TestList(string ch, AsmToken[] tk)
        {
            var sut = TokenizerFactory.Create();
            var actual = sut.Tokenize(ch)
                .Select(t => t.Kind)
                .ToList();

            Assert.Equal(tk, actual);
        }
    }
}
