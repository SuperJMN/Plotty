using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeGen.Intermediate.Codes;
using CodeGen.Parsing.Ast;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class PlottyCodeGenerator
    {
        public GenerationResult Generate(IEnumerable<IntermediateCode> intermediateCodes, Scope scope)
        {
            var generationVisitor = new PlottyCodeGenerationVisitor(scope);

            foreach (var x in intermediateCodes)
            {
                x.Accept(generationVisitor);
            }

            var lines = generationVisitor.Lines.ToList();
            PostProcess(lines, generationVisitor.Fixups);

            return new GenerationResult(lines);
        }

        private static void Fix(IList<ILine> lines, IEnumerable<PendingFixup> generationVisitorFixups)
        {
            foreach (var fixup in generationVisitorFixups)
            {
                fixup.LineFixer.Fix(fixup.Line, lines);
            }
        }

        private static void PostProcess(IList<ILine> finalCode, ReadOnlyCollection<PendingFixup> fixups)
        {
            AttachLabelsToInstructions(finalCode);
            Fix(finalCode, fixups);
            GiveNameToUnnamedLabels(finalCode);
        }

        private static void GiveNameToUnnamedLabels(IEnumerable<ILine> finalCode)
        {
            int count = 0;
            foreach (var line in finalCode)
            {
                if (line.Label != null && line.Label.Name == null)
                {
                    line.Label.Name = $"dyn_label{++count}";
                }
            }
        }

        private static void AttachLabelsToInstructions(IList<ILine> finalCode)
        {
            int i = 0;

            while (i < finalCode.Count - 1)
            {
                var nakedLabel = finalCode[i].Label != null && finalCode[i].Instruction == null;

                if (nakedLabel)
                {
                    finalCode[i] = new Line(finalCode[i].Label, finalCode[i + 1].Instruction);
                    finalCode.RemoveAt(i + 1);                 
                }
                else
                {
                    i++;
                }
            }          
        }
    }
}