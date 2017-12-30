namespace Plotty
{
    public class JumpTarget
    {
        public string Label { get; }
        public int LineNumber { get; }


        public JumpTarget(string target)
        {
            Label = target;
        }

        public JumpTarget(int lineNumber)
        {
            LineNumber = lineNumber;
        }

        public override string ToString()
        {
            var text = Label != null ? $"Label '{Label}'" : $"Line {LineNumber}";

            return text;
        }
    }
}