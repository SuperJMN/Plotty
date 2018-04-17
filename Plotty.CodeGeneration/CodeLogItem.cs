using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    [DebuggerDisplay("{ToString()}")]
    public class CodeLogItem
    {
        private readonly List<Line> lines = new List<Line>();

        public CodeLogItem(IntermediateCode code)
        {
            Code = code;
        }

        public void AddLine(Line l)
        {
            lines.Add(l);
        }

        public IntermediateCode Code { get; }
        public ICollection<Line> Lines => lines.AsReadOnly();

        public override string ToString()
        {
            var items = Lines.Select(line => $"\t{line}");

            return $"{Code}:\n{string.Join("\n", items)}";
        }
    }
}