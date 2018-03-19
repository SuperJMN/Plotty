using ReactiveUI;

namespace Plotty.Uwp
{
    public class MemoryViewModel : ReactiveObject
    {
        private int val;
        private bool hasChanged;

        public MemoryViewModel(int index, int value, string reference = null)
        {
            Index = index;
            Value = value;
            Reference = reference;
        }

        public string Name => $"{Index}";

        public string Tag => $"{Name} - {Value}";

        public int Index { get; }

        public int Value
        {
            get { return val; }
            set
            {
                HasChanged = !val.Equals(value);
                
                this.RaiseAndSetIfChanged(ref val, value);
                this.RaisePropertyChanged(nameof(Tag));
            }
        }

        public string Reference { get; }

        public bool HasChanged
        {
            get { return hasChanged; }
            set { this.RaiseAndSetIfChanged(ref hasChanged, value); }
        }
    }
}