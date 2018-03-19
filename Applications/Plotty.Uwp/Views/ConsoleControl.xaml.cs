using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Plotty.Uwp.Views
{
    public sealed partial class ConsoleControl
    {
        public ConsoleControl()
        {
            InitializeComponent();
            CanvasControl.Draw += CanvasControlOnDraw;

            var p = new CanvasDevice();
            var format = new CanvasTextFormat();
            var layout = new CanvasTextLayout(p, "Q", format, 0F, 0F);
            CharWidth = layout.DrawBounds.Width;
            CharHeight = layout.DrawBounds.Height;
            TextColor = Color.FromArgb(255, 255, 255, 255);
        }

        private void CanvasControlOnDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            for (var y = 0; y < CharMatrix.GetUpperBound(1); y++)
            {
                var finalY = (float)(y * CharHeight);

                for (var x = 0; x < CharMatrix.GetUpperBound(0); x++)
                {
                    var text = new String(CharMatrix[x, y], 1);
                    var position = new Vector2((float)(x * CharWidth), finalY);
                    args.DrawingSession.DrawText(text, position, TextColor);
                }
            }
        }

        public static readonly DependencyProperty CharMatrixProperty = DependencyProperty.Register(
            "CharMatrix", typeof(char[,]), typeof(ConsoleControl), new PropertyMetadata(default(char[,]), CharMatrixChanged));

        private double CharHeight;
        public Color TextColor { get; }
        public double CharWidth { get; }

        private static void CharMatrixChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (ConsoleControl) dependencyObject;
            target.CanvasControl.Invalidate();
        }

        public char[,] CharMatrix
        {
            get { return (char[,])GetValue(CharMatrixProperty); }
            set { SetValue(CharMatrixProperty, value); }
        }
    }
}
