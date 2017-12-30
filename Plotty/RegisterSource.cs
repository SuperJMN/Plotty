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
            return $"Register {Register}";
        }
    }
}