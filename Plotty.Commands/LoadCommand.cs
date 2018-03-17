using Plotty.Common;
using Plotty.Common.Instructions;

namespace Plotty.Commands
{
    public class LoadCommand : Command
    {
        public LoadCommand(IPlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var inst = (LoadInstruction)PlottyCore.CurrentLine.Instruction;
            var value = inst.MemoryAddress.GetValue(PlottyCore);
            PlottyCore.Registers[inst.Destination.Id] = value;
           
            PlottyCore.GoToNext();
        }
    }
}