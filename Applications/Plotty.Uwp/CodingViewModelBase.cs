using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plotty.Model;
using Plotty.VirtualMachine;
using ReactiveUI;

namespace Plotty.Uwp
{
    public abstract class CodingViewModelBase : ReactiveObject
    {
        private string error;
        private readonly ObservableAsPropertyHelper<bool> isBusyOH;
        protected abstract string DefaultSourceCode { get; }

        public CodingViewModelBase()
        {
            Source = DefaultSourceCode;
            CoreViewModel = new CoreViewModel(new PlottyMachine());
            PlayCommand = ReactiveCommand.CreateFromObservable(() => Observable
                .StartAsync(Play)
                .TakeUntil(CancelCommand));
            PlayCommand.ThrownExceptions.Subscribe(ex => { Error = ex.Message; });
            CancelCommand = ReactiveCommand.Create(
                () => { },
                PlayCommand.IsExecuting);

            isBusyOH = PlayCommand.IsExecuting.ToProperty(this, model => model.IsBusy);
            Delay = 250;
        }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public string Error
        {
            get => error;
            set => this.RaiseAndSetIfChanged(ref error, value);
        }

        public ReactiveCommand<Unit, Unit> PlayCommand { get; }

        private async Task Play(CancellationToken cancellationToken)
        {
            Error = string.Empty;
            var instructions = GeneratePlottyInstructions(Source);
            CoreViewModel.Lines = instructions;
            await CoreViewModel.Execute(cancellationToken);
        }

        protected abstract IEnumerable<Line> GeneratePlottyInstructions(string source);

        public string Source { get; set; }
        public CoreViewModel CoreViewModel { get; }

        public bool IsBusy => isBusyOH.Value;

        public int Delay
        {
            get => CoreViewModel.Delay;
            set
            {
                CoreViewModel.Delay = value;
                this.RaisePropertyChanged(nameof(DelayTag));
            }
        }

        public string DelayTag => $"{Delay} ms";

        public abstract string Name { get; }
    }
}