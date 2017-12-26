using System;
using System.Collections.Generic;
using System.Linq;

namespace Plotty
{
    public class PlottyCore
    {
        public int[] Registers { get; } = new int[8];

        private IList<Instruction> Instructions { get; set; }

        public void Load(IList<Instruction> cmds)
        {
            Instructions = cmds.ToList();
            CurrentInstruction = cmds.First();
        }

        public Instruction CurrentInstruction { get; set; }

        private int InstructionIndex { get; set; }
        public int[] Memory { get; } = new int[64 * 1024];

        public void Execute()
        {
            if (CurrentInstruction != null)
            {
                Command cmd;
                switch (CurrentInstruction)
                {
                    case LoadInstruction _:
                        cmd = new LoadCommand(this);
                        break;
                    case ArithmeticInstruction _:
                        cmd = new AddCommand(this);
                        break;
                    case BranchInstruction _:
                        cmd = new BranchCommand(this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                cmd.Execute();
            }
        }

        public void GoToNext()
        {
            Skip(1);
        }

        private void GoToRelative(int relativeToCurrent)
        {
            Skip(relativeToCurrent);
        }

        private void Skip(int count = 1)
        {
            if (InstructionIndex + count >= Instructions.Count)
            {
                CurrentInstruction = null;
                return;
            }

            InstructionIndex += count;
            CurrentInstruction = Instructions[InstructionIndex];
        }

        public void GoTo(string labelName)
        {
            var inst = Instructions.Single(x => x.Label != null && x.Label.Name == labelName);
            var index = Instructions.IndexOf(inst);
            CurrentInstruction = inst;
            InstructionIndex = index;
        }

        public void GoTo(int id)
        {
            CurrentInstruction = Instructions[id];
        }
    }
}