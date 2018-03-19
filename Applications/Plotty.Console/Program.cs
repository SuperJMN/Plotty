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
            machine.Load(result.GenerationResult.Code.ToList());

            var code = string.Join("\n", result.Lines);

            while (machine.CanExecute)
            {
                machine.Execute();
            }              
        }
    }
}