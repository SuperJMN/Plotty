namespace Plotty.Model
{
    public interface ILine
    {
        Label Label { get; }
        Instruction Instruction { get; }
        void Accept(ILineVisitor lineVisitor);
    }

    public class Line : ILine
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
                return $"@'{Label}': {Instruction}";
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