using System.Linq;
using Plotty.Compiler;
using Plotty.Model;

namespace Plotty.Uwp
{
    public class CLangCodingViewModel : CodingViewModelBase
    {
        protected override string DefaultSourceCode =>
            "{\n\ta =1;\n\tb=2;\n}";

        protected override Line[] GeneratePlottyInstructions(string source)
        {
            return new PlottyCompiler().Compile(source).Code.ToArray();
        }

        public override string Name => "C Language";

        public string AsmSource { get; set; }
    }
}