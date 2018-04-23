using System;
using Plotty.Compiler;
using Plotty.VirtualMachine;

namespace Plotty.Uwp.Reloaded
{
    public class MachineContext : IDisposable
    {
        private readonly PlottyMachine machine;
        private MachineState state;

        public MachineContext(string source)
        {
            machine = new PlottyMachine();
            machine.Load(new PlottyCompiler().Compile(source).GenerationResult.Lines);
        }

        public void Dispose()
        {
        }

        public void Execute()
        {
            machine.Execute();
            state = new MachineState(machine.Registers, machine.Memory, machine.CurrentLine);
        }

        public MachineState State => state;
        public bool CanExecute => machine.CanExecute;
    } 
}