using System.Collections.ObjectModel;

namespace Plotty.CodeGeneration
{
    public class CodeLog : Collection<CodeLogItem>
    {
        public override string ToString()
        {
            return string.Join("\n\n", Items);
        }
    }
}