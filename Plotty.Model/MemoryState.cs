using CodeGen.Core;

namespace Plotty.Model
{
    public class MemoryState
    {
        public MemoryState(int index, Reference reference, int value)
        {
            Index = index;
            Reference = reference;
            Value = value;
        }

        public int Index { get; }
        public Reference Reference { get; }
        public int Value { get; }
    }
}