using System;
using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using FluentAssertions;
using Plotty.CodeGeneration;
using Plotty.VirtualMachine;
using Xunit;

namespace Plotty.Compiler.Tests
{
    public class PlottyCompilerSpecs
    {
        [Fact]
        public void Add()
        {
            var source = "void main() {\r\n\tc=add(55);\r\n\treturn;\r\n}\r\n\r\nint add(int a) \r\n{\r\n\treturn a+1;\r\n}";

            var expectations = new List<Expectation>()
            {
                new Expectation("a", 3),
                new Expectation("b", 5),
            };

            AssertRunFull(source, expectations);
        }

        [Fact]
        public void Assignments()
        {
            var source = "void main() { a = 123; b=80; }";

            var expectations = new List<Expectation>()
            {
                new Expectation("a", 123),
                new Expectation("b", 80),
            };

            AssertRunFull(source, expectations);
        }

        [Fact]
        public void NestedCallsToReturn()
        {
            var source = "void main()  { a=func1(); }  int func1()  { return func2(); }  int func2()  { return func3(); }  int func3()  { return 1234; }";

            var expectations = new List<Expectation>()
            {
                new Expectation("a", 1234),
            };

            AssertRunFull(source, expectations);
        }

        [Fact]
        public void NestedCalls()
        {
            var source = "void main()  { jump1(); }  void jump1()  { jump2(); }  void jump2()  { jump3(); }  void jump3()  { jump4(); } void jump4() { jump5(); } void jump5() { }";

            var expectations = new List<Expectation>()
            {
                new Expectation("a", 1234),
            };

            AssertRunFull(source, expectations);
        }

        [Fact]
        public void Simple()
        {
            var source = "void main() { a = simple(); } int simple() { return 85; }";

            //var expectations = new List<Expectation>()
            //{
            //    new Expectation("a", 85),
            //};

            var fixture = new MachineFixture();
            fixture.Run(source);
        }

        [Fact]
        public void Simpler()
        {
            var source = "int main() { return 85; }";

            //var expectations = new List<Expectation>()
            //{
            //    new Expectation("a", 85),
            //};

            var fixture = new MachineFixture();
            fixture.Run(source);
        }

        private class MachineFixture
        {
            public MachineFixture()
            {
                Machine = new PlottyMachine();
            }

            public void Run(string source)
            {
                var compiler = new PlottyCompiler();
                var result = compiler.Compile(source);
                
                Machine.Load(result.GenerationResult.Lines);

                while (Machine.CanExecute)
                {
                    Machine.Execute();
                }
            }

            public PlottyMachine Machine { get; }
        }

        private void AssertRunFull(string source, IEnumerable<Expectation> expectations)
        {
            var sut = new PlottyCompiler();
            var result = sut.Compile(source);

            var machine = new PlottyMachine();

            machine.Load(result.GenerationResult.Lines);

            while (machine.CanExecute)
            {
                machine.Execute();
            }

            foreach (var expectation in expectations)
            {
                var resultScope = result.Scope.Children.Single(s => s.Owner is Function function && function.Name == "main");

                var address = resultScope.Symbols.Keys.ToList().IndexOf(expectation.Reference);
                var value = machine.Memory[address];
                if (expectation.Operator == Operator.Equal)
                {
                    value.Should().Be(expectation.Value);
                }
                else
                {
                    value.Should().NotBe(expectation.Value);
                }
            }
        }

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

            var code = $"void main()\n {{int first;int second;int next;int c; int n; n = {n};\nfirst = 0;\nsecond = 1;\n \nfor (c = 0; c<n ;c=c+1)\n{{\n\tif ( c < 2 )\n\t{{\n    \tnext = c;\n\t}}\n\n    if ( c > 1)\n    {{\n\t\tnext = first + second;\n        first = second;\n\t\tsecond = next;\n\t}}      \n}}\n}}";
            AssertRun(code, new[] { new Expectation("next", result), });
        }

        [Theory]
        [InlineData(5, 3, Operator.NotEqual)]
        [InlineData(3, 3, Operator.NotEqual)]
        [InlineData(1, 2, Operator.Equal)]
        [InlineData(0, 0, Operator.NotEqual)]
        public void LessThan(int a, int b, Operator op)
        {
            var code = WrapInsideMain($"a={a}; b={b};c = a < b;");
            AssertRun(code, new[] { new Expectation("c", 0, op), });
        }

        [Theory]
        [InlineData(5, 3, Operator.Equal)]
        [InlineData(3, 3, Operator.NotEqual)]
        [InlineData(1, 2, Operator.NotEqual)]
        [InlineData(0, 0, Operator.NotEqual)]
        public void GreaterThan(int a, int b, Operator op)
        {
            var code = WrapInsideMain($"int a, b, c; a={a}; b={b};c = a > b;");
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

            //foreach (var expectation in expectations)
            //{
            //    var address = result.AddressMap[new Reference(expectation.Reference)];

            //    if (expectation.Operator == Operator.Equal)
            //    {
            //        machine.Memory[address].Should().Be(expectation.Value);
            //    }
            //    else
            //    {
            //        machine.Memory[address].Should().NotBe(expectation.Value);
            //    }
            //}
        }

        public static IEnumerable<object[]> TestData => new List<object[]>()
        {
            new object[] {WrapInsideMain("{a=123;}"), new[]  {new Expectation("a", 123)}},
            new object[] {WrapInsideMain("{a=1;b=2;}"), new[]  {new Expectation("a", 1), new Expectation("b", 2)}},
            new object[] {WrapInsideMain("{a=1;b=2;b=a;}"),new[] {new Expectation("a", 1), new Expectation("b", 1)}},
            new object[] {WrapInsideMain("a=1+2;}"),  new[]  {new Expectation("a", 3)}},
            new object[] {WrapInsideMain("a=6-2;}"), new[]  {new Expectation("a", 4)}},
            new object[] {WrapInsideMain("a=0==1;}"),  new[]  {new Expectation("a", 0, Operator.NotEqual)}},
            new object[] {WrapInsideMain("a=1==1;}"),  new[]  {new Expectation("a", 0)}},
            new object[] {WrapInsideMain("if (a==1) b=123;}"),  new[]  {new Expectation("b", 123, Operator.NotEqual)}},
            new object[] {WrapInsideMain("if (a==0) b=123;}"),  new[]  {new Expectation("b", 123)}},
            new object[] {WrapInsideMain("if (true) b=123;"),  new[]  {new Expectation("b", 123)}},
            new object[] {WrapInsideMain("if (false) b=123;"),  new[]  {new Expectation("b", 123, Operator.NotEqual)}},
        };

        private static string WrapInsideMain(string s)
        {
            return "void main() { int a, b, c;" + s + "}";
        }

        private static void AssertRun(GenerationResult result, IEnumerable<Expectation> expectations)
        {
            var machine = new PlottyMachine();
            machine.Load(result.Lines);

            while (machine.CanExecute)
            {
                machine.Execute();
            }

            //foreach (var expectation in expectations)
            //{
            //    var address = result.AddressMap[new Reference(expectation.Reference)];

            //    if (expectation.Operator == Operator.Equal)
            //    {
            //        machine.Memory[address].Should().Be(expectation.Value);
            //    }
            //    else
            //    {
            //        machine.Memory[address].Should().NotBe(expectation.Value);
            //    }
            //}
        }

        public class Expectation
        {
            public Expectation(Reference reference, int value, Operator @operator = Operator.Equal)
            {
                Reference = reference;
                Value = value;
                Operator = @operator;
            }

            public Reference Reference { get; }
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
