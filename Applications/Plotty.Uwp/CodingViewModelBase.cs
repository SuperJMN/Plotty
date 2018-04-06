using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plotty.Compiler;
using Plotty.VirtualMachine;
using ReactiveUI;

namespace Plotty.Uwp
{
    public abstract class CodingViewModelBase : ReactiveObject
    {
        private readonly ObservableAsPropertyHelper<bool> isBusyOaph;
        private string assemblyCode;
        private string error;

        public CodingViewModelBase()
        {
            Source = DefaultSourceCode;
            PlottyMachine = new PlottyMachineViewModel(new PlottyMachine());

            PlayCommand = ReactiveCommand.CreateFromObservable(() => Observable
                .StartAsync(Play)
                .TakeUntil(StopCommand));

            PlayCommand.ThrownExceptions.Subscribe(ex => { Error = ex.Message; });
            StopCommand = ReactiveCommand.Create(() => { }, PlayCommand.IsExecuting);

            isBusyOaph = PlayCommand.IsExecuting.ToProperty(this, model => model.IsBusy);
            Delay = 250;
            CompileCommand = ReactiveCommand.Create(Compile);
        }

        protected abstract string DefaultSourceCode { get; }

        public string AssemblyCode
        {
            get => assemblyCode;
            set => this.RaiseAndSetIfChanged(ref assemblyCode, value);
        }

        public ReactiveCommand<Unit, CompilationResult> CompileCommand { get; }

        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public string Error
        {
            get => error;
            private set => this.RaiseAndSetIfChanged(ref error, value);
        }

        public ReactiveCommand<Unit, Unit> PlayCommand { get; }

        public string Source { get; set; }

        public PlottyMachineViewModel PlottyMachine { get; }

        public bool IsBusy => isBusyOaph.Value;

        public int Delay
        {
            get => PlottyMachine.Delay;
            set
            {
                if (value == 0)
                {
                    value = 1;
                }

                PlottyMachine.Delay = value;
                this.RaisePropertyChanged(nameof(DelayTag));
            }
        }

        public string DelayTag => $"{Delay} ms";

        public abstract string Name { get; }

        private CompilationResult Compile()
        {
            return new PlottyCompiler().Compile(Source);
        }

        private async Task Play(CancellationToken cancellationToken)
        {
            Error = string.Empty;
            var compilationResult = Compile();
            AssemblyCode = string.Join("\n", compilationResult.Code);

            //PlottyMachine.AddressMap = compilationResult.GenerationResult.AddressMap;
            await PlottyMachine.Execute(compilationResult.GenerationResult.Lines, cancellationToken);
        }
    }
}