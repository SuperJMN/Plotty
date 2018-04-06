namespace Plotty.Model
{
    public class Line
    {
        public Label Label { get; }
        public Instruction Instruction { get; }

        public Line(Label label, Instruction instruction) : this(instruction)
        {
            Label = label;
        }

        public Line(Instruction instruction)
        {
            Instruction = instruction;
        }

        public override string ToString()
        {
            if (Label != null)
            {
                return $"Label '{Label}'";
            }

            return $"{Instruction}";
        }

        public void Accept(ILineVisitor lineVisitor)
        {
            lineVisitor.Visit(this);
            Instruction?.Accept(lineVisitor);
        }
    }
}