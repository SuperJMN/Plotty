using System;
using System.Collections.Generic;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using Plotty.Model;
using Plotty.VirtualMachine;

namespace Plotty.CodeGeneration.Tests
{
    public partial class PlottyCodeGeneratorSpecs
    {
        private class MachineFixture
        {
            private readonly PlottyCodeGenerator generator;

            public MachineFixture()
            {
                Machine = new PlottyMachine();
                generator = new PlottyCodeGenerator();
            }

            public void Run(IEnumerable<IntermediateCode> ic, Scope scope, IDictionary<int, int> registersState, IDictionary<int, int> memoryState, Action<List<ILine>> fix = null)
            {
                var result = generator.Generate(ic, scope);

                fix?.Invoke(result.Lines);

                Machine.Load(result.Lines);

                foreach (var i in registersState)
                {
                    Machine.Registers[i.Key] = i.Value;
                }

                foreach (var i in memoryState)
                {
                    Machine.Memory[i.Key] = i.Value;
                }

                while (Machine.CanExecute)
                {
                    Machine.Execute();
                }
            }

            public PlottyMachine Machine { get; }
        }
    }
}