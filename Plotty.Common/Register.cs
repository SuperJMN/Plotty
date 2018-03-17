namespace Plotty.Common
{
    public class Register
    {
        public int Id { get; }

        public Register(int id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return $"R{Id}";
        }
    }
}