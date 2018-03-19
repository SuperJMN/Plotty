using System.Collections.Generic;
using CodeGen.Core;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class GenerationResult
    {
        public Dictionary<Reference, int> AddressMap { get; }
        public IList<Line> Lines { get; }

        public GenerationResult(Dictionary<Reference, int> addressMap, IList<Line> lines)
        {
            AddressMap = addressMap;
            Lines = lines;
        }
    }
}