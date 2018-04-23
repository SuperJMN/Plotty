using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeGen.Core;
using Plotty.Model;
using Plotty.VirtualMachine;
using ReactiveUI;

namespace Plotty.Uwp
{
    public class PlottyMachineViewModel : ReactiveObject
    {
        private IList<RegisterViewModel> registers;
        private LineViewModel currentLine;
        private IDictionary<Reference, int> addressMap;
        private IList<MemoryViewModel> memoryMap;
        private PlottyMachine PlottyMachine { get; }
        public ConsoleHandler Console { get; set; }

        public PlottyMachineViewModel(PlottyMachine plottyMachine)
        {
            PlottyMachine = plottyMachine;
            Registers = PlottyMachine.Registers.Select((value, index) => new RegisterViewModel(index, value)).ToList();
            Console = new ConsoleHandler(PlottyMachine.Memory, 20, 20);
        }

        public async Task Execute(List<ILine> lines, CancellationToken ct)
        {
            History.Clear();
            PlottyMachine.Load(lines);
            
            while (PlottyMachine.CanExecute)
            {
                CurrentLine = new LineViewModel(PlottyMachine.LineNumber, PlottyMachine.CurrentLine);
                PlottyMachine.Execute();
                RefreshRegisters();
                //RefreshMemory();

                if (CurrentLine?.Line?.Instruction is StoreInstruction)
                {
                    RefreshConsole();
                }

                ct.ThrowIfCancellationRequested();
                await Task.Delay(Delay, ct);
            }
        }

        public IList<MemoryViewModel> MemoryMap
        {
            get => memoryMap;
            set => this.RaiseAndSetIfChanged(ref memoryMap, value);
        }

        private void RefreshConsole()
        {
            Console.Update();
        }

        private void RefreshRegisters()
        {
            for (var i = 0; i < PlottyMachine.Registers.Length; i++)
            {
                Registers[i].Value = PlottyMachine.Registers[i];
            }
        }

        private void RefreshMemory()
        {
            foreach (var m in MemoryMap)
            {
                var address = m.Index;
                var value = PlottyMachine.Memory[address];                
                m.Value = value;
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

        public IList<RegisterViewModel> Registers
        {
            get => registers;
            set => this.RaiseAndSetIfChanged(ref registers, value);
        }

        public int Delay { get; set; }

        public IDictionary<Reference, int> AddressMap
        {
            get => addressMap;
            set
            {
                this.RaiseAndSetIfChanged(ref addressMap, value);

                MemoryMap = value.Select(x => new MemoryViewModel(x.Value, 0, x.Key)).ToList();
            }
        }
    }
}