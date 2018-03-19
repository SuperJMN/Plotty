using System.Collections.Generic;
using CodeGen.Core;
using ReactiveUI;

namespace Plotty.Uwp
{
    public class MemoryViewModel : ReactiveObject
    {
        private int val;
        private bool hasChanged;
        private Reference reference;

        public MemoryViewModel(int index, int value, Reference reference = null)
        {
            Index = index;
            Value = value;
            Reference = reference;
        }

        public string Name => $"{Index}";

        public string Display => Reference != null? $"{Reference.Identifier} = {Value}" : Value.ToString();

        public int Index { get; }

        public int Value
        {
            get => val;
            set
            {
                HasChanged = !val.Equals(value);
                
                this.RaiseAndSetIfChanged(ref val, value);
                this.RaisePropertyChanged(nameof(Display));
            }
        }

        public Reference Reference
        {
            get => reference;
            set => this.RaiseAndSetIfChanged(ref reference, value);
        }

        public bool HasChanged
        {
            get => hasChanged;
            private set => this.RaiseAndSetIfChanged(ref hasChanged, value);
        }     
    }
}