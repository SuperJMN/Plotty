using System.Collections.Generic;

namespace Plotty.Compiler.Tests
{
    public class IntermediateGenerator
    {
        public IEnumerable<Expression> Generate(AssignmentExpression ast)
        {
            yield break;
        }
    }
}