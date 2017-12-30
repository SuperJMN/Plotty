using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plotty;
using ReactiveUI;

namespace PlottyRunner
{
    public class CoreViewModel : ReactiveObject
    {
        private IList<RegisterViewModel> registers;
        private IEnumerable<Line> lines;
        private LineViewModel currentLine;
        public PlottyCore PlottyCore { get; }

        public CoreViewModel(PlottyCore plottyCore)
        {
            PlottyCore = plottyCore;
            Registers = PlottyCore.Registers.Select((value, index) => new RegisterViewModel(index, value)).ToList();
        }

        public async Task Execute(CancellationToken ct)
        {
            History.Clear();
            while (PlottyCore.CanExecute)
            {
                CurrentLine = new LineViewModel(PlottyCore.InstructionIndex, PlottyCore.CurrentLine);
                await Task.Delay(Delay, ct);
                PlottyCore.Execute();
                RefreshRegisters();
                ct.ThrowIfCancellationRequested();
            }            
        }

        private void RefreshRegisters()
        {
            for (var i = 0; i < PlottyCore.Registers.Length; i++)
            {
                Registers[i].Value = PlottyCore.Registers[i];
            }
        }

        public LineViewModel CurrentLine
        {
            get => currentLine;
            private set
            {
                this.RaiseAndSetIfChanged(ref currentLine, value);
                History.Add(value);
            }
        }

        public ObservableCollection<LineViewModel> History { get; } = new ObservableCollection<LineViewModel>();

        public IEnumerable<Line> Lines
        {
            get => lines;
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

        public int Delay { get; set; }
    }
}