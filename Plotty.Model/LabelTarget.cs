namespace Plotty.Model
{
    public class LabelTarget : JumpTarget
    {
        public Label Label { get; }
        
        public LabelTarget(Label label)
        {
            Label = label;
        }


        public override string ToString()
        {
            return $"Label '{Label}'";
        }
    }
}