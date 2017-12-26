namespace Plotty
{
    public class BranchCommand : Command
    {
        public BranchCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var instruction = (BranchInstruction)PlottyCore.CurrentInstruction;
            var r1 = PlottyCore.Registers[instruction.One.Id];
            var r2 = PlottyCore.Registers[instruction.One.Id];
            if (r1 == r2)
            {
                if (instruction.Target.Label != null)
                {
                    PlottyCore.GoTo(instruction.Target.Label);
                }
            }
            else
            {
                PlottyCore.GoToNext();
            }
        }
    }
}