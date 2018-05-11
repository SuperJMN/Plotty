using CodeGen.Parsing.Ast;

namespace Plotty.Compiler.Tests
{
    internal class ExecutionCore
    {
        public SymbolTable MainSymbolTable { get; }
        public int[] MachineRegisters { get; }
        public int[] MachineMemory { get; }

        public ExecutionCore(SymbolTable mainSymbolTable, int[] machineRegisters, int[] machineMemory)
        {
            MainSymbolTable = mainSymbolTable;
            MachineRegisters = machineRegisters;
            MachineMemory = machineMemory;
        }
    }
}