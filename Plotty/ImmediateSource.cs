namespace Plotty
{
    public class ImmediateSource : Source
    {
        public int Immediate { get; }

        public ImmediateSource(int immediate)
        {
            Immediate = immediate;
        }

        public override string ToString()
        {
            return $"immediate {Immediate}";
        }
    }
}