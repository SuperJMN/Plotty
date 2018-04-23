namespace Plotty.Model
{
    public class NoOperation : Instruction
    {
        public override void Accept(ILineVisitor visitor)
        {            
            visitor.Visit(this);
        }
    }
}