namespace Plotty
{
    public class RegisterIndirectAddress : MemoryAddress
    {
        public Register Register { get; }

        public RegisterIndirectAddress(Register register)
        {
            Register = register;
        }


        public override int GetAddress(PlottyCore plottyCore)
        {
            return plottyCore.Registers[Register.Id];
        }

        public override string ToString()
        {
            return $"{Register}";
        }
    }
}