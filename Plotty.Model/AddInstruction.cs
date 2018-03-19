namespace Plotty.Model
{
    public class AddInstruction : Instruction
    {
        public Register Destination { get; set; }
        public Source Addend { get; set; }
        public Register Source { get; set; }

        public override string ToString()
        {
            return $"Add {Addend} to {Source}, store into {Destination}";
        }

        public override void Accept(ILineVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}