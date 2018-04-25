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
        public GenerationResult Generate(IEnumerable<IntermediateCode> intermediateCodes, SymbolTable symbolTable)
        {
            var generationVisitor = new PlottyCodeGenerationVisitor(symbolTable, (lines, fixups, sr, br) =>  new Emitter(lines, fixups, sr, br));

            foreach (var x in intermediateCodes)
            {
                x.Accept(generationVisitor);
            }

            var generatedLines = generationVisitor.Lines.ToList();
            PostProcess(generatedLines, generationVisitor.Fixups);

            return new GenerationResult(generatedLines);
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