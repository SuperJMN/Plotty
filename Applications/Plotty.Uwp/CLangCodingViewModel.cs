using System.Collections.Generic;
using System.Linq;
using Plotty.Compiler;
using Plotty.Model;
using ReactiveUI;

namespace Plotty.Uwp
{
    public class CLangCodingViewModel : CodingViewModelBase
    {
        private string asmSource;

        protected override string DefaultSourceCode =>
            "{\n\ta =1;\n\tb=2;\n}";

        protected override IEnumerable<Line> GeneratePlottyInstructions(string source)
        {
            var instructions = new PlottyCompiler().Compile(source).Code.ToArray();
            return instructions;
        }

        public override string Name => "C Language";

        public string AsmSource
        {
            get => asmSource;
            set
            {
                asmSource = value;
                this.RaiseAndSetIfChanged(ref asmSource, value);
            }
        }
    }
}