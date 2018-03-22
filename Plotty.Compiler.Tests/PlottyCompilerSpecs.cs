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
        public void GivenSourceVariablesHaveTheExpectedValues(string source, IEnumerable<Expectation> expectations)
        {
            var result = new PlottyCompiler().Compile(source);
            AssertRun(result.GenerationResult, expectations);
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
