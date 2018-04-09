using System.Collections.Generic;
using System.Diagnostics;
using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    [DebuggerDisplay("{ToString()}")]
    internal class CodeLog
    {
        private readonly List<Line> lines = new List<Line>();

        public CodeLog(IntermediateCode code)
        {
            Code = code;            
        }

        public void AddLine(Line l)
        {
            lines.Add(l);
        }

        public IntermediateCode Code { get; set; }
        public ICollection<Line> Lines => lines.AsReadOnly();

        public override string ToString()
        {
           return $"{Code}";
        }
    }
}