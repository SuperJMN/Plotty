using System;
using System.Collections.Generic;
using System.Linq;

namespace Plotty
{
    public class PlottyCore
    {
        public uint[] Registers { get; set; } = new uint[8];

        private IList<Instruction> Instructions { get; set; }

        public void Load(IList<Instruction> cmds)
        {
            this.Instructions = cmds.ToList();
            CurrentInstruction = cmds.First();
            Memory[0] = 124;
        }

        private Instruction CurrentInstruction { get; set; }

        private int InstructionIndex { get; set; }
        public uint[] Memory { get; } = new uint[64 * 1024];

        public bool Execute()
        {
            if (CurrentInstruction != null)
            {
                Command cmd;
                switch (CurrentInstruction.OpCode)
                {
                    case OpCodes.Load:
                        cmd = new LoadCommand(this, CurrentInstruction.FirstRegister, CurrentInstruction.Address);
                        break;
                    case OpCodes.Store:
                        cmd = new StoreCommand(this);
                        break;
                    case OpCodes.Add:
                        cmd = new AddCommand(this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                cmd.Execute();
                return true;
            }

            return false;
        }

        public void Next()
        {
            if (InstructionIndex +1 == Instructions.Count)
            {
                CurrentInstruction = null;
                return;
            }

            CurrentInstruction = Instructions[++InstructionIndex];
        }
    }
}