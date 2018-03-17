using System.Collections.Generic;
using System.Text;
using Superpower;
using Superpower.Model;
using Superpower.Parsers;

namespace Plotty.Parser
{
    public class Tokenizer : Tokenizer<AsmToken>
    {
        private readonly IDictionary<char, AsmToken> charToTokenDict =
            new Dictionary<char, AsmToken>()
            {
                {':', AsmToken.Colon},
                {'+', AsmToken.Plus},
                {'#', AsmToken.Hash},
                {',', AsmToken.Comma},
                {'\n', AsmToken.NewLine},
            };

        private readonly IDictionary<string, AsmToken> words = new Dictionary<string, AsmToken>()
        {
            {"MOVE", AsmToken.Move},
            {"LOAD", AsmToken.Load},
            {"STORE", AsmToken.Store},
            {"ADD", AsmToken.Add},
            {"BRANCH", AsmToken.Branch},
            {"HALT", AsmToken.Halt}
        };

        protected override IEnumerable<Result<AsmToken>> Tokenize(TextSpan span)
        {
            var filtered = new TextSpan(span.Source.Replace("\r\n", "\n"));
            var cursor = SkipWhiteSpace(filtered);

            do
            {
                if (cursor.Value == 'R')
                {
                    var regNum = Numerics.Integer(cursor.Remainder);
                    yield return Result.Value(AsmToken.Register, cursor.Location, regNum.Remainder);
                    cursor = regNum.Remainder.ConsumeChar();
                }
                else if (charToTokenDict.TryGetValue(cursor.Value, out var token))
                {
                    yield return Result.Value(token, cursor.Location, cursor.Remainder);
                    cursor = cursor.Remainder.ConsumeChar();
                }
                else if (char.IsWhiteSpace(cursor.Value))
                {
                    yield return Result.Value(AsmToken.Whitespace, cursor.Location, cursor.Remainder);
                    cursor = SkipWhiteSpace(cursor.Remainder);
                }
                else if (char.IsDigit(cursor.Value))
                {
                    var integer = Numerics.Integer(cursor.Location);
                    yield return Result.Value(AsmToken.Number, integer.Location, integer.Remainder);
                    cursor = integer.Remainder.ConsumeChar();
                }
                else if (char.IsLetter(cursor.Value))
                {
                    var keywordBuilder = new StringBuilder();
                    var start = cursor.Location;
                    keywordBuilder.Append(cursor.Value);

                    do
                    {
                        cursor = cursor.Remainder.ConsumeChar();

                        if (cursor.HasValue && char.IsLetter(cursor.Value))
                        {
                            keywordBuilder.Append(cursor.Value);
                        }
                    } while (!words.Keys.Contains(keywordBuilder.ToString()) && cursor.HasValue &&
                             char.IsLetter(cursor.Value));

                    if (cursor.HasValue && char.IsLetter(cursor.Value))
                    {
                        cursor = cursor.Remainder.ConsumeChar();
                    }

                    var keyword = keywordBuilder.ToString();

                    if (words.Keys.Contains(keyword))
                    {
                        yield return Result.Value(words[keyword], start, cursor.Location);
                    }
                    else
                    {
                        yield return Result.Value(AsmToken.Text, start, cursor.Location);
                    }
                }
                else
                {
                    yield return Result.Empty<AsmToken>(cursor.Location, "Unexpected token");
                }

            } while (cursor.HasValue);
        }
    }
}
