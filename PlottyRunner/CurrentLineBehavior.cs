using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Microsoft.Xaml.Interactivity;
using Monaco;
using Monaco.Editor;
using Monaco.Helpers;

namespace Plotty.Uwp
{
    public class CurrentLineBehavior : Behavior<CodeEditor>
    {
        public static readonly DependencyProperty CurrentLineProperty = DependencyProperty.Register(
            "CurrentLine", typeof(int), typeof(CurrentLineBehavior), new PropertyMetadata(default(int), PropertyChangedCallback));


        private IModelDeltaDecoration previousLineDecoration;

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (CurrentLineBehavior) dependencyObject;
            var previousLine = Convert.ToUInt32(dependencyPropertyChangedEventArgs.OldValue);
            var newLine = Convert.ToUInt32(dependencyPropertyChangedEventArgs.NewValue);
            target.PropertyChangedCallback(previousLine, newLine);
        }

        private void PropertyChangedCallback(uint previousLine, uint newLine)
        {          
            AssociatedObject.Decorations.Remove(previousLineDecoration);

            AddDecoration(newLine);
        }

        private void AddDecoration(uint line)
        {
            var lineDecoration = new IModelDeltaDecoration(new Range(line, 1, line, 1), new IModelDecorationOptions()
            {
                IsWholeLine = true,
                ClassName = new CssLineStyle
                {
                    BackgroundColor = LineBrush
                }
            });

            AssociatedObject.Decorations.Add(lineDecoration);
            previousLineDecoration = lineDecoration;
        }

        public int CurrentLine
        {
            get { return (int) GetValue(CurrentLineProperty); }
            set { SetValue(CurrentLineProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(CurrentLineBehavior), new PropertyMetadata(default(bool), IsEnabledChanged ));

        private static void IsEnabledChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (CurrentLineBehavior)dependencyObject;
            var previous = (bool)dependencyPropertyChangedEventArgs.OldValue;
            var next = (bool)dependencyPropertyChangedEventArgs.NewValue;
            target.IsEnabledChanged(previous, next);
        }

        private void IsEnabledChanged(bool oldIsEnabled, bool newIsEnabled)
        {
            if (!newIsEnabled)
            {
                AssociatedObject.Decorations.Remove(previousLineDecoration);
            }
            else if (AssociatedObject.Decorations.All(x => x.Range.StartLineNumber != CurrentLine))
            {
                AddDecoration(Convert.ToUInt32(CurrentLine));
            }
        }

        public bool IsEnabled
        {
            get { return (bool) GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty LineBrushProperty = DependencyProperty.Register(
            "LineBrush", typeof(SolidColorBrush), typeof(CurrentLineBehavior), new PropertyMetadata(new SolidColorBrush(Colors.Red)));

        public SolidColorBrush LineBrush
        {
            get { return (SolidColorBrush) GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }
    }
}