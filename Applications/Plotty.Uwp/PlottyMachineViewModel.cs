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
    public class PlottyMachineViewModel : ReactiveObject
    {
        private IList<RegisterViewModel> registers;
        private IEnumerable<Line> lines;
        private LineViewModel currentLine;
        private IList<MemoryViewModel> memory;
        private PlottyMachine PlottyCore { get; }
        public ConsoleHandler Console { get; set; }

        public PlottyMachineViewModel(PlottyMachine plottyCore)
        {
            PlottyCore = plottyCore;
            Registers = PlottyCore.Registers.Select((value, index) => new RegisterViewModel(index, value)).ToList();
            memory = PlottyCore.Memory.Select((value, index) => new MemoryViewModel(index, value)).ToList();
            Console = new ConsoleHandler(PlottyCore.Memory, 20, 20);
        }

        public async Task Execute(CancellationToken ct)
        {
            History.Clear();
            while (PlottyCore.CanExecute)
            {
                CurrentLine = new LineViewModel(PlottyCore.LineNumber, PlottyCore.CurrentLine);
                PlottyCore.Execute();
                RefreshRegisters();
                RefreshMemory();

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

        private void RefreshMemory()
        {
            for (var i = 0; i < PlottyCore.Memory.Length; i++)
            {
                Memory[i].Value = PlottyCore.Memory[i];
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
            get => registers;
            set => this.RaiseAndSetIfChanged(ref registers, value);
        }

        public int Delay { get; set; }

        public IList<MemoryViewModel> Memory
        {
            get => memory;
            set => this.RaiseAndSetIfChanged(ref memory, value);
        }
    }
}