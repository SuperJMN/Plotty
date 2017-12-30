using System;
using System.Collections.Generic;
using System.Linq;

namespace Plotty
{
    public class PlottyCore
    {
        private const int RegCount = 8;
        private const int MemoryCount = 64 * 1024;

        public int[] Registers { get; private set; } = new int[RegCount];

        private List<Line> Instructions { get; set; }

        public Line CurrentLine { get; set; }

        public int InstructionIndex { get; private set; }
        public int[] Memory { get; private set; } = new int[MemoryCount];
        public Status Status { get; set; } = Status.Running;
        public bool CanExecute => CurrentLine != null && Status != Status.Halted;

        public void Load(Line[] cmds)
        {
            Instructions = cmds.ToList();
            InstructionIndex = 0;
            Status = Status.Running;
            CurrentLine = cmds.First();
            ClearMemory();
        }

        private void ClearMemory()
        {
            Registers = Enumerable.Repeat(0, RegCount).ToArray();
            Memory = Enumerable.Repeat(0, MemoryCount).ToArray();
        }

        public void Execute()
        {
            if (!CanExecute)
            {
                throw new InvalidOperationException("Cannot execute in the current state");
            }

            if (CurrentLine != null)
            {
                Command cmd;
                switch (CurrentLine.Instruction)
                {
                    case MoveInstruction _:
                        cmd = new LoadCommand(this);
                        break;
                    case ArithmeticInstruction _:
                        cmd = new AddCommand(this);
                        break;
                    case BranchInstruction _:
                        cmd = new BranchCommand(this);
                        break;
                    case HaltInstruction _:
                        cmd = new HaltCommand(this);
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
                CurrentLine = null;
                return;
            }

            InstructionIndex += count;
            CurrentLine = Instructions[InstructionIndex];
        }

        public void GoTo(string labelName)
        {
            var inst = Instructions.Single(x => x.Label != null && x.Label.Name == labelName);
            var index = Instructions.IndexOf(inst);
            CurrentLine = inst;
            InstructionIndex = index;
        }

        public void GoTo(int id)
        {
            CurrentLine = Instructions[id];
        }
    }
}