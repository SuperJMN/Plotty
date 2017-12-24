using System.Collections.Generic;
using System.Linq;

namespace Plotty
{
    public class Command
    {
        public OpCodes OpCode { get; set; }
        public Register FirstRegister => Registers.First();
        public int Address { get; set; }
        public IList<Register> Registers { get; set; }
    }
}