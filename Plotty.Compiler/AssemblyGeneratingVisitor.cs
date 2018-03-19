using System.Collections.Generic;
using Plotty.Model;

namespace Plotty.Compiler
{
    public class AssemblyGeneratingVisitor : ILineVisitor
    {
        private readonly List<string> lines = new List<string>();
        public IReadOnlyCollection<string> Lines => lines.AsReadOnly();

        public void Visit(HaltInstruction instruction)
        {
            lines.Add("\tHALT");
        }

        public void Visit(MoveInstruction instruction)
        {
            lines.Add($"\tMOVE {instruction.Destination.GetAssemblySymbol()},{instruction.Source.GetAssemblySymbol()}");
        }

        public void Visit(StoreInstruction instruction)
        {
            lines.Add($"\tSTORE {instruction.Source.GetAssemblySymbol()},{instruction.MemoryAddress.GetAssemblySymbol()}");
        }

        public void Visit(LoadInstruction instruction)
        {
            lines.Add($"\tLOAD {instruction.Destination.GetAssemblySymbol()},{instruction.MemoryAddress.GetAssemblySymbol()}");
        }

        public void Visit(AddInstruction instruction)
        {
            lines.Add($"\tADD {instruction.Source.GetAssemblySymbol()},{instruction.Addend.GetAssemblySymbol()},{instruction.Destination.GetAssemblySymbol()}");
        }

        public void Visit(BranchInstruction instruction)
        {
            lines.Add($"\tBRANCH");
        }
    }
}