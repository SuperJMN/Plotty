namespace Plotty.Common.Instructions
{
    public class ArithmeticInstruction : Instruction
    {
        public Register Destination { get; set; }
        public Source Addend { get; set; }
        public Register Source { get; set; }

        public override string ToString()
        {
            return $"Add {Addend} to {Source}, store into {Destination}";
        }
    }
}