using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TextUtils
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextCaret1 : Control
    {
        /// <inheritdoc />
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            // var rect = GetClipRect(LayoutHelper.GetViewportElementLeft(this), LayoutHelper.GetViewportElementTop(this), arrangeBounds);
            // Clip = new RectangleGeometry(rect);
            // Debug.WriteLine("Clip is " + RoslynCodeControl.RoundRect(Clip.Bounds));
            return base.ArrangeOverride(arrangeBounds);
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            var measureOverride = base.MeasureOverride(constraint);

            return new Size(CaretWidth, LineHeight);
            // return measureOverride;
        }

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
            "EndPoint", typeof(Point), typeof(TextCaret1), new PropertyMetadata(default(Point)));

        public Point EndPoint
        {
            get { return (Point) GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public static readonly DependencyProperty LineHeightProperty = LayoutHelper.LineHeightProperty.AddOwner(typeof(TextCaret1),
            new FrameworkPropertyMetadata(default(double),FrameworkPropertyMetadataOptions.AffectsMeasure|FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnLineHeightChanged)));

        public static readonly DependencyProperty ViewportElementLeftProperty =
            LayoutHelper.ViewportElementLeftProperty.AddOwner(typeof(TextCaret1),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsArrange|FrameworkPropertyMetadataOptions.AffectsParentArrange));
        public static readonly DependencyProperty ViewportElementTopProperty =
            LayoutHelper.ViewportElementTopProperty.AddOwner(typeof(TextCaret1),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsArrange));

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var solidColorBrush = Brushes.Black.Clone();
            solidColorBrush.Opacity = 0.2;
            // drawingContext.DrawRectangle(solidColorBrush, new Pen(solidColorBrush, 1.5), new Rect(DesiredSize));
        }

        public double LineHeight
        {
            get { return (double) GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public static readonly DependencyProperty CaretWidthProperty = DependencyProperty.Register(
            "CaretWidth", typeof(double), typeof(TextCaret1), new PropertyMetadata(default(double)));

        public double CaretWidth
        {
            get { return (double) GetValue(CaretWidthProperty); }
            set { SetValue(CaretWidthProperty, value); }
        }
        private static void OnLineHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextCaret1 tc)
            {
                tc.OnLineHeightChanged((double)e.OldValue, (double)e.NewValue);
            }
            
        }


        private void OnLineHeightChanged(double oldValue, double newValue)
        {
            EndPoint = new Point(0,newValue);
            Debug.WriteLine(EndPoint);
        }

        static TextCaret1()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextCaret1),
                new FrameworkPropertyMetadata(typeof(TextCaret1)));
        }


        /// <inheritdoc />
        // protected override Size MeasureOverride(Size constraint)
        // {
        // var measureOverride = base.MeasureOverride(constraint);
        // return new Size(1,LineHeight);
        // return measureOverride;
        // }

        private int _caretWidth = 3;
        private Pen _pen;

        public TextCaret1(double lineHeight)
        {
            this.LineHeight = lineHeight;
            _pen = new Pen(Brushes.Black,  _caretWidth);
        }

        public TextCaret1()
        {
            _pen = new Pen(Brushes.Black,  _caretWidth);
        }

       

        // protected override void OnRender(DrawingContext drawingContext)
        // {
        // base.OnRender(drawingContext);
        // var c = VisualTreeHelper.GetContentBounds(this);
        // drawingContext.DrawLine(_pen, new Point(0, 0), new Point(0, LineHeight));

        // }


        public void SetCaretPosition(in double left, in double top)
        {
            var rect = GetClipRect(left, top, DesiredSize);

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);

            ViewportElementLeft = left;
            ViewportElementTop = top;

            // //Debug.WriteLine("clip rect is " + rect);
            // Clip = new RectangleGeometry(rect);
        }


        public double ViewportElementLeft
        {
            get { return (double)GetValue(ViewportElementLeftProperty); }
            set { SetValue(ViewportElementLeftProperty, value); }
        }

        public double ViewportElementTop
        {
            get { return (double)GetValue(ViewportElementTopProperty); }
            set { SetValue(ViewportElementTopProperty, value); }
        }

        private Rect GetClipRect(double left, double top, Size arrangeBounds)
        {
            double x = 0.0;
            double y = 0.0;
            double width = arrangeBounds.Width;
            var height = arrangeBounds.Height;
            if (left < 0 && left + width >= 0.0)
            {
                x = -1 * left;
                width += left;
            }

            if (top < 0 && top + height >= 0.0)
            {
                y = -1 * top;
                height += top;
            }

            var rect = new Rect(x, y, width, height);
            return rect;
        }
    }
}