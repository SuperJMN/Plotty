using System.Linq;
using Plotty.Compiler;
using Plotty.VirtualMachine;

namespace Plotty.Console
{
    internal static class Program
    {
        private static void Main()
        {
            var result = new PlottyCompiler().Compile("{ a=123; b = a; }");

            var machine = new PlottyMachine();
            machine.Load(result.GenerationResult.Lines.ToList());

            var code = string.Join("\n", result.Code);

            while (machine.CanExecute)
            {
                machine.Execute();
            }              
        }
    }
}