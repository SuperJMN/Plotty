using Plotty.Model;

namespace Plotty.Commands
{
    public class ArithmeticCommand : Command
    {
        public ArithmeticCommand(IPlottyMachine plottyMachine) : base(plottyMachine)
        {
        }

        public override void Execute()
        {
            var inst = (ArithmeticInstruction)PlottyMachine.CurrentLine.Instruction;
            var origin = inst.Left.Id;
            var destination = inst.Destination.Id;

            int value = inst.Right.GetValue(PlottyMachine);
            
            if (inst.ArithmeticOperator == ArithmeticOperator.Add)
            {
                PlottyMachine.Registers[destination] = PlottyMachine.Registers[origin] + value;            
            }

            if (inst.ArithmeticOperator == ArithmeticOperator.Substract)
            {
                PlottyMachine.Registers[destination] = PlottyMachine.Registers[origin] - value;            
            }

            if (inst.ArithmeticOperator == ArithmeticOperator.Multiply)
            {
                PlottyMachine.Registers[destination] = PlottyMachine.Registers[origin] * value;            
            }

            PlottyMachine.GoToNext();
        }
    }
}