namespace Plotty
{
    public class BranchInstruction : Instruction
    {
        public Register One { get; set; }
        public Register Another { get; set; }
        public JumpTarget Target { get; set; }
    }
}