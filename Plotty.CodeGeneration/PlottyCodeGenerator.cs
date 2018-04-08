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
        public GenerationResult Generate(List<IntermediateCode> intermediateCodes, Scope scope)
        {
            var generationVisitor = new PlottyCodeGenerationVisitor(scope);
            intermediateCodes.ForEach(x => x.Accept(generationVisitor));
            var generateLines = generationVisitor.Lines.ToList();

            PostProcess(generateLines, generationVisitor.Fixups);
            
            return new GenerationResult(generateLines);
        }

        private static void Fix(IList<Line> lines, IEnumerable<PendingFixup> generationVisitorFixups)
        {
            foreach (var fixup in generationVisitorFixups)
            {
                fixup.LineFixer.Fix(fixup.Line, lines);
            }
        }

        private static void PostProcess(IList<Line> finalCode, ReadOnlyCollection<PendingFixup> fixups)
        {
            AttachLabelsToInstructions(finalCode);
            Fix(finalCode, fixups);
            GiveNameToUnnamedLabels(finalCode);
        }

        private static void GiveNameToUnnamedLabels(IEnumerable<Line> finalCode)
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

        private static void AttachLabelsToInstructions(IList<Line> finalCode)
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