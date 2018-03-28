using Plotty.Model;

namespace Plotty.Commands
{
    public class MoveCommand : Command
    {
        public MoveCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {
        }

        public override void Execute()
        {
            var inst = (MoveInstruction)PlottyMachine.CurrentLine.Instruction;

            switch (inst.Source)
            {
                case ImmediateSource im:
                    PlottyMachine.Registers[inst.Destination.Id] = im.Immediate;
                    break;
                case RegisterSource reg:
                    PlottyMachine.Registers[inst.Destination.Id] = PlottyMachine.Registers[reg.Register.Id];
                    break;
            }

            PlottyMachine.GoToNext();
        }
    }
}