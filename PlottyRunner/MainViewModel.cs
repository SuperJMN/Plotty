using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plotty;
using ReactiveUI;
using Superpower;

namespace PlottyRunner
{
    public class MainViewModel : ReactiveObject
    {
        private string error;
        private readonly ObservableAsPropertyHelper<bool> isBusyOH;

        public MainViewModel()
        {
            Source = "LOAD\tR1,#5\r\npepito:ADD\tR2,#1\r\n\tBRANCH\tR1,R2,end\r\n        BRANCH\tR0,R0,pepito\r\nend:\tHALT";
            CoreViewModel = new CoreViewModel(new PlottyCore());
            PlayCommand = ReactiveCommand.CreateFromObservable(() => Observable
                .StartAsync(Play)
                .TakeUntil(CancelCommand));
            PlayCommand.ThrownExceptions.Subscribe(ex => { Error = ex.Message; });
            CancelCommand = ReactiveCommand.Create(
                () => { },
                PlayCommand.IsExecuting);

            isBusyOH = PlayCommand.IsExecuting.ToProperty(this, model => model.IsBusy);
        }

        public ReactiveCommand<Unit, Unit> CancelCommand { get; set; }

        public string Error
        {
            get { return error; }
            set { this.RaiseAndSetIfChanged(ref error, value); }
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
        public CoreViewModel CoreViewModel { get; set; }

        public bool IsBusy => isBusyOH.Value;
    }

    public class CoreViewModel : ReactiveObject
    {
        private IList<RegisterViewModel> registers;
        private IEnumerable<Line> lines;
        private int currentLine;
        public PlottyCore PlottyCore { get; }

        public CoreViewModel(PlottyCore plottyCore)
        {
            PlottyCore = plottyCore;
            Registers = PlottyCore.Registers.Select((value, index) => new RegisterViewModel(index, value)).ToList();
        }

        public async Task Execute(CancellationToken ct)
        {
            while (PlottyCore.CanExecute)
            {
                await Task.Delay(1000, ct);
                PlottyCore.Execute();
                RefreshRegister();
                CurrentLine = PlottyCore.InstructionIndex;
                ct.ThrowIfCancellationRequested();
            }            
        }

        private void RefreshRegister()
        {
            for (int i = 0; i < PlottyCore.Registers.Length; i++)
            {
                Registers[i].Value = PlottyCore.Registers[i];
            }
        }

        public int CurrentLine
        {
            get { return currentLine; }
            set { this.RaiseAndSetIfChanged(ref currentLine, value); }
        }

        public IEnumerable<Line> Lines
        {
            get { return lines; }
            set
            {
                lines = value;
                PlottyCore.Load(value.ToArray());
            }
        }

        public IList<RegisterViewModel> Registers
        {
            get { return registers; }
            set { this.RaiseAndSetIfChanged(ref registers, value); }
        }
    }

    public class RegisterViewModel : ReactiveObject
    {
        private int val;

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
                this.RaiseAndSetIfChanged(ref val, value);
                this.RaisePropertyChanged(nameof(Tag));
            }
        }
    }
}