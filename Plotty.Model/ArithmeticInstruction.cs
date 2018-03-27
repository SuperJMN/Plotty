namespace Plotty.Model
{
    public class ArithmeticInstruction : Instruction
    {
        public ArithmeticOperator ArithmeticOperator { get; set; }
        public Register Left { get; set; }
        public Source Right { get; set; }
        public Register Destination { get; set; }
        
        public override string ToString()
        {
            return $"{ArithmeticOperator.Name} {Right} to {Left}, store into {Destination}";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}