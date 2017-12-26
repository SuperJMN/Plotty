namespace Plotty
{
    public class JumpTarget
    {
        public string Label { get; }
        public int Number { get; }


        public JumpTarget(string target)
        {
            Label = target;
        }

        public JumpTarget(int number)
        {
            Number = number;
        }
    }
}