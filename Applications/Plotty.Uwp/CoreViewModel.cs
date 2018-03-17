using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plotty.Model;
using Plotty.VirtualMachine;
using ReactiveUI;

namespace Plotty.Uwp
{
    public class CoreViewModel : ReactiveObject
    {
        private IList<RegisterViewModel> registers;
        private IEnumerable<Line> lines;
        private LineViewModel currentLine;
        public PlottyCore PlottyCore { get; }
        public ConsoleHandler Console { get; set; }

        public CoreViewModel(PlottyCore plottyCore)
        {
            PlottyCore = plottyCore;
            Registers = PlottyCore.Registers.Select((value, index) => new RegisterViewModel(index, value)).ToList();
            Console = new ConsoleHandler(PlottyCore.Memory, 80, 80);
        }

        public async Task Execute(CancellationToken ct)
        {
            History.Clear();
            while (PlottyCore.CanExecute)
            {
                CurrentLine = new LineViewModel(PlottyCore.LineNumber, PlottyCore.CurrentLine);
                PlottyCore.Execute();
                RefreshRegisters();

                if (CurrentLine?.Line?.Instruction is StoreInstruction)
                {
                    RefreshConsole();
                }

                ct.ThrowIfCancellationRequested();
                await Task.Delay(Delay, ct);
            }
        }

        private void RefreshConsole()
        {
            Console.Update();
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