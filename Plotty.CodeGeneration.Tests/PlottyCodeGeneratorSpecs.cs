using System.Collections.Generic;
using System.Linq;
using CodeGen.Core;
using CodeGen.Intermediate.Codes;
using CodeGen.Intermediate.Codes.Common;
using FluentAssertions;
using Plotty.Model;
using Plotty.VirtualMachine;
using Xunit;
using Xunit.Theory.Extended;
using Label = CodeGen.Intermediate.Label;

namespace Plotty.CodeGeneration.Tests
{
    public class PlottyCodeGeneratorSpecs
    {
        [ExtendedTheory]
        [MemberData(nameof(TestData))]
        public void IntermediateToPlottyCode(List<IntermediateCode> intermediateCodes,
            IDictionary<string, int> initialState, IEnumerable<Expectation> expectedValues)
        {
            var sut = new PlottyCodeGenerator();
            var result = sut.Generate(intermediateCodes);

            AssertRun(result, initialState, expectedValues);
        }

        public static IEnumerable<object[]> TestData => Misc.Concat(Arithmetic);

        private static IEnumerable<object[]> Misc
        {
            get
            {
                return new List<object[]>()
                {
                    new object[]
                    {
                        new List<IntermediateCode>()
                        {
                            new BoolExpressionAssignment(BooleanOperation.IsEqual, new Reference("a"), new Reference("b"),
                                new Reference("c"))
                        },
                        new Dictionary<string, int> {{"b", 5}, {"c", 3}},
                        new List<Expectation> {new Expectation("a", 0, Operator.NotEqual)}
                    },
                    new object[]
                    {
                        new List<IntermediateCode>()
                        {
                            new BoolExpressionAssignment(BooleanOperation.IsEqual, new Reference("a"), new Reference("b"),
                                new Reference("c"))
                        },
                        new Dictionary<string, int> {{"b", 123}, {"c", 123}}, new List<Expectation> {new Expectation("a", 0)}
                    },
                    new object[]
                    {
                        new List<IntermediateCode>()
                        {
                            new JumpIfFalse(new Reference("a"), new Label("label1")),
                            new IntegerConstantAssignment(new Reference("b"), 123),
                            new LabelCode(new Label("label1"))
                        },
                        new Dictionary<string, int> {{"a", 1}}, new List<Expectation> {new Expectation("b", 0)}
                    },
                    new object[]
                    {
                        new List<IntermediateCode>()
                        {
                            new JumpIfFalse(new Reference("a"), new Label("label1")),
                            new IntegerConstantAssignment(new Reference("b"), 123),
                            new LabelCode(new Label("label1"))
                        },
                        new Dictionary<string, int> {{"a", 0}}, new List<Expectation> {new Expectation("b", 123)}
                    },
                };
            }
        }

        private static IEnumerable<object[]> Arithmetic
        {
            get
            {
                yield return GetAritmethic(ArithmeticOperator.Add, 2, 3, 5);
                yield return GetAritmethic(ArithmeticOperator.Substract, 10, 2, 8);
                yield return GetAritmethic(ArithmeticOperator.Mult, 3, 4, 12);                
            }
        }

        private static object[] GetAritmethic(ArithmeticOperator add, int op1, int op2, int result)
        {
            return new object[]
            {
                new List<IntermediateCode>
                {
                    new ArithmeticAssignment(add, new Reference("a"), new Reference("b"),
                        new Reference("c"))
                },
                new Dictionary<string, int> {{"b", op1}, {"c", op2}}, new List<Expectation> {new Expectation("a", result)}
            };
        }

        private static void AssertRun(GenerationResult result, IDictionary<string, int> initialState, IEnumerable<Expectation> expectedState)
        {
            var machine = new PlottyMachine();

            machine.Load(result.Lines);

            foreach (var state in initialState)
            {
                var r = new Reference(state.Key);
                var address = result.AddressMap[r];
                machine.Memory[address] = state.Value;
            }

            while (machine.CanExecute)
            {
                machine.Execute();
            }

            foreach (var expectation in expectedState)
            {
                var address = result.AddressMap[new Reference(expectation.RefName)];
                var expectedValue = expectation.Value;

                if (expectation.Operator== Operator.Equal)
                {
                    machine.Memory[address].Should().Be(expectedValue);
                }
                else
                {
                    machine.Memory[address].Should().NotBe(expectedValue);
                }
            }
        }

        public class Expectation
        {
            public Expectation(string refName, int value, Operator @operator = Operator.Equal)
            {
                RefName = refName;
                Value = value;
                Operator = @operator;
            }

            public string RefName { get; }
            public int Value { get; }
            public Operator Operator { get; }
        }

        public enum Operator
        {
            Equal,
            NotEqual,
        }
    }
}
