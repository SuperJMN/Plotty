using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using Plotty;
using ReactiveUI;
using Superpower;

namespace PlottyRunner
{
    public class MainViewModel : ReactiveObject
    {
        private string error;

        public MainViewModel()
        {
            Source = "LOAD\tR1,#5\r\npepito:ADD\tR2,#1\r\n\tBRANCH\tR1,R2,end\r\n        BRANCH\tR0,R0,pepito\r\nend:\tHALT";
            CoreViewModel = new CoreViewModel(new PlottyCore());
            ParseAndExecute = ReactiveCommand.Create(() => Parse());
            ParseAndExecute.ThrownExceptions.Subscribe(ex => { Error = ex.Message; });
        }

        public string Error
        {
            get { return error; }
            set { this.RaiseAndSetIfChanged(ref error, value); }
        }

        public ReactiveCommand<Unit, Unit> ParseAndExecute { get; set; }

        private void Parse()
        {
            Error = string.Empty;
            var instructions = Parser.AsmParser.Parse(new Tokenizer().Tokenize(Source));
            CoreViewModel.Lines = instructions;
            CoreViewModel.Execute();
        }

        public string Source { get; set; }
        public CoreViewModel CoreViewModel { get; set; }
    }

    public class CoreViewModel : ReactiveObject
    {
        private IEnumerable<RegisterViewModel> registers;
        private IEnumerable<Line> lines;
        public PlottyCore PlottyCore { get; }

        public CoreViewModel(PlottyCore plottyCore)
        {
            PlottyCore = plottyCore;
            ExecuteCommand = ReactiveCommand.Create(() => Execute());
        }

        public ReactiveCommand<Unit, Unit> ExecuteCommand { get; set; }

        public void Execute()
        {
            while (PlottyCore.CanExecute)
            {
                PlottyCore.Execute();
            }

            Registers = PlottyCore.Registers.Select((value, index) => new RegisterViewModel(index, value));
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

        public IEnumerable<RegisterViewModel> Registers
        {
            get { return registers; }
            set { this.RaiseAndSetIfChanged(ref registers, value); }
        }
    }

    public class RegisterViewModel : ReactiveObject
    {
        public RegisterViewModel(int index, int value)
        {
            Index = index;
            Value = value;
        }

        public string Tag => $"R{Index} - {Value}";

        public int Index { get; }
        public int Value { get; }
    }
}