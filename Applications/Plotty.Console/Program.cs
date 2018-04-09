using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Plotty.Compiler;
using Plotty.Model;
using Plotty.VirtualMachine;

namespace Plotty.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<RunOptions, ShowOptions>(args)
                .WithParsed<RunOptions>(opts => Run(Compile(opts)) )
                .WithParsed<ShowOptions>(opts => PrintCode(Compile(opts).Code))
                .WithNotParsed(HandleParseError);           
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            System.Console.WriteLine($"Cannot parse the specified arguments: {string.Join(",", errs)}");
        }

        private static CompilationResult Compile(Options opts)
        {
            var source = GetSource(opts);
            return new PlottyCompiler().Compile(source);
        }

        private static void Run(CompilationResult result)
        {
            var machine = new PlottyMachine();
            machine.Load(result.GenerationResult.Lines.ToList());
            while (machine.CanExecute)
            {
                machine.Execute();
            }

            System.Console.WriteLine();
            //ShowMachineState(machine, result);
        }

        private static void ShowMachineState(PlottyMachine machine, CompilationResult result)
        {
            System.Console.WriteLine("Memory References:");

            var state = machine.GetMemoryState(null);

            var taggedReferences = state.Select(x => $"[{x.Index}]\t{x.Reference}={x.Value}");
            
            System.Console.WriteLine(string.Join("\n", taggedReferences));
            System.Console.WriteLine();
        }

        private static void PrintCode(IEnumerable<string> readOnlyCollection)
        {
            var taggedLines = readOnlyCollection.Select((s, i) => $"{i}\t{s}");
            System.Console.WriteLine(string.Join("\n", taggedLines));
        }

        private static string GetSource(Options opts)
        {
            var source = File.ReadAllText(opts.File);
            return source;
        }

        class Options
        {
            [Option('f', "file", HelpText = "Source file", Required = true)]
            public string File { get; set; }
        }

        [Verb("run")]
        class RunOptions : Options
        {           
        }

        [Verb("show")]
        class ShowOptions : Options
        {
        }
    }
}