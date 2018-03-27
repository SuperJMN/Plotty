using System.Collections.Generic;
using Plotty.Model;

namespace Plotty.Compiler
{
    public class AssemblyGeneratingVisitor : ILineVisitor
    {
        private readonly List<string> lines = new List<string>();
        private string label;
        public IReadOnlyCollection<string> Lines => lines.AsReadOnly();

        public void Visit(HaltInstruction instruction)
        {
            lines.Add($"{label}\tHALT");
        }

        public void Visit(MoveInstruction instruction)
        {
            lines.Add($"{label}\tMOVE {instruction.Destination.GetAssemblySymbol()},{instruction.Source.GetAssemblySymbol()}");
        }

        public void Visit(StoreInstruction instruction)
        {
            lines.Add($"{label}\tSTORE {instruction.Source.GetAssemblySymbol()},{instruction.MemoryAddress.GetAssemblySymbol()}");
        }

        public void Visit(LoadInstruction instruction)
        {
            lines.Add($"{label}\tLOAD {instruction.Destination.GetAssemblySymbol()},{instruction.MemoryAddress.GetAssemblySymbol()}");
        }

        public void Visit(BranchInstruction instruction)
        {
            var instructionName = instruction.Operator == BooleanOperator.Equal ? "BEQ" : "BLT";

            lines.Add($"{label}\t{instructionName} {instruction.One.GetAssemblySymbol()},{instruction.Another.GetAssemblySymbol()},{instruction.Target.GetAssemblySymbol()}");
        }

        public void Visit(Line line)
        {
            if (line.Label != null)
            {
                var labelName = line.Label;
                label = $"{labelName}:";
            }
            else
            {
                label = "";
            }
        }

        public void Visit(ArithmeticInstruction instruction)
        {
            var instructionName = instruction.ArithmeticOperator == ArithmeticOperator.Add ? "ADD" : "SUB";
            lines.Add($"{label}\t{instructionName} {instruction.Left.GetAssemblySymbol()},{instruction.Right.GetAssemblySymbol()},{instruction.Destination.GetAssemblySymbol()}");           
        }
    }
}