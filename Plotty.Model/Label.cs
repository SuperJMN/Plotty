namespace Plotty.Model
{
    public class Label
    {
        public string Name { get; set; }

        public Label()
        {            
        }

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