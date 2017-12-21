using System.Collections.Generic;
using System.Text;
using Superpower;
using Superpower.Model;

namespace Plotty
{
    public class Tokenizer : Tokenizer<PlottyToken>
    {
        private readonly IDictionary<char, PlottyToken> charToTokenDict =
            new Dictionary<char, PlottyToken>()
            {
                {',', PlottyToken.Comma},
                {'@', PlottyToken.At},
                {'-', PlottyToken.Hyphen},
                {'\n', PlottyToken.NewLine},
            };

        private readonly IDictionary<string, PlottyToken> words = new Dictionary<string, PlottyToken>()
            {{"COLOR", PlottyToken.Color}, {"PLOT", PlottyToken.Plot}, {"MOVE", PlottyToken.Move}};

        protected override IEnumerable<Result<PlottyToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            do
            {
                if (next.Value == '\n')
                {
                    yield return Result.Value(PlottyToken.NewLine, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if (char.IsWhiteSpace(next.Value))
                {
                    yield return Result.Value(PlottyToken.Whitespace, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if (char.IsLetter(next.Value))
                {
                    var keywordBuilder = new StringBuilder();
                    var start = next.Location;
                    keywordBuilder.Append(next.Value);

                    do
                    {
                        next = next.Remainder.ConsumeChar();
                        if (char.IsLetter(next.Value))
                        {
                            keywordBuilder.Append(next.Value);
                        }

                    } while (!words.Keys.Contains(keywordBuilder.ToString()) && next.HasValue &&
                             char.IsLetter(next.Value));

                    next = next.Remainder.ConsumeChar();

                    var keyword = keywordBuilder.ToString();

                    if (words.Keys.Contains(keyword))
                    {
                        yield return Result.Value(words[keyword], start, next.Location);
                    }
                    else
                    {
                        yield return Result.Empty<PlottyToken>(start, $"Unexpected keyword {keyword}");
                    }
                }
                else if (charToTokenDict.TryGetValue(next.Value, out var token))
                {
                    yield return Result.Value(token, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if (ParsingHelper.TryParseInteger(next, out var integerSpan))
                {
                    yield return Result.Value(PlottyToken.Number, integerSpan.Location, integerSpan.Remainder);
                    next = integerSpan.Remainder.ConsumeChar();
                }
            } while (next.HasValue);
        }
    }
}
