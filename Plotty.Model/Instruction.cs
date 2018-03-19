namespace Plotty.Model
{
    public abstract class Instruction
    {
        public abstract void Accept(ILineVisitor visitor);
    }
}