namespace Plotty.Uwp
{
    public class CLangCodingViewModel : CodingViewModelBase
    {
        protected override string DefaultSourceCode =>
            "void main()\n{\n\ta=1;\n\tb=2;\n\tcall(a, b);\n}";

        public override string Name => "C Language";       
    }
}