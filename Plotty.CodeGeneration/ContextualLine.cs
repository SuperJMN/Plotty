using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    internal class ContextualLine : ILine
    {
        public ILine Line { get; }
        public IntermediateCode Generator { get; }
        public string Description { get; }

        public ContextualLine(ILine line, IntermediateCode generator, string description)
        {
            Line = line;
            Generator = generator;
            Description = description;
        }

        public Label Label => Line.Label;
        public Instruction Instruction => Line.Instruction;
        public void Accept(ILineVisitor lineVisitor)
        {
            Line.Accept(lineVisitor);
        }

        public override string ToString()
        {
            return $"[{Description}]: {Line} <== {Generator}";
        }
    }
}