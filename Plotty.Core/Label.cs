namespace Plotty.Core
{
    public class Label
    {
        public string Name { get; }

        public Label(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}