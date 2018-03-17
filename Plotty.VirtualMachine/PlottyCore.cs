using System;
using System.Collections.Generic;
using System.Linq;
using Plotty.Commands;
using Plotty.Common;
using Plotty.Common.Instructions;

namespace Plotty.VirtualMachine
{
    public class PlottyCore : IPlottyCore
    {
        private const int RegCount = 8;
        private const int MemoryCount = 64 * 1024;

        public int[] Registers { get; private set; } = new int[RegCount];

        private List<Line> Instructions { get; set; }

        public Line CurrentLine { get; set; }

        public int LineNumber { get; private set; }
        public int[] Memory { get; private set; } = new int[MemoryCount];
        public Status Status { get; set; } = Status.Running;
        public bool CanExecute => CurrentLine != null && Status != Status.Halted;

        public void Load(Line[] cmds)
        {
            Instructions = cmds.ToList();
            LineNumber = 0;
            Status = Status.Running;
            CurrentLine = cmds.First();
            ClearMemory();
        }

        private void ClearMemory()
        {
            Array.Clear(Registers, 0, RegCount);
            Array.Clear(Memory, 0, MemoryCount);
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
                        cmd = new MoveCommand(this);
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

                    case StoreInstruction _:
                        cmd = new StoreCommand(this);
                        break;

                    case LoadInstruction _:
                        cmd = new LoadCommand(this);
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
            if (LineNumber + count >= Instructions.Count)
            {
                CurrentLine = null;
                return;
            }

            LineNumber += count;
            CurrentLine = Instructions[LineNumber];
        }

        public void GoTo(string labelName)
        {
            var inst = Instructions.Single(x => x.Label != null && x.Label.Name == labelName);
            var index = Instructions.IndexOf(inst);
            GoTo(index);
        }

        public void GoTo(int id)
        {
            CurrentLine = Instructions[id];
            LineNumber = id;
        }
    }
}