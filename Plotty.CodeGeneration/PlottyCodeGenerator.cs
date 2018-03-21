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

            var generateCore = GenerateCore(intermediateCodes, addressMap).ToList();

            generateCore.Add(new Line(new HaltInstruction()));

            for (int i = 0; i < generateCore.Count - 1; i++)
            {
                if (generateCore[i].Label != null)
                {
                    generateCore[i] = new Line(generateCore[i].Label, generateCore[i + 1].Instruction);
                    generateCore.RemoveAt(i + 1);
                }
            }

            return new GenerationResult(addressMap, generateCore.ToList());
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

                        yield return MoveImmediate(addressMap[code.Left], new Register(0));
                        yield return Load(new Register(1), new Register(0));

                        yield return MoveImmediate(addressMap[code.Right], new Register(0));
                        yield return Load(new Register(2), new Register(0));

                        yield return MoveImmediate(addressMap[code.Target], new Register(0));

                        yield return new Line(new ArithmeticInstruction()
                        {
                            Operator = code.Operation == OperationKind.Add ? Operators.Add : Operators.Substract,
                            Left = new Register(1),
                            Right = new RegisterSource(new Register(2)),
                            Destination = new Register(1),
                        });

                        yield return Store(new Register(1), new Register(0));

                        break;

                    case BoolExpressionAssignment code:

                        if (code.Operation == BooleanOperation.IsEqual)
                        {
                            yield return MoveImmediate(addressMap[code.Left], new Register(0));
                            yield return Load(new Register(1), new Register(0));

                            yield return MoveImmediate(addressMap[code.Right], new Register(0));
                            yield return Load(new Register(2), new Register(0));

                            yield return MoveImmediate(addressMap[code.Target], new Register(0));

                            yield return new Line(new ArithmeticInstruction()
                            {
                                Left = new Register(1),
                                Right = new RegisterSource(new Register(2)),
                                Destination = new Register(1),
                            });
                        }

                        break;

                    case JumpIfFalse code:

                        yield return MoveImmediate(0, new Register(0));
                        yield return Load(new Register(1), new Register(0));

                        yield return MoveImmediate(0, new Register(0));
                        
                        yield return new Line(new BranchInstruction()
                        {
                            Target = new LabelTarget(code.Label.Name),
                            One = new Register(1),
                            Another = new Register(0),
                        });

                        break;
                    case LabelCode code:
                        yield return new Line(new Model.Label(code.Label.Name), null);

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