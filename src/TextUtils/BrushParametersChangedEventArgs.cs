using System.Windows;
using System.Windows.Media;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TextUtils
{
    public class BrushParametersChangedEventArgs : RoutedEventArgs
    {
        public DrawingVisual1 DrawingVisual1 { get; }
        public DrawingBrush Brush { get; }

        public BrushParametersChangedEventArgs(DrawingVisual1 drawingVisual1, DrawingBrush brush, object sender) : base(DrawingVisual1.BrushParametersChangedEvent,sender)
        {
            DrawingVisual1 = drawingVisual1;
            Brush = brush;
        }
    }
}