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

        public static string GetAssemblySymbol(this MemoryAddress address)
        {
            switch (address)
            {
                case IndexedAddress ia:
                    return $"{ia.BaseRegister.GetAssemblySymbol()}, {ia.Offset.GetAssemblySymbol()}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(address));
            }
        }
    }
}