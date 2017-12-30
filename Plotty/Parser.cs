using Superpower;
using Superpower.Parsers;

namespace Plotty
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
            from first in Register
            from comma in Token.EqualTo(AsmToken.Comma)
            from second in Source
            from third in Register.OptionalOrDefault()
            select (Instruction)new ArithmeticInstruction()
            {
                Source = first,
                Addend = second,
                Destination = third ?? first,
            };

        private static readonly TextParser<string> RegisterParser = Character.EqualTo('R')
            .IgnoreThen(Character.Digit.AtLeastOnce())
            .Select(c => new string(c)); 

        private static TokenListParser<AsmToken, Register> Register =
            from r in Token.EqualTo(AsmToken.Register).Apply(RegisterParser)
            select new Register(int.Parse(r));

        public static readonly TokenListParser<AsmToken, JumpTarget> LabelTarget =
            from text in Token.EqualTo(AsmToken.Text)
            select new JumpTarget(text.ToStringValue());

        public static readonly TokenListParser<AsmToken, JumpTarget> RelativeTarget =
            from number in Number
            select new JumpTarget(number);

        public static readonly TokenListParser<AsmToken, JumpTarget> JumpTarget =
            LabelTarget.Or(RelativeTarget);

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

        public static readonly TokenListParser<AsmToken, Instruction> Action = 
            from wh in Token.EqualTo(AsmToken.Whitespace).OptionalOrDefault()
            from ins in Add.Or(Move).Or(Branch).Or(Halt)
            select ins;

        public static readonly TokenListParser<AsmToken, Line> Line =
            from label in InstructionLabel.OptionalOrDefault()
            from instruction in Action
            select new Line(label, instruction);
            
        public static readonly TokenListParser<AsmToken, Line[]> AsmParser = 
            Line.ManyDelimitedBy(Token.EqualTo(AsmToken.NewLine));
    }
}