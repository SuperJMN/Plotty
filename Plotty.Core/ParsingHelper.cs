using Superpower.Model;
using Superpower.Parsers;

namespace Plotty.Parser
{
    public class ParsingHelper
    {
        public static bool TryParseInteger(Result<char> next, out Result<TextSpan> result)
        {
            result = default(Result<TextSpan>);
            if (char.IsDigit(next.Value))
            {
                var integer = Numerics.Integer(next.Location);

                result = integer;
                return true;
            }

            return false;
        }
    }
}