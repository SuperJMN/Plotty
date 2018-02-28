namespace Plotty.Core
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

        public override int GetValue(PlottyCore plottyCore)
        {
            return Immediate;
        }
    }
}