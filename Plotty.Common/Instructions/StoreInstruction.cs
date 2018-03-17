namespace Plotty.Common.Instructions
{
    public class StoreInstruction : Instruction
    {
        public MemoryAddress Address { get; set; }
        public Source Source { get; set; }

        public override string ToString()
        {
            return $"Store {Source} in Memory at [{Address}]";
        }
    }
}