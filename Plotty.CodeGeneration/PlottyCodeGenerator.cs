using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate;
using CodeGen.Intermediate.Codes;
using Plotty.Model;

namespace Plotty.CodeGeneration
{
    public class PlottyCodeGenerator
    {
        public GenerationResult Generate(List<IntermediateCode> intermediateCodes)
        {
            var addressMap = CreateReferenceToAddressMap(intermediateCodes);
            var lines = GenerateLines(intermediateCodes, addressMap);
            PostProcess(lines);

            return new GenerationResult(addressMap, lines.ToList());
        }

        private static List<Line> GenerateLines(List<IntermediateCode> intermediateCodes, IDictionary<Reference, int> addressMap)
        {
            var generationVisitor = new PlottyCodeGenerationVisitor(addressMap);

            intermediateCodes.ForEach(x => x.Accept(generationVisitor));

            return generationVisitor.Lines.ToList();
        }

        private static Dictionary<Reference, int> CreateReferenceToAddressMap(List<IntermediateCode> intermediateCodes)
        {
            var namedObjectCollector = new NamedObjectCollector();
            intermediateCodes.ForEach(x => x.Accept(namedObjectCollector));

            var addressMap = namedObjectCollector.References
                .Distinct()
                .Select((reference, index) => new {Reference = reference, Index = index})
                .ToDictionary(key => key.Reference, value => value.Index);
            return addressMap;
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

            void GiveName(Model.Label label)
            {
                label.Name = $"dyn_label{++count}";
            }

            var unnamed = finalCode.Where(x => x.Label != null && x.Label.Name == null).ToList();
            unnamed.ForEach(x => GiveName(x.Label));
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