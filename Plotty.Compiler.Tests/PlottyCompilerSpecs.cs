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
        [Fact(Skip = "No funciona")]
        public void Add()
        {
            var source = "void main() {\r\n\tc=add(55);\r\n\treturn;\r\n}\r\n\r\nint add(int a) \r\n{\r\n\treturn a+1;\r\n}";

            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.GetReferenceValue("a").Should().Be(3);
            fixture.GetReferenceValue("b").Should().Be(5);
        }

        [Fact]
        public void Assignments()
        {
            var source = "void main() { a = 123; b=80; }";          

            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.GetReferenceValue("a").Should().Be(123);
            fixture.GetReferenceValue("b").Should().Be(80);
        }

        [Fact]
        public void NestedCallsToReturn()
        {
            var source = "void main()  { a = func1(); }  int func1()  { return func2(); }  int func2()  { return func3(); }  int func3()  { return 1234; }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetReferenceValue("a").Should().Be(1234);
        }

        [Fact]
        public void NestedCallsCombined()
        {
            var source = "int func5() \n{\n\treturn 2;\n}\n\nint func4() \n{\n\treturn 5;\n}\n\nint func3() \n{\n\treturn 3+4;\n}\n\n\nint func2() \n{\n\treturn func3() + func4() + func5();\n}\n\n\nint func1() \n{\n\treturn 1;\n}\n\n\nint main() \n{\n\treturn func1() + func2();\n}\n";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(15);
        }

        [Fact]
        public void NestedCalls()
        {
            var source = "void main()  { jump1(); }  void jump1()  { jump2(); }  void jump2()  { jump3(); }  void jump3()  { jump4(); } void jump4() { jump5(); } void jump5() { }";

            var fixture = new MachineFixture();
            fixture.Run(source);
        }

        [Fact]
        public void Simple()
        {
            var source = "int main() { return simple(); } int simple() { return 85 + 5; }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(90);
        }

        [Fact]
        public void SimpleWithAddition()
        {
            var source = "void main() { return simple() + 5; } int simple() { return 85; }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(90);
        }

        [Fact]
        public void Simpler()
        {
            var source = "int main() { return 85; }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(85);
        }

        [Fact]
        public void TwoFuncsAdded()
        {
            var source = "int main() { return first() + second(); } int first() { return 3; } int second() { return 5; } }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(8);
        }

        [Fact]
        public void FuncWithParams()
        {
            var source = "int main() { return add(123); } int add(int a) { return a+555; }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(678);
        }

        [Fact]
        public void AddWithParams()
        {
            var source = "int main() { return add(3, 4); } int add(int a, int b) { return a+b; }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(7);
        }

        [Fact]
        public void TwoReturns()
        {
            var source = "int main() \r\n{ \r\n\ta=3; \r\n\r\n\tif (a==3) \r\n\t{\r\n\t\treturn 5; \r\n\t}\r\n\telse \r\n\t{\r\n\t\treturn 2; \r\n\t}\r\n}";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(5);
        }

        
        [Fact]
        public void Recursive()
        {
            var source = "int main() \r\n{ \r\n\treturn rec(1);\r\n}\r\n\r\nint rec(int n)\r\n{\r\n\tif (n > 10)\r\n\t{\r\n\t\treturn 0;\r\n\t}\r\n\r\n\treturn n+rec(n+1);\r\n}";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(55);
        }

        [Fact]
        public void FibonacciRecursive()
        {
            var source = "int main() \r\n{\r\n\treturn fib(10);\r\n}\r\n\r\nint fib(int n)\r\n{\r\n  if (n == 0)\r\n    return n;\r\n\r\n  if (n == 1)\r\n    return n;\r\n\r\n    return (fib(n-1) + fib(n-2));\r\n}";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(55);
        }


        private class MachineFixture
        {
            private Scope mainScope;
            public List<IntermediateCode> IntermediateCode { get; private set; }
            public int ReturnedValue => Machine.Registers[PlottyCodeGenerationVisitor.ReturnRegisterIndex];
            public int GetReferenceValue(Reference r)
            {
                var address = mainScope.Symbols.Keys.ToList().IndexOf(r);
                if (address == -1)
                {
                    throw new InvalidOperationException($"The referece {r} doesn't exist in the 'main' scope");
                }

                return Machine.Memory[address];
            }

            public MachineFixture()
            {
                Machine = new PlottyMachine();
            }

            public void Run(string source)
            {
                var compiler = new PlottyCompiler();
                var result = compiler.Compile(source);
                mainScope = result.Scope.Children.Single(x => x.Owner is Function f && f.Name == "main");
                IntermediateCode = result.IntermediateCode;
                
                Machine.Load(result.GenerationResult.Lines);

                while (Machine.CanExecute)
                {
                    Machine.Execute();
                }
            }

            public PlottyMachine Machine { get; }
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

            var source = $"void main()\n {{int first;int second;int next;int c; int n; n = {n};\nfirst = 0;\nsecond = 1;\n \nfor (c = 0; c<n ;c=c+1)\n{{\n\tif ( c < 2 )\n\t{{\n    \tnext = c;\n\t}}\n\n    if ( c > 1)\n    {{\n\t\tnext = first + second;\n        first = second;\n\t\tsecond = next;\n\t}}      \n}}\n}}";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.GetReferenceValue("next").Should().Be(result);
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
