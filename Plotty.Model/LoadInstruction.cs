namespace Plotty.Model
{
    public class LoadInstruction : Instruction
    {
        public Register Destination { get; set; }
        public MemoryAddress MemoryAddress { get; set; }

        public override string ToString()
        {
            return $"Load Memory at [{MemoryAddress}] into {Destination}";
        }
    }
}