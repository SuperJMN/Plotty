using System;
using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate;
using CodeGen.Intermediate.Codes;
using CodeGen.Intermediate.Codes.Common;
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
                    case IntegerConstantAssignment code:

                        yield return MoveImmediate(addressMap[code.Target], new Register(0));
                        yield return MoveImmediate(code.Value, new Register(1));
                        yield return Store(new Register(1), new Register(0));

                        break;

                    case ReferenceAssignment code:

                        yield return MoveImmediate(addressMap[code.Origin], new Register(0));
                        yield return Load(new Register(1), new Register(0));
                        yield return MoveImmediate(addressMap[code.Target], new Register(0));
                        yield return Store(new Register(1), new Register(0));

                        break;

                    case OperationAssignment code:

                        if (code.Operation != OperationKind.Add)
                        {
                            throw new InvalidOperationException();
                        }

                        yield return MoveImmediate(addressMap[code.Left], new Register(0));
                        yield return Load(new Register(1), new Register(0));

                        yield return MoveImmediate(addressMap[code.Right], new Register(0));
                        yield return Load(new Register(2), new Register(0));

                        yield return MoveImmediate(addressMap[code.Target], new Register(0));

                        yield return new Line(new AddInstruction()
                        {
                            Source = new Register(1),
                            Addend = new RegisterSource(new Register(2)),
                            Destination = new Register(1),
                        });

                        yield return Store(new Register(1), new Register(0));

                        break;
                }
            }
        }

        private static Line Store(Register register, Register baseRegister, int offset = 0)
        {
            return new Line(new StoreInstruction
            {
                Source = new RegisterSource(register),
                MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
            });
        }

        private static Line Load(Register register, Register baseRegister, int offset = 0)
        {
            return new Line(new LoadInstruction
            {
                Destination = register,
                MemoryAddress = new IndexedAddress(baseRegister, new ImmediateSource(offset))
            });
        }

        private static Line MoveImmediate(int immediate, Register destination)
        {
            return new Line(new MoveInstruction
            {
                Destination = destination,
                Source = new ImmediateSource(immediate),
            });
        }
    }
}