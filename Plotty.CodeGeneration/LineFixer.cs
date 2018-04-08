using System.Collections.Generic;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public abstract class LineFixer
    {
        public abstract void Fix(Line toFix, IList<Line> compilationResult);
    }
}