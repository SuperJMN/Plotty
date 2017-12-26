namespace Plotty
{
    public class RegisterSource : Source
    {
        public Register Register { get; }

        public RegisterSource(Register register)
        {
            Register = register;
        }
    }
}