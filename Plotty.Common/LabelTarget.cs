namespace Plotty.Common
{
    public class LabelTarget : JumpTarget
    {
        public string Label { get; }
        
        public LabelTarget(string label)
        {
            Label = label;
        }


        public override string ToString()
        {
            return $"Label '{Label}'";
        }
    }
}