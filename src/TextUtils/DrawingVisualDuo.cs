using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class DrawingVisualDuo : FrameworkElement
    {
        private readonly VisualCollection _children;

        /// <inheritdoc />
        protected override void OnRender(DrawingContext drawingContext)
        {
            // if (PrimaryVisual is DrawingVisual1 dv1) dv1.DoRender();

            // if (PrimaryVisual is DrawingVisual1 dv2) dv2.DoRender();
            base.OnRender(drawingContext);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (double.IsNaN(finalSize.Width) || double.IsInfinity(finalSize.Width)) finalSize.Width = 0;
            if (double.IsNaN(finalSize.Height) || double.IsInfinity(finalSize.Height)) finalSize.Height = 0;

            if (PrimaryVisual is DrawingVisual1 d)
            {
                var drawingBounds = LayoutHelper.GetInverseBounds((DrawingGroup) d.Fill.Drawing);
                if (drawingBounds.IsEmpty == false)
                {
                    if (double.IsInfinity(drawingBounds.X)) drawingBounds.X = 0;

                    if (double.IsInfinity(drawingBounds.Y))
                        drawingBounds.Y = 0;

                    // d.Offset = new Vector(drawingBounds.X, drawingBounds.Y);
                }

                d.ArrangedSize = finalSize;
            } else if (PrimaryVisual is DrawingVisual dv)
            {
                var drawingBounds = LayoutHelper.GetInverseBounds(dv.Drawing);
                if (drawingBounds.IsEmpty == false)
                {
                    if (double.IsInfinity(drawingBounds.X)) drawingBounds.X = 0;

                    if (double.IsInfinity(drawingBounds.Y))
                        drawingBounds.Y = 0;

                    // dv.Offset = new Vector(drawingBounds.X, drawingBounds.Y);
                }
            }
            else
            {
            }

            if (SecondaryVisual is DrawingVisual1 d1)
            {
                d1.ArrangedSize = finalSize;
            }
            else
            {
            }

            return finalSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (PrimaryVisual is DrawingVisual1 dv) dv.AvailableSize = availableSize;
            var b1 = VisualTreeHelper.GetContentBounds(PrimaryVisual);
            if (SecondaryVisual != null)
            {
                var b2 = VisualTreeHelper.GetContentBounds(SecondaryVisual);
                if (!b2.IsEmpty && !b1.IsEmpty)
                    b1.Union(b2);
            }

            if (b1.IsEmpty)
            {
                // return b2.IsEmpty ? new Size(0,0) : b2.Size;
            }

            return availableSize;
        }

        public static readonly DependencyProperty SecondaryProperty = DependencyProperty.RegisterAttached("Secondary",
            typeof(Visual), typeof(DrawingVisualDuo), new PropertyMetadata(default(Visual)));

        public DrawingVisualDuo()
        {
            _children = new VisualCollection(this);
        }

        static DrawingVisualDuo()
        {
            FocusableProperty.OverrideMetadata(typeof(DrawingVisualDuo), new FrameworkPropertyMetadata(false));
            IsHitTestVisibleProperty.OverrideMetadata(typeof(DrawingVisualDuo), new FrameworkPropertyMetadata(true));
        }

        public static readonly DependencyProperty SecondaryVisualProperty = DependencyProperty.Register(
            "SecondaryVisual", typeof(Visual), typeof(DrawingVisualDuo),
            new PropertyMetadata(default(Visual?), OnSecondaryVisualChanged));

        public Visual? SecondaryVisual
        {
            get { return (Visual?) GetValue(SecondaryVisualProperty); }
            set { SetValue(SecondaryVisualProperty, value); }
        }

        private static void OnSecondaryVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DrawingVisualDuo) d).OnSecondaryVisualChanged((Visual?) e.OldValue, (Visual?) e.NewValue);
        }


        protected virtual void OnSecondaryVisualChanged(Visual? oldValue, Visual? newValue)
        {
            if (oldValue != null) _children.Remove(oldValue);
            if (newValue != null)
            {
                _children.Insert(_children.Count, newValue);
                if (PrimaryVisual != null) SetSecondary(PrimaryVisual, newValue);
            }
        }

        public static readonly DependencyProperty PrimaryVisualProperty = DependencyProperty.Register(
            "PrimaryVisual", typeof(Visual), typeof(DrawingVisualDuo),
            new PropertyMetadata(default(Visual), OnPrimaryVisualChanged));

        public Visual PrimaryVisual
        {
            get { return (Visual) GetValue(PrimaryVisualProperty); }
            set { SetValue(PrimaryVisualProperty, value); }
        }

        private static void OnPrimaryVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DrawingVisualDuo) d).OnPrimaryVisualChanged((Visual) e.OldValue, (Visual) e.NewValue);
        }


        protected virtual void OnPrimaryVisualChanged(Visual oldValue, Visual newValue)
        {
            if (oldValue != null)
            {
                _children.Remove(oldValue);
                SetSecondary(oldValue, null);
            }

            if (newValue != null)
            {
                _children.Insert(0, newValue);
                SetSecondary(newValue, SecondaryVisual);
            }
        }


        /// <inheritdoc />
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsCustomHitTesting { get; set; } = true;
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool DebugMode { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool FitToSize { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public double Top { get; set; }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            return _children[index];
        }


        public static Visual GetSecondary(DependencyObject element)
        {
            return (Visual) element.GetValue(SecondaryProperty);
        }

        private static void SetSecondary(DependencyObject element, Visual? value)
        {
            element.SetValue(SecondaryProperty, value);
        }
        // protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        // {

// var r = HitTestHelper.RecursiveTest(null,  PrimaryVisual, this, hitTestParameters);
// return new AppGeometryHitTestResult(this, IntersectionDetail.NotCalculated, r);
// }

#if CUSTOMHITTEST
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (!IsCustomHitTesting)
                return base.HitTestCore(hitTestParameters);
            return HitTestHelper.HitTestCore(this, hitTestParameters);
            // var p = hitTestParameters.HitPoint;

            // var r = HitTestHelper.RecursiveTest(null, (Drawing) PrimaryVisual, this, hitTestParameters)
            // .GroupBy(z => z.Item2)
            // .Select(z1 => (z1.Key, z1.Select(zz => zz.Item1)));

            // return new AppPointHitTestResult(this, p, r);
        }
#endif
    }
}