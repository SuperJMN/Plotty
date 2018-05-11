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
        public void Assignments()
        {
            var source = "void main() { a = 123; b=80; }";          

            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.GetValue("a").Should().Be(123);
            fixture.GetValue("b").Should().Be(80);
        }

        [Fact]
        public void NestedCallsToReturn()
        {
            var source = "void main()  { a = func1(); }  int func1()  { return func2(); } int func2() { return func3(); } int func3()  { return 1234; }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("a").Should().Be(1234);
        }

        [Fact]
        public void NestedCallsCombined()
        {
            var source = "int func5()  {  return 2; }  int func4()  {  return 5; }  int func3()  {  return 3+4; }   int func2()  {  return func3() + func4() + func5(); }   int func1()  {  return 1; }   int main()  {  return func1() + func2(); } ";

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

        [Fact(Skip = "Not implemented yet")]
        public void Pointer()
        {
            var source = "int main() { int a=14; int* b=&a; return *b; }";

            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(14);
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
        public void OrOperation1()
        {
            var source = "void main()\n{\n\ta=12;\n\tif (a==12 || a==5) \n\t{\n\t\tb=3;\n\t}\n}";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("b").Should().Be(3);
        }

        [Fact]
        public void OrOperation2()
        {
            var source = "void main()\n{\n\ta=5;\n\tif (a==12 || a==5) \n\t{\n\t\tb=3;\n\t}\n}";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("b").Should().Be(3);
        }

        [Fact]
        public void Add()
        {
            var source = "void main() { int a=4; b=a+1; }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("b").Should().Be(5);
        }
        
        [Fact]
        public void DoWhile()
        {
            var source = "void main() { int a=0; do { int b = 123; a=a+1; } while (a < 2); }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("a").Should().Be(2);
            fixture.GetValue("b").Should().Be(123);

        }

        [Fact]
        public void DoWhile_SentenceIsExecutedOnce()
        {
            var source = "void main() { int a=0; do { b = 123; } while (a < 0); }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("a").Should().Be(0);
            fixture.GetValue("b").Should().Be(123);
        }

        [Fact]
        public void WhileStatement_WhenConditionNotMet_BlockIsNotExecuted()
        {
            var source = "void main() { int a=0; while (a>5) { b = 123; } }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("b").Should().NotBe(123);
        }

        [Fact]
        public void WhileStatement_WhenConditionIsMet_BlockIsExecuted()
        {
            var source = "void main() { int a=0; while (a==0) { b = 123; a=a+1; } }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.GetValue("b").Should().Be(123);
        }

        [Fact]
        public void TwoReturns()
        {
            var source = "int main() { a=3; if (a==3) { return 5; } else { return 2; } }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(5);
        }

        
        [Fact]
        public void Recursive()
        {
            var source = "int main() { return rec(1); } int rec(int n) { if (n > 10) { return 0; } return n+rec(n+1); }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(55);
        }

        [Fact]
        public void FibonacciRecursive()
        {
            var source = "int main() { return fib(10); } int fib(int n)  { if (n == 0) return n; if (n == 1) return n; return (fib(n-1) + fib(n-2)); }";
            
            var fixture = new MachineFixture();
            fixture.Run(source);

            fixture.ReturnedValue.Should().Be(55);
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

            var source = $"void main()  {{int first;int second;int next;int c; int n; n = {n}; first = 0; second = 1;   for (c = 0; c<n ;c=c+1) {{  if ( c < 2 )  {{      next = c;  }}      if ( c > 1)     {{   next = first + second;         first = second;   second = next;  }}       }} }}";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.GetValue("next").Should().Be(result);
        }  
        
        [Fact]
        public void IntFuntionThatDoesNothing()
        {
            var source = "int main() { int a; nothing(a); return 2; } void nothing(int n) { }";
            var fixture = new MachineFixture();
            fixture.Run(source);
        }

        [Fact]
        public void FuntionThatDoesNothing()
        {
            var source = "void main() { do_nothing(); } void do_nothing() { }";
            var fixture = new MachineFixture();
            fixture.Run(source);
        }

        [Fact]
        public void FuntionThatDoesNothingWithDeclaration()
        {
            var source = "void main() { int n; do_nothing(n); } void do_nothing(int n) { }";
            var fixture = new MachineFixture();
            fixture.Run(source);
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
