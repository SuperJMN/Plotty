namespace Plotty
{
    public class StoreInstruction : Instruction
    {
        public int Address { get; set; }
        public Source Source { get; set; }

        public override string ToString()
        {
            return $"Store {Source} in MEM:{Address}";
        }
    }
}