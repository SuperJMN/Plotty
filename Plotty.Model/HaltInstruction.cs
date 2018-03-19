namespace Plotty.Model
{
    public class HaltInstruction : Instruction
    {
        public override string ToString()
        {
            return $"Halt";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}