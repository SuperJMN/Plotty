using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plotty.Compiler;
using Plotty.Model;
using Plotty.VirtualMachine;
using ReactiveUI;

namespace Plotty.Uwp
{
    public abstract class CodingViewModelBase : ReactiveObject
    {
        private string error;
        private readonly ObservableAsPropertyHelper<bool> isBusyOaph;
        private string assemblyCode;
        protected abstract string DefaultSourceCode { get; }

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

        public string AssemblyCode
        {
            get => assemblyCode;
            set => this.RaiseAndSetIfChanged(ref assemblyCode, value);
        }

        public ReactiveCommand<Unit, CompilationResult> CompileCommand { get; }

        private CompilationResult Compile()
        {
            return new PlottyCompiler().Compile(Source);
        }

        public ReactiveCommand<Unit, Unit> StopCommand { get; }

        public string Error
        {
            get => error;
            set => this.RaiseAndSetIfChanged(ref error, value);
        }

        public ReactiveCommand<Unit, Unit> PlayCommand { get; }

        private async Task Play(CancellationToken cancellationToken)
        {
            Error = string.Empty;
            var result = Compile();
            AssemblyCode = string.Join("\n", result.Code);
            PlottyMachine.Lines = result.GenerationResult.Lines;
            await PlottyMachine.Execute(cancellationToken);
        }

        public string Source { get; set; }
        public PlottyMachineViewModel PlottyMachine { get; }

        public bool IsBusy => isBusyOaph.Value;

        public int Delay
        {
            get => PlottyMachine.Delay;
            set
            {
                PlottyMachine.Delay = value;
                this.RaisePropertyChanged(nameof(DelayTag));
            }
        }

        public string DelayTag => $"{Delay} ms";

        public abstract string Name { get; }
    }
}