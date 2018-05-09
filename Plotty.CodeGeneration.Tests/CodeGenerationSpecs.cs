using System.Collections.Generic;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using FluentAssertions;
using Plotty.Model;
using Plotty.VirtualMachine;
using Xunit;

namespace Plotty.CodeGeneration.Tests
{
    public class CodeGenerationSpecs
    {
        [Theory]
        [InlineData(0, 3)]
        [InlineData(1, 4)]
        [InlineData(2, 5)]
        [InlineData(3, 6)]
        [InlineData(4, 7)]
        public void FromIndexed(int index, int expected)
        {
            var sut = new PlottyCodeGenerator();
            var symbolTable = new SymbolTable(new FakeUnit(), null);
            symbolTable.Annotate("a", PrimitiveType.Int);
            symbolTable.Annotate("index", PrimitiveType.Int);
            symbolTable.Annotate("b", PrimitiveType.Int);

            var generationResult = sut.Generate(new List<IntermediateCode> { new LoadFromArray("a", "b", "index") },
                symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Memory[0] = 1; // a
            machine.Memory[1] = index; // index
            machine.Memory[2] = 3; // b[0]
            machine.Memory[3] = 4; // b[1]
            machine.Memory[4] = 5; // b[2]
            machine.Memory[5] = 6; // b[3]
            machine.Memory[6] = 7; // b[4]

            machine.ExecuteUntilHalt();

            machine.Memory[symbolTable.Symbols["a"].Offset].Should().Be(expected);
        }

        [Fact]
        public void ToIndexed()
        {
            var sut = new PlottyCodeGenerator();
            var symbolTable = new SymbolTable(new FakeUnit(), null);
            symbolTable.Annotate("b", PrimitiveType.Int);
            symbolTable.Annotate("index", PrimitiveType.Int);
            symbolTable.Annotate("a", PrimitiveType.Int);

            var generationResult = sut.Generate(new List<IntermediateCode> { new StoreToArray("a", "index", "b") },
                symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Memory[0] = 123; // b
            machine.Memory[1] = 2; // index
            machine.Memory[2] = 3; // a[0]
            machine.Memory[3] = 4; // a[1]
            machine.Memory[4] = 5; // a[2]
            machine.Memory[5] = 6; // a[3]
            machine.Memory[6] = 7; // a[4]

            machine.ExecuteUntilHalt();

            machine.Memory[4].Should().Be(123);
        }

        [Fact]
        public void Param()
        {
            var sut = new PlottyCodeGenerator();
            var symbolTable = new SymbolTable(new FakeUnit(), null);

            symbolTable.Annotate("a", PrimitiveType.Int);
            symbolTable.Annotate("b", PrimitiveType.Int);
            symbolTable.Annotate("c", PrimitiveType.Int);
            symbolTable.Annotate("d", PrimitiveType.Int);

            var generationResult = sut.Generate(new List<IntermediateCode> { new ParameterCode("c") }, symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Registers[6] = 0;
            machine.Registers[7] = symbolTable.Size;

            machine.Memory[0] = 1; // a
            machine.Memory[1] = 2; // b
            machine.Memory[2] = 3; // c
            machine.Memory[3] = 4; // d

            machine.ExecuteUntilHalt();
            machine.Memory[symbolTable.Size].Should().Be(3);
        }    
        
        [Fact]
        public void ArrayParam()
        {
            var sut = new PlottyCodeGenerator();
            var symbolTable = new SymbolTable(new FakeUnit(), null);

            symbolTable.Annotate("a", PrimitiveType.Int);
            symbolTable.Annotate("b", PrimitiveType.Int);
            symbolTable.Annotate("array", PrimitiveType.Int, 3);
            symbolTable.Annotate("c", PrimitiveType.Int);

            var generationResult = sut.Generate(new List<IntermediateCode> { new ParameterCode("array") }, symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Registers[6] = 0;
            machine.Registers[7] = symbolTable.Size;

            machine.Memory[0] = 1; // a
            machine.Memory[1] = 5; // b
            machine.Memory[2] = 2; // array[0]
            machine.Memory[3] = 3; // array[1]
            machine.Memory[4] = 4; // array[2]
            machine.Memory[5] = 6; // c
            
            machine.ExecuteUntilHalt();

            machine.Memory[symbolTable.Size].Should().Be(2);
        }       
    }
    
    public class FakeUnit : ICodeUnit
    {
        public void Accept(ICodeUnitVisitor unitVisitor)
        {
        }
    }
}