using System.Collections.Generic;
using Plotty.Assembly;
using Superpower;
using Superpower.Parsers;

namespace Plotty
{
    public class Parser
    {
        public static readonly TokenListParser<AsmToken, int> Number =
            Token.EqualTo(AsmToken.Number).Apply(Numerics.IntegerInt32);

        public static readonly TokenListParser<AsmToken, Instruction> Load =
            from keyword in Token.EqualTo(AsmToken.Load)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from register in Register            
            from comma in Token.EqualTo(AsmToken.Comma)
            from address in Number
            select new Instruction()
            {
                OpCode = OpCodes.Load,
                Registers = new List<Register>() {register},
                Address = address,
            };

        public static TokenListParser<AsmToken, Instruction> Store =
            from keyword in Token.EqualTo(AsmToken.Load)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from register in Register            
            from comma in Token.EqualTo(AsmToken.Comma)
            from address in Number
            select new Instruction()
            {
                OpCode = OpCodes.Store,
                Registers = new List<Register>() {register},
                Address = address,
            };

        public static TokenListParser<AsmToken, Instruction> Add =
            from keyword in Token.EqualTo(AsmToken.Add)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from registers in Registers
            select new Instruction()
            {
                OpCode = OpCodes.Add,
                Registers = registers,
            };

        public static readonly TokenListParser<AsmToken, Register> Register =
            from registerPrefix in Token.EqualTo(AsmToken.Register)
            from number in Number
            select new Register(number);

        public static readonly TokenListParser<AsmToken, Register[]> Registers =
            Register.ManyDelimitedBy(Token.EqualTo(AsmToken.Comma));      
        
        public static readonly TokenListParser<AsmToken, Instruction> Instructions =
            Load.Or(Store).Or(Add);

        public static readonly TokenListParser<AsmToken, Instruction[]> AsmParser =
            Instructions.ManyDelimitedBy(Token.EqualTo(AsmToken.NewLine));
    }
}