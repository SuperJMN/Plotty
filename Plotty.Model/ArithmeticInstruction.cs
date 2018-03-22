namespace Plotty.Model
{
    public class ArithmeticInstruction : Instruction
    {
        public Operators Operator { get; set; }
        public Register Left { get; set; }
        public Source Right { get; set; }
        public Register Destination { get; set; }
        
        public override string ToString()
        {
            return $"{Operator.Name} {Right} to {Left}, store into {Destination}";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}