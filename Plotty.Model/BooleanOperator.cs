namespace Plotty.Model
{
    public class BooleanOperator
    {
        public string Symbol { get; }
        public string Name { get; }
        
        public static BooleanOperator Equal = new BooleanOperator("==", "Equal");
        public static BooleanOperator LessThan = new BooleanOperator("<", "Less than");
        public static BooleanOperator GreaterThan = new BooleanOperator(">", "Greater than");
        public static BooleanOperator GreaterOrEqual = new BooleanOperator(">=", "Greater than or equal to");
        public static BooleanOperator LessThanOrEqual = new BooleanOperator("<=", "Less than or equal to");

        private BooleanOperator(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}