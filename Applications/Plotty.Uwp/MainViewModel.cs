using System.Collections.Generic;
using System.Linq;
using ReactiveUI;

namespace Plotty.Uwp
{
    public class MainViewModel : ReactiveObject
    {
        private CodingViewModelBase selectedView;

        public MainViewModel()
        {
            SelectedView = CodingViews.First();
        }

        public List<CodingViewModelBase> CodingViews { get; set; } = new List<CodingViewModelBase>()
        {
            new CLangCodingViewModel(),
            new AssemblyCodingViewModel(),
        };

        public CodingViewModelBase SelectedView
        {
            get => selectedView;
            set => this.RaiseAndSetIfChanged(ref selectedView, value);
        }
    }
}