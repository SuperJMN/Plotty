using Plotty.Model;

namespace Plotty.Commands
{
    public class ArithmeticCommand : Command
    {
        public ArithmeticCommand(IPlottyCore plottyCore) : base(plottyCore)
        {
        }

        public override void Execute()
        {
            var inst = (ArithmeticInstruction)PlottyCore.CurrentLine.Instruction;
            var origin = inst.Left.Id;
            var destination = inst.Destination.Id;

            int value = inst.Right.GetValue(PlottyCore);
            
            if (inst.Operator == Operators.Add)
            {
                PlottyCore.Registers[destination] = PlottyCore.Registers[origin] + value;            
            }

            if (inst.Operator == Operators.Substract)
            {
                PlottyCore.Registers[destination] = PlottyCore.Registers[origin] - value;            
            }

            if (inst.Operator == Operators.Multiply)
            {
                PlottyCore.Registers[destination] = PlottyCore.Registers[origin] * value;            
            }

            PlottyCore.GoToNext();
        }
    }
}