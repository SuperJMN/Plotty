using System.Collections.Generic;
using CodeGen.Core;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing;
using CodeGen.Parsing.Ast;
using FluentAssertions;
using Plotty.Model;
using Plotty.VirtualMachine;
using Xunit;

namespace Plotty.CodeGeneration.Tests
{
    public class CodeGenerationSpecs
    {
        [Fact]
        public void AddressOf()
        {
            var builder = new SymbolTableBuilder();
            builder.AddAppearance("a", ReturnType.Int);
            builder.AddAppearance("b", ReturnType.Int);
            builder.AddAppearance("c", ReturnType.Int);
            builder.AddAppearance("T1", ReturnType.Int);

            var sut = new PlottyCodeGenerator();

            var symbolTable = builder.Build();

            var result = sut.Generate(new List<IntermediateCode>() {new AddressOf("T1", "c")}, symbolTable);

            var plottyMachine = new PlottyMachine();
            
            plottyMachine.Load(result.Lines);
            plottyMachine.Registers[6] = 100;
            
            plottyMachine.ExecuteUntilHalt();

            plottyMachine.Memory[symbolTable.Symbols["T1"].Offset + 100].Should().Be(symbolTable.Symbols["c"].Offset + 100);
        }

        [Fact]
        public void ContentsOf()
        {
            var builder = new SymbolTableBuilder();
            builder.AddAppearance("a", ReturnType.Int);
            builder.AddAppearance("b", ReturnType.Int);
            builder.AddAppearance("c", ReturnType.Int);
            builder.AddAppearance("T1", ReturnType.Int);

            var sut = new PlottyCodeGenerator();

            var symbolTable = builder.Build();

            var result = sut.Generate(new List<IntermediateCode>() {new ContentOf("T1", "c")}, symbolTable);

            var plottyMachine = new PlottyMachine();
            
            plottyMachine.Load(result.Lines);

            const int baseAddr = 500;
            plottyMachine.Memory[symbolTable.Symbols["c"].Offset + baseAddr] = 500;
            const int contents = 123;
            plottyMachine.Memory[500] = contents;
            plottyMachine.Registers[6] = baseAddr;
            
            plottyMachine.ExecuteUntilHalt();

            
            plottyMachine.Memory[symbolTable.Symbols["T1"].Offset + baseAddr].Should().Be(contents);
        }

        [Fact(Skip = "No funciona")]
        public void FromIndexed()
        {
            var sut = new PlottyCodeGenerator();

            var builder = new SymbolTableBuilder();
            builder.AddAppearance("a", ReturnType.Int);
            builder.AddAppearance("index", ReturnType.Int);
            builder.AddAppearance("array", ReturnType.Int, 4);

            var symbolTable = builder.Build();
            var generationResult = sut.Generate(new List<IntermediateCode> { new LoadFromArray("a", new IndexedReference("array", "index")) }, symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Memory[0] = 1; // a
            machine.Memory[1] = 2; // index
            machine.Memory[2] = 4; // array[0]
            machine.Memory[3] = 5; // array[1]
            machine.Memory[4] = 6; // array[2]
            machine.Memory[5] = 7; // array[3]
            machine.Memory[6] = 8; // array[4]
            
            machine.ExecuteUntilHalt();

            machine.Memory[symbolTable.Symbols["a"].Offset].Should().Be(6);
        }

        [Fact(Skip = "No funciona")]
        public void ToIndexed()
        {
            var sut = new PlottyCodeGenerator();
            var symbolTableBuilder = new SymbolTableBuilder();

            symbolTableBuilder.AddAppearance("b", ReturnType.Int);
            symbolTableBuilder.AddAppearance("index", ReturnType.Int);
            symbolTableBuilder.AddAppearance("a", ReturnType.Int);

            var symbolTable = symbolTableBuilder.Build();

            var generationResult = sut.Generate(new List<IntermediateCode> { new StoreToArray(new IndexedReference("a", "index"), "b") },
                symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Memory[0] = 123; // b
            machine.Memory[1] = 2; // index
            machine.Memory[2] = 4; // a[0]
            machine.Memory[3] = 5; // a[1]
            machine.Memory[4] = 6; // a[2]
            machine.Memory[5] = 7; // a[3]
            machine.Memory[6] = 7; // a[4]

            machine.ExecuteUntilHalt();

            machine.Memory[4].Should().Be(123);
        }

        [Fact]
        public void Param()
        {
            var sut = new PlottyCodeGenerator();
            var symbolTableBuilder = new SymbolTableBuilder();

            symbolTableBuilder.AddAppearance("a", ReturnType.Int);
            symbolTableBuilder.AddAppearance("b", ReturnType.Int);
            symbolTableBuilder.AddAppearance("c", ReturnType.Int);
            symbolTableBuilder.AddAppearance("d", ReturnType.Int);

            var symbolTable = symbolTableBuilder.Build();

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
        
        [Fact(Skip = "No funciona")]
        public void ArrayParam()
        {
            var sut = new PlottyCodeGenerator();
            var symbolTableBuilder = new SymbolTableBuilder();

            symbolTableBuilder.AddAppearance("a", ReturnType.Int);
            symbolTableBuilder.AddAppearance("b", ReturnType.Int);
            symbolTableBuilder.AddAppearance("c", ReturnType.Int);
            symbolTableBuilder.AddAppearance("array", ReturnType.Int, 3);

            var symbolTable = symbolTableBuilder.Build();

            var generationResult = sut.Generate(new List<IntermediateCode> { new ParameterCode("array") }, symbolTable);

            var machine = new PlottyMachine();

            machine.Load(generationResult.Lines);

            machine.Registers[6] = 0;
            machine.Registers[7] = symbolTable.Size;

            machine.Memory[0] = 1; // a
            machine.Memory[1] = 5; // b
            machine.Memory[2] = 5; // c
            machine.Memory[3] = 1; // array[0]
            machine.Memory[4] = 2; // array[1]
            machine.Memory[5] = 3; // array[2]
            
            machine.ExecuteUntilHalt();

            machine.Memory[symbolTable.Size].Should().Be(3);
        }       
    }
}
