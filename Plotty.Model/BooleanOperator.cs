namespace Plotty.Model
{
    public class BooleanOperator
    {
        public string Symbol { get; }
        public string Name { get; }
        
        public static BooleanOperator Equal = new BooleanOperator("==", "Equal");
        public static BooleanOperator LessThan = new BooleanOperator("<", "Less than");

        private BooleanOperator(string symbol, string name)
        {
            Symbol = symbol;
            Name = name;
        }
    }
}