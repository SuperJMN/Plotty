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
            if (Equals(One, Another))
            {
                return $"Jump to {Target}";
            }

            return $"Jump to {Target} if {One} is {Operator.Name} {Another}";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}