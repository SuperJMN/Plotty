using System.Collections.Generic;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using CodeGen.Parsing.Ast.Statements;
using FluentAssertions;
using Plotty.Model;
using Xunit;

namespace Plotty.CodeGeneration.Tests
{
    public partial class PlottyCodeGeneratorSpecs
    {
        [Fact(Skip = "Ignore")]
        public void Call()
        {
            var func = new Function("func", VariableType.Void, new List<Argument>(), new Block());

            var scope = new Scope();
            scope.AddReferences(10);
            var child = scope.CreateChildScope(func);
            child.AddReferences(5);

            var intermediateCodes = new List<IntermediateCode> { new CallCode(func.Name), new HaltCode(), new FunctionDefinitionCode(func), new ReturnCode() };

            var fixture = new MachineFixture();
            fixture.Run(intermediateCodes, scope, new Dictionary<int, int> { { 6, 10 }, { 7, 50 } }, new Dictionary<int, int>());

            fixture.Machine.Memory[60].Should().Be(6);

            fixture.Machine.Registers[6].Should().Be(10);
            fixture.Machine.Registers[7].Should().Be(50);
        }

        [Fact(Skip = "Ignore")]
        public void Return()
        {
            var scope = new Scope();

            var intermediateCodes = new List<IntermediateCode> { new ReturnCode() };

            var fixture = new MachineFixture();
            fixture.Run(intermediateCodes, scope, new Dictionary<int, int> { { 6, 10 }, { 7, 50 } },
                new Dictionary<int, int>() { { 60, 6 } });

            fixture.Machine.Memory[60].Should().Be(6);

            fixture.Machine.Registers[6].Should().Be(5);
            fixture.Machine.Registers[7].Should().Be(61);
        }
    }

    static class ScopeMixin
    {
        public static void AddReferences(this Scope child, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                child.AddReference($"Y{i}");
            }
        }
    }
}