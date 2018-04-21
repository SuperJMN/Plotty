using System;
using CodeGen.Intermediate.Codes.Common;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public static class Mapper
    {
        public static BooleanOperator ToOperator(this BooleanOperation op)
        {
            if (op == BooleanOperation.IsEqual)
                return BooleanOperator.Equal;

            if (op == BooleanOperation.IsLessThan)
                return BooleanOperator.LessThan;
            
            if (op == BooleanOperation.IsGreaterThan)
                return BooleanOperator.GreaterThan;

            if (op == BooleanOperation.IsGreaterOrEqual)
                return BooleanOperator.GreaterOrEqual;

            if (op == BooleanOperation.IsLessOrEqual)
                return BooleanOperator.LessThanOrEqual;

            if (op == BooleanOperation.And)
                return BooleanOperator.And;

            if (op == BooleanOperation.Or)
                return BooleanOperator.Or;

            throw new ArgumentOutOfRangeException();
        }
    }
}