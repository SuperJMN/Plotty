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
            var visitor = new NamedObjectCollector();
            intermediateCodes.ForEach(x => x.Accept(visitor));

            var addressMap = visitor.References
                .Distinct()
                .Select((reference, index) => new { Reference = reference, Index = index })
                .ToDictionary(key => key.Reference, value => value.Index);

            return new GenerationResult(addressMap, GenerateCore(intermediateCodes, addressMap).ToList());
        }

        private static IEnumerable<Line> GenerateCore(IEnumerable<IntermediateCode> intermediateCodes, Dictionary<Reference, int> addressMap)
        {
            foreach (var intermediateCode in intermediateCodes)
            {
                switch (intermediateCode)
                {
                    case IntegerConstantAssignment ias:

                        yield return new Line(new MoveInstruction
                        {
                            Destination = new Register(1),
                            Source = new ImmediateSource(ias.Value),
                        });
                        yield return new Line(new StoreInstruction()
                        {
                            Source = new RegisterSource(new Register(1)),
                            Address = new IndexedAddress(new Register(0), new ImmediateSource(addressMap[ias.Target]))
                        });

                        break;

                    case ReferenceAssignment ras:

                        yield return new Line(new LoadInstruction()
                        {
                            Destination = new Register(1),
                            MemoryAddress = new IndexedAddress(new Register(0), new ImmediateSource(addressMap[ras.Origin]))
                        });

                        yield return new Line(new StoreInstruction()
                        {
                            Source = new RegisterSource(new Register(1)),
                            Address = new IndexedAddress(new Register(0), new ImmediateSource(addressMap[ras.Target]))
                        });

                        break;
                }
            }
        }
    }
}