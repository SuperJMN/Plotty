using System;
using Plotty.Model;

namespace Plotty.Compiler
{
    public static class FormattingExtensions
    {
        public static string GetAssemblySymbol(this Source source)
        {
            switch (source)
            {
                case RegisterSource r:
                    return $"R{r.Register.Id}";

                case ImmediateSource r:
                    return $"#{r.Immediate}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(source));
            }
        }

        public static string GetAssemblySymbol(this Register register)
        {
            return $"R{register.Id}";
        }

        public static string GetAssemblySymbol(this BooleanOperator op)
        {
            if (op == BooleanOperator.Equal)
            {
                return "BEQ";
            }

            if (op == BooleanOperator.LessThan)
            {
                return "BLT";
            }

            if (op == BooleanOperator.LessThanOrEqual)
            {
                return "BLE";
            }

            if (op == BooleanOperator.GreaterThan)
            {
                return "BGT";
            }

            if (op == BooleanOperator.GreaterOrEqual)
            {
                return "BGE";
            }

            if (op == BooleanOperator.Or)
            {
                return "BOR";
            }

            if (op == BooleanOperator.And)
            {
                return "BAND";
            }

            throw new ArgumentOutOfRangeException(nameof(op));
        }


        public static string GetAssemblySymbol(this JumpTarget jumpTarget)
        {
            switch (jumpTarget)
            {
                case LabelTarget target:
                    return $"{target.Label}";

                case SourceTarget target:
                    return $"{target.Target.GetAssemblySymbol()}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(jumpTarget));
            }
        }

        public static string GetAssemblySymbol(this MemoryAddress address)
        {
            switch (address)
            {
                case IndexedAddress ia:
                    return $"{ia.BaseRegister.GetAssemblySymbol()},{ia.Offset.GetAssemblySymbol()}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(address));
            }
        }
    }
}