using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace TextUtils
{
    public abstract class GenericGraphicsElement : Control
    {
        public static readonly DependencyProperty ViewportElementLeftProperty = LayoutHelper.ViewportElementLeftProperty.AddOwner(typeof(GenericGraphicsElement));
        public static readonly DependencyProperty ViewportElementTopProperty = LayoutHelper.ViewportElementTopProperty.AddOwner(typeof(GenericGraphicsElement));
        private Color _debugColor;

        public double ViewportElementLeft
        {
            get => (double)GetValue(ViewportElementLeftProperty);
            set => SetValue(ViewportElementLeftProperty, value);
        }

        public double ViewportElementTop
        {
            get => (double)GetValue(ViewportElementTopProperty);
            set => SetValue(ViewportElementTopProperty, value);
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var p = VisualParent;
            while (p != null && !(p is DesignSurface))
            {
                p = VisualTreeHelper.GetParent(p);
            }

            DesignSurface = (DesignSurface?) p;
        }

        public DesignSurface? DesignSurface { get; set; }
        public object? Dragging { get; set; }
        public object? Hover { get; set; }
        public bool Completed { get; set; }
        public abstract Drawing AsDrawing();
        public int ElementNum { get; set; }

        public Color DebugColor
        {
            get => _debugColor;
            set
            {
                _debugColor = value;
                DebugBrush = new SolidColorBrush(value);
            }
        }

        public Pen StrokePen { get; set; }

        public abstract Visual? GetVisual(int param);

        public SolidColorBrush DebugBrush { get; set; }

        public void DrawDebugPoint(Point p)
        {
            var d = new GeometryDrawing(DebugBrush, null, new EllipseGeometry(p, 3, 3));
            ProtoLogger2.Instance.LogAction(JsonSerializer.Serialize(new ClientRenderRequest
            { DrawingXaml = XamlWriter.Save(d), DrawingIdentifier = "new1" }));

        }
    }
}