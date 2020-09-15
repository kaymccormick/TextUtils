using System.Windows;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TextUtils
{
    public class ClientRenderRequest
    {
        public string DrawingIdentifier { get; set; }

        public string BrushColor { get; set; }
        public string PenColor { get; set; }
        public double PenThickness { get; set; }
        public Rect Rect { get; set; }
        public string DrawingXaml { get; set; }
    }
}