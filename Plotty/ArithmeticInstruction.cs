namespace Plotty
{
    public class ArithmeticInstruction : Instruction
    {
        public Register Destination { get; set; }
        public Source Addend { get; set; }
        public Register Source { get; set; }
    }
}