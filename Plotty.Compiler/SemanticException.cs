using System;

namespace Plotty.Compiler
{
    public class SemanticException : Exception
    {
        public SemanticException(string message) : base(message)
        {
        }
    }
}