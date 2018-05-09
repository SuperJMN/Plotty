using System;
using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using Plotty.CodeGeneration;
using Plotty.VirtualMachine;

namespace Plotty.Compiler.Tests
{
    internal class MachineFixture
    {
        private SymbolTable mainSymbolTable;
        public List<IntermediateCode> IntermediateCode { get; private set; }
        public int ReturnedValue => Machine.Registers[PlottyCodeGenerationVisitor.ReturnRegisterIndex];
        public int GetReferenceValue(Reference r)
        {
            if (mainSymbolTable.Symbols.TryGetValue(r, out var props))
            {
                return Machine.Memory[props.Offset];
            }
           
            throw new InvalidOperationException($"The referece {r} doesn't exist in the 'main' symbolTable");           
        }

        public MachineFixture()
        {
            Machine = new PlottyMachine();
        }

        public void Run(string source)
        {
            var compiler = new PlottyCompiler();
            var result = compiler.Compile(source);
            mainSymbolTable = result.SymbolTable.Children.Single(x => x.Owner is Function f && f.Name == "main");
            IntermediateCode = result.IntermediateCode;
                
            Machine.Load(result.GenerationResult.Lines);

            while (Machine.CanExecute)
            {
                Machine.Execute();
            }
        }

        public PlottyMachine Machine { get; }

        public int[] GetArray(Reference r, int lenght)
        {
            if (mainSymbolTable.Symbols.TryGetValue(r, out var props))
            {
                return Machine.Memory.Skip(props.Offset).Take(lenght).ToArray();
            }

            throw new InvalidOperationException($"The referece {r} doesn't exist in the 'main' symbolTable");
        }
    }
}