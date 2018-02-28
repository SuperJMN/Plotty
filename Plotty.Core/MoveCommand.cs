﻿namespace Plotty.Core
{
    public class MoveCommand : Command
    {
        public MoveCommand(PlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var inst = (MoveInstruction)PlottyCore.CurrentLine.Instruction;

            switch (inst.Source)
            {
                case ImmediateSource im:
                    PlottyCore.Registers[inst.Destination.Id] = im.Immediate;
                    break;
                case RegisterSource reg:
                    PlottyCore.Registers[inst.Destination.Id] = PlottyCore.Registers[reg.Register.Id];
                    break;
            }

            PlottyCore.GoToNext();
        }
    }
}