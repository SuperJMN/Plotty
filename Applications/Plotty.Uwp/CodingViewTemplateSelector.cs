using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Plotty.Uwp
{
    public class CodingViewTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            switch (item)
            {
                case CLangCodingViewModel _:
                    return CLangTemplate;
                case AssemblyCodingViewModel _:
                    return AssemblyTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }

        public DataTemplate CLangTemplate { get; set; }

        public DataTemplate AssemblyTemplate { get; set; }
    }
}