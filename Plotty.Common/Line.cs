using Plotty.Common.Instructions;

namespace Plotty.Common
{
    public class Line
    {
        public Label Label { get; }
        public Instruction Instruction { get; }

        public Line(Label label, Instruction instruction)
        {
            Label = label;
            Instruction = instruction;
        }

        public override string ToString()
        {
            return $"{Instruction}";
        }
    }
}