using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Plotty.Compiler.Tests
{
    public class IntermediateGeneratorSpecs
    {
        [Fact]
        public void Test()
        {
            var ast = new AssignmentExpression(new RefExpresion("a"),
                new AddExpression(new RefExpresion("b"),
                    new MultExpression(new RefExpresion("c"), new RefExpresion("d"))));

            var expected = new List<Expression>
            {
                new AssignmentExpression(new RefExpresion("t1"), new MultExpression(new RefExpresion("c"), new RefExpresion("d"))),
                new AssignmentExpression(new RefExpresion("t2"), new AddExpression(new RefExpresion("b"), new RefExpresion("t1"))),
                new AssignmentExpression(new RefExpresion("a"), new RefExpresion("t2")),
            };

            var sut = new IntermediateGenerator();
            var actual = sut.Generate(ast);

            actual.Should().BeEquivalentTo(expected);
        }
    }
}
