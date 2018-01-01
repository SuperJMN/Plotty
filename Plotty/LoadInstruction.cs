namespace Plotty
{
    public class LoadInstruction : Instruction
    {
        public int Address { get; set; }
        public Register Destination { get; set; }

        public override string ToString()
        {
            return $"Load MEM:{Address} into {Destination}";
        }
    }
}