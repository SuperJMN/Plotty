using System.Collections.Generic;
using CodeGen.Core;
using FluentAssertions;
using Plotty.CodeGeneration;
using Plotty.VirtualMachine;
using Xunit;

namespace Plotty.Compiler.Tests
{
    public class PlottyCompilerSpecs
    {
        [Theory]
        [MemberData(nameof(TestData))]
        public void ReferencesHaveTheExpectedValues(string source, IEnumerable<Expectation> expectations)
        {
            var result = new PlottyCompiler().Compile(source);
            AssertRun(result.GenerationResult, expectations);
        }

        [Fact]
        public void Fibonacci()
        {
            var n = 12;
            var result = 89;

            var code = $"n = {n};\nfirst = 0;\nsecond = 1;\n \nfor (c = 0; c<n ;c=c+1)\n{{\n\tif ( c < 2 )\n\t{{\n    \tnext = c;\n\t}}\n\n    if ( c > 1)\n    {{\n\t\tnext = first + second;\n        first = second;\n\t\tsecond = next;\n\t}}      \n}}\n";
            AssertRun(code, new[] { new Expectation("next", result), });
        }

        [Theory]
        [InlineData(5, 3, Operator.NotEqual)]
        [InlineData(3, 3, Operator.NotEqual)]
        [InlineData(1, 2, Operator.Equal)]
        [InlineData(0, 0, Operator.NotEqual)]
        public void LessThan(int a, int b, Operator op)
        {
            var code = $"a={a}; b={b};c = a < b;";
            AssertRun(code, new[] { new Expectation("c", 0, op), });
        }

        [Theory]
        [InlineData(5, 3, Operator.Equal)]
        [InlineData(3, 3, Operator.NotEqual)]
        [InlineData(1, 2, Operator.NotEqual)]
        [InlineData(0, 0, Operator.NotEqual)]
        public void GreaterThan(int a, int b, Operator op)
        {
            var code = $"a={a}; b={b};c = a > b;";
            AssertRun(code, new[] { new Expectation("c", 0, op), });
        }

        private void AssertRun(string source, Expectation[] expectations)
        {
            var result = new PlottyCompiler()
                .Compile(source)
                .GenerationResult;

            var machine = new PlottyMachine();
            machine.Load(result.Lines);

            while (machine.CanExecute)
            {
                machine.Execute();
            }

            foreach (var expectation in expectations)
            {
                var address = result.AddressMap[new Reference(expectation.RefName)];

                if (expectation.Operator == Operator.Equal)
                {
                    machine.Memory[address].Should().Be(expectation.Value);
                }
                else
                {
                    machine.Memory[address].Should().NotBe(expectation.Value);
                }
            }
        }

        public static IEnumerable<object[]> TestData => new List<object[]>()
        {
            new object[] {"{a=123;}", new[]  {new Expectation("a", 123)}},
            new object[] {"{a=1;b=2;}", new[]  {new Expectation("a", 1), new Expectation("b", 2)}},
            new object[] {"{a=1;b=2;b=a;}",new[] {new Expectation("a", 1), new Expectation("b", 1)}},
            new object[] {"a=1+2;}",  new[]  {new Expectation("a", 3)}},
            new object[] {"a=6-2;}", new[]  {new Expectation("a", 4)}},
            new object[] {"a=0==1;}",  new[]  {new Expectation("a", 0, Operator.NotEqual)}},
            new object[] {"a=1==1;}",  new[]  {new Expectation("a", 0)}},
            new object[] {"if (a==1) b=123;}",  new[]  {new Expectation("b", 123, Operator.NotEqual)}},
            new object[] {"if (a==0) b=123;}",  new[]  {new Expectation("b", 123)}},
            new object[] {"if (true) b=123;",  new[]  {new Expectation("b", 123)}},
            new object[] {"if (false) b=123;",  new[]  {new Expectation("b", 123, Operator.NotEqual)}},
        };
        
        private static void AssertRun(GenerationResult result, IEnumerable<Expectation> expectations)
        {
            var machine = new PlottyMachine();
            machine.Load(result.Lines);

            while (machine.CanExecute)
            {
                machine.Execute();
            }

            foreach (var expectation in expectations)
            {
                var address = result.AddressMap[new Reference(expectation.RefName)];

                if (expectation.Operator == Operator.Equal)
                {
                    machine.Memory[address].Should().Be(expectation.Value);
                }
                else
                {
                    machine.Memory[address].Should().NotBe(expectation.Value);
                }
            }
        }

        public class Expectation
        {
            public Expectation(string refName, int value, Operator @operator = Operator.Equal)
            {
                RefName = refName;
                Value = value;
                Operator = @operator;
            }

            public string RefName { get; }
            public int Value { get; }
            public Operator Operator { get; }
        }

        public enum Operator
        {
            Equal,
            NotEqual,
        }
    }    
}
