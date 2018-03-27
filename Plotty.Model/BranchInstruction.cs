namespace Plotty.Model
{
    public class BranchInstruction : Instruction
    {
        public Register One { get; set; }
        public Register Another { get; set; }
        public JumpTarget Target { get; set; }
        public BooleanOperator Operator { get; set; }

        public override string ToString()
        {
            return $"Jump to {Target} if {One} and {Another} are equal";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}