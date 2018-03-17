namespace Plotty.Model
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

        public override int GetValue(IPlottyCore plottyCore)
        {
            return Immediate;
        }
    }
}