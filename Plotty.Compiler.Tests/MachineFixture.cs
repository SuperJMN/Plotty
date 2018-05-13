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
        public int GetValue(Reference r, int index = 0)
        {
            return r.GetValue(mainSymbolTable, Machine.Memory, index);
        }

        public MachineFixture()
        {
            Machine = new PlottyMachine();
        }

        public PlottyMachine Machine { get; }

        public int[] GetArray(Reference r, int lenght)
        {
            return r.GetArray(mainSymbolTable, Machine.Memory, lenght);
        }

        public void Run(string source, Action<ExecutionCore> action = null)
        {
            var compiler = new PlottyCompiler();
            var result = compiler.Compile(source);
            mainSymbolTable = result.SymbolTable.Children.Single(x => x.Owner is Function f && f.Name == "main");
            IntermediateCode = result.IntermediateCode;
                
            Machine.Load(result.GenerationResult.Lines);

            action?.Invoke(new ExecutionCore(mainSymbolTable, Machine.Registers, Machine.Memory));

            while (Machine.CanExecute)
            {
                Machine.Execute();
            }
        }
    }
}