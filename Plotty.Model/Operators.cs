namespace Plotty.Model
{
    public class Operators
    {
        public string Symbol { get; }
        public string Name { get; }
        public static Operators Add = new Operators("+", "Add");
        public static Operators Substract = new Operators("-", "Substract");

        private Operators(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}