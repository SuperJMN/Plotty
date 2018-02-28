using ReactiveUI;

namespace Plotty.Uwp
{
    public class RegisterViewModel : ReactiveObject
    {
        private int val;
        private bool hasChanged;

        public RegisterViewModel(int index, int value)
        {
            Index = index;
            Value = value;
        }

        public string Name => $"R{Index}";

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

        public bool HasChanged
        {
            get { return hasChanged; }
            set { this.RaiseAndSetIfChanged(ref hasChanged, value); }
        }
    }
}