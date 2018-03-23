namespace Plotty.Model
{
    public class Operators
    {
        public string Symbol { get; }
        public string Name { get; }
        
        public static Operators Add = new Operators("+", "Add");
        public static Operators Substract = new Operators("-", "Substract");
        public static Operators Multiply = new Operators("*", "Multiply");

        private Operators(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}