namespace Plotty
{
    internal class LoadInstruction : Instruction
    {
        public Register Destination { get; set; }
        public Source Source { get; set; }
    }
}