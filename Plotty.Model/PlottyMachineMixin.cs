using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;

namespace Plotty.Model
{
    public static class PlottyMachineMixin
    {
        public static IEnumerable<MemoryState> GetMemoryState(this IPlottyMachine machine, IDictionary<Reference, int> addressMap)
        {
            return addressMap.Select((x, i) =>
            {
                var value = machine.Memory[addressMap[x.Key]];
                return new MemoryState(i, x.Key, value);
            });
        }
    }
}