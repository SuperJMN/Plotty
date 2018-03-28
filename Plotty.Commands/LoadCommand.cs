using Plotty.Model;

namespace Plotty.Commands
{
    public class LoadCommand : Command
    {
        public LoadCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {
        }

        public override void Execute()
        {
            var inst = (LoadInstruction)PlottyMachine.CurrentLine.Instruction;
            var value = inst.MemoryAddress.GetValue(PlottyMachine);
            PlottyMachine.Registers[inst.Destination.Id] = value;
           
            PlottyMachine.GoToNext();
        }
    }
}