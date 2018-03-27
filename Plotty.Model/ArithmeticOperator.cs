namespace Plotty.Model
{
    public class ArithmeticOperator
    {
        public string Symbol { get; }
        public string Name { get; }
        
        public static ArithmeticOperator Add = new ArithmeticOperator("+", "Add");
        public static ArithmeticOperator Substract = new ArithmeticOperator("-", "Substract");
        public static ArithmeticOperator Multiply = new ArithmeticOperator("*", "Multiply");

        private ArithmeticOperator(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}