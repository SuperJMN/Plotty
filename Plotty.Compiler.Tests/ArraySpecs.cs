using FluentAssertions;
using Xunit;

namespace Plotty.Compiler.Tests
{
    public class ArraySpecs
    {
        [Fact]
        public void FirstElement()
        {
            var source = "int main()  { int array[1]; array[0] = 123; return array[0]; }";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(123);
        }

        [Fact]
        public void SecondElement()
        {
            var source = "void main()  { int array[2]; array[1] = 123; return array[1]; }";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(123);
        }

        [Fact]
        public void AssignAndSet()
        {
            var source = "int main()  { int array[2], a; array[1] = 123; a=array[1]; return a; }";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(123);
        }

        [Fact]
        public void AdditionOfTwoItems()
        {
            var source = "int main()  { int array[2]; array[0] = 3; array[1] = 5; return array[0]+array[1]; }";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(8);
        }

        [Fact]
        public void DoubleAssignment()
        {
            var source = "int main()  { int array[2], a, b; array[0] = 3; array[1] = 5; a=array[0]; b=array[1]; return a+b; }";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.GetReferenceValue("a").Should().Be(3);
            fixture.GetReferenceValue("b").Should().Be(5);
        }

        [Fact]
        public void FunctionWithLocalArray()
        {
            var source = "int main()  { int a=5, b=6; return Add(a, b); } int Add(int a, int b) { int array[2]; array[0]=a; array[1]=b; return array[0] + array[1]; }";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(11);
        }

        
        [Fact]
        public void SumShort()
        {
            var source = "int main()\n{\n    int a;\n    int b=0;\n    int array[3];\n    \n    array[0] = 1;\n    array[1] = 2;\n    array[2] = 3;\n    \n    for (a=0; a<3; a=a+1) \n    {\n        b = b + array[a];\n    }\n    \n    return b;\n}\n";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(6);
        }

        [Fact]
        public void SumLongBasic()
        {
            var source = "int main()\n{\n    int a;\n    int b=0;\n    int array[10];\n    \n    array[0] = 1;\n    array[1] = 1;\n    array[2] = 1;\n    array[3] = 1;\n    array[4] = 1;\n    array[5] = 1;\n    array[6] = 1;\n    array[7] = 1;\n    array[8] = 1;\n    array[9] = 1;\n    \n    for (a=0; a<10; a=a+1) \n    {\n        b = b + array[a];\n    }\n    \n    return b;\n}\n";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(10);
        }

        [Fact]
        public void SumLong()
        {
            var source = "int main()\n{\n    int a;\n    int b=0;\n    int array[10];\n    \n    array[0] = 1;\n    array[1] = 2;\n    array[2] = 3;\n    array[3] = 4;\n    array[4] = 5;\n    array[5] = 6;\n    array[6] = 7;\n    array[7] = 8;\n    array[8] = 9;\n    array[9] = 10;\n    \n    for (a=0; a<10; a=a+1) \n    {\n        b = b + array[a];\n    }\n    \n    return b;\n}\n";
            var fixture = new MachineFixture();
            fixture.Run(source);
            fixture.ReturnedValue.Should().Be(55);
        }
    }
}