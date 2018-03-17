using Plotty.Model;
using Superpower;
using Superpower.Parsers;

namespace Plotty.Parser
{
    public class Parser
    {
        public static readonly TokenListParser<AsmToken, int> Number =
            Token.EqualTo(AsmToken.Number).Apply(Numerics.IntegerInt32);

        public static readonly TokenListParser<AsmToken, Label> InstructionLabel =
            from text in Token.EqualTo(AsmToken.Text)
            from colon in Token.EqualTo(AsmToken.Colon)
            from white in Token.EqualTo(AsmToken.Whitespace).OptionalOrDefault()
            select new Label(text.ToStringValue());

        public static readonly TokenListParser<AsmToken, Source> ImmediateSource =
            from token in Token.EqualTo(AsmToken.Hash)
            from number in Number
            select (Source)new ImmediateSource(number);

        public static readonly TokenListParser<AsmToken, Source> RegisterSource =
            from reg in Parse.Ref(() => Register)
            select (Source)new RegisterSource(reg);

        public static readonly TokenListParser<AsmToken, Source> Source =
            ImmediateSource.Or(RegisterSource);

        public static readonly TokenListParser<AsmToken, Instruction> Load =
            from keyword in Token.EqualTo(AsmToken.Load)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from destination in Register
            from comma in Token.EqualTo(AsmToken.Comma)
            from memoryAddress in MemoryAddress
            select (Instruction)new LoadInstruction
            {
                MemoryAddress = memoryAddress,
                Destination = destination,
            };

        public static readonly TokenListParser<AsmToken, MemoryAddress> IndexedAddress =
            from baseRegister in Parse.Ref(() => Register)
            from comma in  Token.EqualTo(AsmToken.Comma) 
            from offset in Source
            select (MemoryAddress)new IndexedAddress(baseRegister, offset);

        public static readonly TokenListParser<AsmToken, MemoryAddress> MemoryAddress =
            IndexedAddress;

        public static readonly TokenListParser<AsmToken, Instruction> Store =
            from keyword in Token.EqualTo(AsmToken.Store)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from source in Source
            from comma in Token.EqualTo(AsmToken.Comma)
            from address in MemoryAddress
            select (Instruction)new StoreInstruction
            {
                Source = source,
                Address = address,
            };

        public static readonly TokenListParser<AsmToken, Instruction> Move =
            from keyword in Token.EqualTo(AsmToken.Move)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from destination in Register
            from comma in Token.EqualTo(AsmToken.Comma)
            from source in Source
            select (Instruction)new MoveInstruction
            {
                Destination = destination,
                Source = source,
            };

        public static readonly TokenListParser<AsmToken, Instruction> Halt =
            from keyword in Token.EqualTo(AsmToken.Halt)
            select (Instruction)new HaltInstruction();

        public static readonly TokenListParser<AsmToken, Instruction> Add =
            from keyword in Token.EqualTo(AsmToken.Add)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from source in Register
            from comma in Token.EqualTo(AsmToken.Comma)
            from addend in Source
            from destination in (from cm in Token.EqualTo(AsmToken.Comma) from reg in Register select reg).OptionalOrDefault()
            select (Instruction)new ArithmeticInstruction
            {
                Source = source,
                Addend = addend,
                Destination = destination ?? source,
            };

        private static readonly TextParser<string> RegisterParser = Character.EqualTo('R')
            .IgnoreThen(Character.Digit.AtLeastOnce())
            .Select(c => new string(c));

        private static readonly TokenListParser<AsmToken, Register> Register =
            from r in Token.EqualTo(AsmToken.Register).Apply(RegisterParser)
            select new Register(int.Parse(r));

        public static readonly TokenListParser<AsmToken, JumpTarget> LabelTarget =
            from text in Token.EqualTo(AsmToken.Text)
            select (JumpTarget)new LabelTarget(text.ToStringValue());

        public static readonly TokenListParser<AsmToken, JumpTarget> RegisterTarget =
            from number in Source
            select (JumpTarget)new SourceTarget(number);

        public static readonly TokenListParser<AsmToken, JumpTarget> JumpTarget =
            LabelTarget.Or(RegisterTarget);

        public static readonly TokenListParser<AsmToken, Instruction> Branch =
            from token in Token.EqualTo(AsmToken.Branch)
            from white in Token.EqualTo(AsmToken.Whitespace)
            from r1 in Register
            from c1 in Token.EqualTo(AsmToken.Comma)
            from r2 in Register
            from c2 in Token.EqualTo(AsmToken.Comma)
            from target in JumpTarget
            select (Instruction)new BranchInstruction
            {
                One = r1,
                Another = r2,
                Target = target,
            };

        public static readonly TokenListParser<AsmToken, Instruction> Instruction =
            from wh in Token.EqualTo(AsmToken.Whitespace).OptionalOrDefault()
            from ins in Add.Or(Move).Or(Branch).Or(Halt).Or(Load).Or(Store)
            select ins;

        public static readonly TokenListParser<AsmToken, Line> Line =
            from label in InstructionLabel.OptionalOrDefault()
            from instruction in Instruction
            select new Line(label, instruction);

        public static readonly TokenListParser<AsmToken, Line[]> AsmParser =
            Line.ManyDelimitedBy(Token.EqualTo(AsmToken.NewLine));
    }
}