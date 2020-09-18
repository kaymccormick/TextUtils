using System;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KimGraphics;

namespace TextUtils
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TextCaret1 : FrameworkElement, IDisposable
    {
        private Guid _guid;

        public double PixelsPerDip { get; set; }

        private readonly DrawServiceClient? _client;
        // ReSharper disable once RedundantOverriddenMember
        protected override unsafe Size ArrangeOverride(Size arrangeBounds)
        {
            // var rect = GetClipRect(LayoutHelper.GetViewportElementLeft(this), LayoutHelper.GetViewportElementTop(this), arrangeBounds);
            // Clip = new RectangleGeometry(rect);
            // Debug.WriteLine("Clip is " + RoslynCodeControl.RoundRect(Clip.Bounds));
            var cb = VisualTreeHelper.GetContentBounds(this);
            VisualBrush v = new VisualBrush((Visual) VisualParent)
            {
                Viewport = cb, ViewportUnits = BrushMappingMode.Absolute, TileMode = TileMode.None,
                Stretch = Stretch.None
            };

            int width = (int) Math.Ceiling(DpiScale1.DpiScaleX * arrangeBounds.Width);
            int height = (int) Math.Ceiling(DpiScale1.DpiScaleY * arrangeBounds.Height);
            double dpiX = DpiScale1.DpiScaleX * 72;
            double dpiY = DpiScale1.DpiScaleY * 72;
            var stride = width * 4;
            // byte[] pixels = new byte[stride * height];
            // ImageSource1 = BitmapSource.Create(width, height, dpiX, dpiY,PixelFormats.Pbgra32, null, pixels, stride);
            var w = new WriteableBitmap(width, height, dpiX, dpiY, PixelFormats.Pbgra32, null);
            DrawingVisual vv = new DrawingVisual();
            var dc = vv.RenderOpen();
            dc.DrawRectangle(new LinearGradientBrush(Colors.Red, Colors.Blue, 45), new Pen(Brushes.Gray, 2), new Rect(arrangeBounds));
            dc.Close();
            _client.SendDrawing(vv.Drawing, _origin);
            _origin.Y += arrangeBounds.Height + 5;
            RenderTargetBitmap tb = new RenderTargetBitmap(width, height, dpiX, dpiY, PixelFormats.Pbgra32);
            tb.Render(vv);
            w.Lock();
            tb.CopyPixels(new Int32Rect(0,0,width,height), w.BackBuffer, w.BackBufferStride * height, w.BackBufferStride);
            // var offset = 0;
            var p = (byte*)w.BackBuffer;

            for(var y = 0 ; y < height; y++)
            for (var x = 0; x < width; x++)
            {
#if false
                var r = pixels[offset];
                pixels[offset] = (byte) (255 - r);
                offset++;
                var g = pixels[offset];
                pixels[offset] = (byte) (255 - g);
                offset++;
                var b = pixels[offset];
                pixels[offset] = (byte) (255 - b);
                offset ++;
                var a = 255;
                pixels[offset] = (byte)(a);
                offset++;
                if (r != 0 || g != 0 || b != 0)
                {
                    Debug.WriteLine("[{0},{1}] = 0x{2:X2}{3:X2}{4:X2}{5:X2}", x, y, r, g, b, a);

                    }
#else
                var r = *p;
                *p++ = (byte)(255 - r);
                var g = *p;
                *p++ = (byte)(255 - g);
                var b = *p;
                *p++ = (byte)(255 - b);
                byte a = 255;
                *p++ = a;
                // if (r != 0 || g != 0 || b != 0)
                // {
                    // Debug.WriteLine("[{0},{1}] = 0x{2:X2}{3:X2}{4:X2}{5:X2}", x, y, r, g, b, a);

                // }
#endif
                }

            w.Unlock();
            ImageSource1 = w;
            var arrangeOverride = base.ArrangeOverride(arrangeBounds);
            return arrangeOverride;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            // ReSharper disable once UnusedVariable
            var measureOverride = base.MeasureOverride(constraint);

            return new Size(CaretWidth, LineHeight);
            // return measureOverride;
        }

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
            "EndPoint", typeof(Point), typeof(TextCaret1), new PropertyMetadata(default(Point)));

        public Point EndPoint
        {
            get => (Point) GetValue(EndPointProperty);
            set => SetValue(EndPointProperty, value);
        }

        public static readonly DependencyProperty LineHeightProperty = LayoutHelper.LineHeightProperty.AddOwner(typeof(TextCaret1),
            new FrameworkPropertyMetadata(default(double),FrameworkPropertyMetadataOptions.AffectsMeasure|FrameworkPropertyMetadataOptions.AffectsRender,
                OnLineHeightChanged));

        public static readonly DependencyProperty ViewportElementLeftProperty =
            LayoutHelper.ViewportElementLeftProperty.AddOwner(typeof(TextCaret1),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsArrange|FrameworkPropertyMetadataOptions.AffectsParentArrange, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        public static readonly DependencyProperty ViewportElementTopProperty =
            LayoutHelper.ViewportElementTopProperty.AddOwner(typeof(TextCaret1),
                new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsArrange, PropertyChangedCallback1));

        private static void PropertyChangedCallback1(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawImage(ImageSource1, new Rect(DesiredSize));
            // var solidColorBrush = Brushes.Black.Clone();
            // solidColorBrush.Opacity = 0.2;
            // drawingContext.DrawRectangle(solidColorBrush, new Pen(solidColorBrush, 1.5), new Rect(DesiredSize));
        }

        public ImageSource ImageSource1 { get; set; }

        public double LineHeight
        {
            get => (double) GetValue(LineHeightProperty);
            set => SetValue(LineHeightProperty, value);
        }

        public static readonly DependencyProperty CaretWidthProperty = DependencyProperty.Register(
            "CaretWidth", typeof(double), typeof(TextCaret1), new PropertyMetadata(default(double)));

        private Point _origin = new Point(0,0);

        public double CaretWidth
        {
            get => (double) GetValue(CaretWidthProperty);
            set => SetValue(CaretWidthProperty, value);
        }
        private static void OnLineHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextCaret1 tc)
            {
                tc.OnLineHeightChanged((double)e.OldValue, (double)e.NewValue);
            }
            
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

        }


        // ReSharper disable once UnusedParameter.Local
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

        public TextCaret1()
        {
            _guid = Guid.NewGuid();
            CoerceValue(DrawingIdentifierProperty);
            _client = new DrawServiceClient(DrawingIdentifier);
            Debug.WriteLine(DrawingIdentifier);
            DpiScale1 = VisualTreeHelper.GetDpi(this);
            PixelsPerDip = DpiScale1.PixelsPerDip;

        }

        public string DrawingIdentifier
        {
            get => (string)GetValue(DrawingIdentifierProperty);
            set => SetValue(DrawingIdentifierProperty, value);
        }

        public static readonly DependencyProperty AppNameProperty =
            DrawingHelper.AppNameProperty.AddOwner(typeof(TextCaret1), new PropertyMetadata(default, PropertyChangedCallback2));

        private static void PropertyChangedCallback2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textCaret1 = (TextCaret1)d;
            // textCaret1.DrawingIdentifier = textCaret1._guid.ToString() + e.NewValue;
        }

        public static readonly DependencyProperty DrawingIdentifierProperty =
            DrawingHelper.DrawingIdentifierProperty.AddOwner(
                typeof(TextCaret1),
                new PropertyMetadata(default, PropertyChangedCallback3, CoerceValueCallback));

        private static object CoerceValueCallback(DependencyObject d, object basevalue)
        {
            var textCaret1 = (TextCaret1)d;

            return (string)d.GetValue(AppNameProperty) + " " + textCaret1._guid.ToString();
        }

        private static void PropertyChangedCallback3(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textCaret1 = (TextCaret1)d;
            textCaret1._client?.CloseDrawing();
            textCaret1.CoerceValue(DrawingIdentifierProperty);
            if (textCaret1._client != null)
            {
                textCaret1._client.DrawingIdentifier = (string) e.NewValue;
                textCaret1._client.Open();
            }
        }

        public DpiScale DpiScale1 { get; set; }


        // protected override Size MeasureOverride(Size constraint)
        // {
        // var measureOverride = base.MeasureOverride(constraint);
        // return new Size(1,LineHeight);
        // return measureOverride;
        // }


        // protected override void OnRender(DrawingContext drawingContext)
        // {
        // base.OnRender(drawingContext);
        // var c = VisualTreeHelper.GetContentBounds(this);
        // drawingContext.DrawLine(_pen, new Point(0, 0), new Point(0, LineHeight));

        // }


        public void SetCaretPosition(in double left, in double top)
        {
            // var rect = GetClipRect(left, top, DesiredSize);

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);

            ViewportElementLeft = left;
            ViewportElementTop = top;

            // //Debug.WriteLine("clip rect is " + rect);
            // Clip = new RectangleGeometry(rect);
        }


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


        private static void PropertyChangedCallback4(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextCaret1) d).DrawingIdentifier = (string) e.NewValue;
        }

        public string AppName
        {
            get { return (string) GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }
        public string Name1 { get; set; }

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

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}