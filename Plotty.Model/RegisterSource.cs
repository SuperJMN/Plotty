namespace Plotty.Model
{
    public class RegisterSource : Source
    {
        public Register Register { get; }

        public RegisterSource(Register register)
        {
            Register = register;
        }

        public override string ToString()
        {
            return $"{Register}";
        }

        public override int GetValue(IPlottyMachine plottyMachine)
        {
            return plottyMachine.Registers[Register.Id];
        }
    }
}