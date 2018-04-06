using System.Collections.Generic;
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
            var lines = GenerateLines(intermediateCodes, scope);
            PostProcess(lines);

            return new GenerationResult(lines);
        }

        private static List<Line> GenerateLines(List<IntermediateCode> intermediateCodes, Scope scope)
        {
            var generationVisitor = new PlottyCodeGenerationVisitor(scope);

            intermediateCodes.ForEach(x => x.Accept(generationVisitor));

            return generationVisitor.Lines.ToList();
        }

        private static void PostProcess(IList<Line> finalCode)
        {
            finalCode.Add(new Line(new HaltInstruction()));

            AttachLabelsToInstructions(finalCode);
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
            for (var i = 0; i < finalCode.Count - 1; i++)
            {
                if (finalCode[i].Label != null)
                {
                    finalCode[i] = new Line(finalCode[i].Label, finalCode[i + 1].Instruction);
                    finalCode.RemoveAt(i + 1);
                }
            }
        }
    }
}