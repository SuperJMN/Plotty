namespace Plotty.Uwp
{
    public class CLangCodingViewModel : CodingViewModelBase
    {
        protected override string DefaultSourceCode =>
            "{\n\ta =1;\n\tb=2;\n}";

        public override string Name => "C Language";       
    }
}