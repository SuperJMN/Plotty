using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Superpower;

namespace Plotty.Uwp
{
    public class MainViewModel : ReactiveObject
    {
        private string error;
        private readonly ObservableAsPropertyHelper<bool> isBusyOH;

        public MainViewModel()
        {
            Source = "\t\tMOVE\tR0,#27\r\n\t\tMOVE\tR1,#65\r\nstart:\tSTORE\tR1,0+R2\r\n\t\tADD\tR1,#1\r\n\t\tADD\tR2,#1\r\n\t\tBRANCH\tR0,R2,end\r\n\t\tBRANCH\tR3,R3,start\r\nend:\tHALT";
            CoreViewModel = new CoreViewModel(new PlottyCore());
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
            var instructions = Parser.AsmParser.Parse(new Tokenizer().Tokenize(Source));
            CoreViewModel.Lines = instructions;
            await CoreViewModel.Execute(cancellationToken);
        }

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
    }
}