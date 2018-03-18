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
        public void GivenSourceVariablesHaveTheExpectedValues(string source, IDictionary<string, int> expectations)
        {
            var result = new PlottyCompiler().Compile(source);
            AssertRun(result, expectations);
        }

        public static IEnumerable<object[]> TestData => new List<object[]>()
        {
            new object[] {"{a=123;}", new Dictionary<string, int> {{"a", 123}}},
            new object[] {"{a=1; b=2;}", new Dictionary<string, int> {{"a", 1}, {"b", 2}}},
            new object[] {"{a=1; b=2; b=a; }", new Dictionary<string, int> {{"a", 1}, {"b", 1}}},
        };

        private static void AssertRun(GenerationResult result, IDictionary<string, int> dictionary)
        {
            var machine = new PlottyMachine();
            machine.Load(result.Code);

            while (machine.CanExecute)
            {
                machine.Execute();
            }

            foreach (var expectation in dictionary)
            {
                var address = result.AddressMap[new Reference(expectation.Key)];
                machine.Memory[address].Should().Be(expectation.Value);
            }
        }
    }
}
