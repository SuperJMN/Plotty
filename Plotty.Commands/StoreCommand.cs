using Plotty.Model;

namespace Plotty.Commands
{
    public class StoreCommand : Command
    {
        public StoreCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {
        }

        public override void Execute()
        {
            var inst = (StoreInstruction)PlottyMachine.CurrentLine.Instruction;

            int value = inst.Source.GetValue(PlottyMachine);
            var address = inst.MemoryAddress.GetAddress(PlottyMachine);

            PlottyMachine.Memory[address] = value;
           
            PlottyMachine.GoToNext();
        }
    }
}