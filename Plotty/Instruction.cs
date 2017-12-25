using System.Collections.Generic;
using System.Linq;

namespace Plotty
{
    public class Instruction
    {
        public OpCodes OpCode { get; set; }
        public Register FirstRegister => Registers.First();
        public LoadParam Address { get; set; }
        public IList<Register> Registers { get; set; }      
    }
}