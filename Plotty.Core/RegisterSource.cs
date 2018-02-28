namespace Plotty
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

        public override int GetValue(PlottyCore plottyCore)
        {
            return plottyCore.Registers[Register.Id];
        }
    }
}