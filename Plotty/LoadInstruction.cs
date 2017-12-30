namespace Plotty
{
    internal class LoadInstruction : Instruction
    {
        public Register Destination { get; set; }
        public Source Source { get; set; }

        public override string ToString()
        {
            return $"Load {Source} into {Destination}";
        }
    }
}