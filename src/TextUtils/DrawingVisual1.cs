// #undef DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class DrawingVisual1 : DrawingVisual
    {
        public static readonly RoutedEvent BrushParametersChangedEvent =
            EventManager.RegisterRoutedEvent("BrushParametersChangedEvent", RoutingStrategy.Bubble,
                typeof(BrushParametersChangedEventHandler), typeof(DrawingVisual1));

        /// <inheritdoc />
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            // base.OnVisualParentChanged(oldParent);
            // Debug.WriteLine("XXX " + VisualParent);
            // Debug.WriteLine(ArrangedSize);
            // if (VisualParent is UIElement e)
            // {
                // e.InvalidateMeasure();
                // e.InvalidateArrange();
                // e.InvalidateVisual();
            // }
        }

        public static readonly DependencyProperty SourceDrawingProperty = DependencyProperty.Register(
            "SourceDrawing", typeof(Drawing), typeof(DrawingVisual1),
            new FrameworkPropertyMetadata(default(Drawing), FrameworkPropertyMetadataOptions.AffectsRender,OnSourceDrawingChanged));

        public Drawing SourceDrawing
        {
            get { return (Drawing) GetValue(SourceDrawingProperty); }
            set { SetValue(SourceDrawingProperty, value); }
        }

        private static void OnSourceDrawingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DrawingVisual1) d).OnSourceDrawingChanged((Drawing) e.OldValue, (Drawing) e.NewValue);
        }


        protected void OnSourceDrawingChanged(Drawing oldValue, Drawing newValue)
        {
            if (_isRendering)
                return;
             Fill ??= InitializeFill(newValue);
        }



        // protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters)
        // {
        // var v2 = DrawingVisualDuo.GetSecondary(this);
        // if (v2 is DrawingVisual vv)
        // {
        // _dc = vv.RenderOpen();
        // }

// var r = HitTestHelper.RecursiveTest(null, Drawing, this, hitTestParameters);
// return new AppGeometryHitTestResult(this, IntersectionDetail.NotCalculated, r);
// }

#if CUSTOMHITTEST
        /// <inheritdoc />
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (!IsCustomHitTesting) return base.HitTestCore(hitTestParameters);
            return HitTestHelper.HitTestCore(this, hitTestParameters);
#if false
            var p = hitTestParameters.HitPoint;
            var v2 = DrawingVisualDuo.GetSecondary(this);
            if (v2 is DrawingVisual vv)
            {
                _dc = vv.RenderOpen();
            }
            
            // var r = HitTestHelper.RecursiveTest(null, Drawing, this, hitTestParameters).GroupBy(z => z.Item2)
                // .Select(z1 => (z1.Key, z1.Select(zz => zz.Item1)));

            // if (_dc != null)
            // {
                // NewMethod(v2, r, _dc);
                // _dc.Close();
                // _dc = null;
            // }

            
            // return new AppPointHitTestResult(this, p, r);
#endif
        }
#endif
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public bool IsCustomHitTesting { get; set; } = true;

        private static void NewMethod(Visual v2, IEnumerable<(DependencyObject Key, IEnumerable<DependencyObject>)> r,
            DrawingContext? drawingContext)
        {
            foreach (var (key, dependencyObjects) in r)
                {
                    if (key is Visual v)
                    {
                        var r1 = VisualTreeHelper.GetContentBounds(v);
                        r1.Inflate(2, 2);
                        drawingContext?.DrawRectangle(null, new Pen(Brushes.Aqua, 3), r1);
                    }

                    if (key is Drawing dd0)
                    {
                        var r1 = dd0.Bounds;
                        r1.Inflate(2, 2);
                        drawingContext?.DrawRectangle(null, new Pen(Brushes.Aqua, 3), r1);
                    }

                    foreach (var dependencyObject in dependencyObjects)
                        if (dependencyObject is Drawing dd)
                        {
                            var r1 = dd.Bounds;
                            r1.Inflate(2, 2);
                            drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 3), r1);
                        }
                }


            
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
            "Fill", typeof(DrawingBrush), typeof(DrawingVisual1),
            new FrameworkPropertyMetadata(default(DrawingBrush),FrameworkPropertyMetadataOptions.AffectsRender, OnFillChanged));

        private DrawingContext? _dc;
        private bool _isRendering;

        public DrawingBrush? Fill
        {
            get { return (DrawingBrush?) GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        private static void OnFillChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DrawingVisual1) d).OnFillChanged((DrawingBrush?) e.OldValue, (DrawingBrush?) e.NewValue);
        }


        protected virtual void OnFillChanged(DrawingBrush? oldValue, DrawingBrush? newValue)
        {
            if (_isRendering)
                return;
            if (oldValue != null)
            {
                oldValue.Changed -= OldValueOnChanged;
                // if (oldValue.Drawing != null)
                // {
                    // oldValue.Drawing.Changed -= DrawingOnChanged;
                // }
            }

            if (newValue != null)
            {
                newValue.Changed += OldValueOnChanged;
                // newValue.Drawing.Changed += DrawingOnChanged;
                
            }

            DoRender();
        }

        private void OldValueOnChanged(object? sender, EventArgs e)
        {
            // DoRender();
        }

        public bool DoRender()
        {
            if (_isRendering)
                return true;
            if (ArrangedSize.IsEmpty)
            {
                return false;
            }
            if (!CanRender) return false;
            // if (SourceDrawing == null)
            // {
            // return false;
            // }

            var drawingBounds = Fill.Drawing.Bounds;
            if (drawingBounds.IsEmpty)
                return false;
                if (DebugRender)
                Debug.WriteLine("Begin render");
            
            _isRendering = true;


            DrawingContext? dc = null;
            if (drawingBounds.IsEmpty == false)
            {
                var size = ArrangedSize;
                // if (size.IsEmpty)
                // {
                    // size = drawingBounds.Size;
                // }

                if (Fill.Drawing is DrawingGroup dg)
                {
                    var b = LayoutHelper.GetInverseBounds(dg);
                    if (b.IsEmpty == false)
                    {
                        var offsetX = -1 * b.X;
                        var offsetY = -1 * b.Y;
                        // dg.Transform = new TranslateTransform(offsetX, offsetY);
                        // Debug.WriteLine("Transform is " + offsetX + ", " + offsetY);
                        Rect fillViewport;
                        // var sizeWidth = size.Width - b.X;
                        // var sizeHeight = size.Height - b.Y;
                        // Fill.Viewbox = new Rect(b.X, b.Y, sizeWidth, sizeHeight);

                        Rect fillViewbox;
                        BrushMappingMode fillViewboxUnits;
                        BrushMappingMode fillViewportUnits;
                        if (FitToSize)
                        {
                            fillViewboxUnits = BrushMappingMode.Absolute;
                            fillViewportUnits = BrushMappingMode.Absolute;
                            Fill.Stretch = Stretch.Uniform;
                         
                             fillViewport = new Rect(0, 0, 1, 1);
                         fillViewbox = new Rect(0, 0, 1, 1);
                            // fillViewbox = new Rect(0,0,ArrangedSize.Width,ArrangedSize.Height);
                        }
                        else
                        {
                            fillViewboxUnits = BrushMappingMode.Absolute;
                            fillViewportUnits = BrushMappingMode.Absolute;
                            fillViewport = new Rect(0, 0, size.Width, size.Height);
                            fillViewbox = fillViewport;

                        }


                        if (Fill.Viewport != fillViewport || Fill.Viewbox != fillViewbox ||
                            Fill.ViewportUnits != fillViewportUnits
                            || Fill.ViewboxUnits != fillViewboxUnits)
                        {
                            Fill.Viewport = fillViewport;
                            Fill.ViewportUnits = fillViewportUnits;
                            Fill.Viewbox = fillViewbox;
                            Fill.ViewboxUnits = fillViewboxUnits;

                            RaiseEvent(new BrushParametersChangedEventArgs(this, Fill, this));

                        }
                    }
                }

                dc = RenderOpen();
                var rectangle = new Rect(size);
                // Debug.WriteLine("DrawRectangle " + RoslynCodeControl.RoundRect(rectangle));

                dc.DrawRectangle(Fill,
                    null, 
                    rectangle);
                if (DG != null && DebugMode)
                {
                    var dc0 = DG.Append();
                    dc0.DrawRectangle(Fill,
                        null,//new Pen(Brushes.Blue, 5), 
                        rectangle);
                    dc0.Close();
                }
            }
            else
            {

                if (Fill.Drawing is DrawingGroup fillDrawing)
                {
                    // Debug.WriteLine("DG Count" + fillDrawing.Children.Count);
                }
            }

            if (!WriteInfo)
            {
                try
                {
                    dc?.Close();
                }
                catch
                {
                    // ignored
                }

                _isRendering = false;
                if(DebugRender)
                    Debug.WriteLine("End render");
                return true;
            }var ft = new FormattedText(DebugInfo(Fill), CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Arial"), default, default, default), 14.0, Brushes.LimeGreen, null,
                    1.0);
                var origin = new Point(10, 10);
                List<GeometryHitTestResult> rs = new List<GeometryHitTestResult>();
                Visual? reference = Window.GetWindow(this);
                if (reference == null)
                {
                    DependencyObject? parent = VisualTreeHelper.GetParent(this);
                    DependencyObject? old1 = null;
                    while (parent != null)
                    {
                        old1 = parent;
                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    reference = (Visual?) old1;
                }

                if (reference != null)
                {
                    for (;;)
                    {
                        var rect = new Rect(origin, new Size(ft.Width, ft.Height));

                        rect = TransformToAncestor(reference).TransformBounds(rect);
                        // //Debug.WriteLine(rect);
                        VisualTreeHelper.HitTest(reference, target => { return HitTestFilterBehavior.Continue; },
                            result =>
                            {
                                switch (result)
                                {
                                    case AppGeometryHitTestResult appGeometryHitTestResult:
                                        rs.Add(appGeometryHitTestResult);
                                        break;
                                    case GeometryHitTestResult rr:
                                        rs.Add(rr);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(result));
                                }

                                return HitTestResultBehavior.Continue;
                            },
                            new GeometryHitTestParameters(
                                new RectangleGeometry(rect)));

                        // //Debug.WriteLine("===========================");
                        bool intersectsText = false;
                        foreach (var zzz in rs.GroupBy(zz => zz.GetType()))
                        {
                            // //Debug.WriteLine($"{zzz.Key} - {zzz.Count()}");
                            foreach (GeometryHitTestResult geometryHitTestResult in zzz)
                            {
                                switch (geometryHitTestResult)
                                {
                                    case AppGeometryHitTestResult appGeometryHitTestResult:
                                        // //Debug.WriteLine(
                                            // $"{appGeometryHitTestResult.VisualHit} - {appGeometryHitTestResult.ValueTuples.Count()}");
                                        foreach (var (dependencyObject, _, _) in
                                            appGeometryHitTestResult.ValueTuples)
                                        {
                                            // //Debug.WriteLine($"     {dependencyObject} {item2} {intersectionDetail}");
                                            if (dependencyObject is GlyphRunDrawing ggg)
                                            {
                                                intersectsText = true;
                                                //Debug.WriteLine(String.Join("", ggg.GlyphRun.Characters));
                                                // //Debug.WriteLine(rect.ToString() + " " + ggg.Bounds.ToString());
                                                var r11 = rect;
                                                r11.Intersect(ggg.Bounds);
                                                //Debug.WriteLine(r11);
                                            }
                                        }

                                        break;
                                }
                            }
                        }

                        // //Debug.WriteLine("===========================");


                        if (!intersectsText)
                            break;


                        origin = new Point(origin.X, origin.Y + ft.Height + 5);
                        //Debug.WriteLine(origin);
                        // break;
                    }
                }

                dc ??= RenderOpen();
                dc.DrawText(ft, origin);
                dc.Close();
                _isRendering = false;
                if(DebugRender)
                Debug.WriteLine("End render");

                return true;
            }

        public virtual bool CanRender => true;

        private void RaiseEvent(RoutedEventArgs args)
        {
            DependencyObject p = this;
            while(p != null && !(p is  UIElement))
            {
                p = VisualTreeHelper.GetParent(p);
            }

            if (p is UIElement u)
            {
                u.RaiseEvent(args);
            }
        }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        public bool DebugRender { get; } = true;

        public bool WriteInfo { get; set; } = false;

        private static string DebugInfo(DrawingBrush fill)
        {
            return $"{fill.Drawing} - {fill.Viewport} - {fill.Viewbox} - {fill.Drawing.Bounds}";
        }

        private static DrawingBrush InitializeFill(Drawing drawing)
        {
            return new DrawingBrush(drawing)
            {
                TileMode = TileMode.None,
                Stretch = Stretch.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Top
            };
        }

        public static readonly DependencyProperty ArrangedSizeProperty = DependencyProperty.Register(
            "ArrangedSize", typeof(Size), typeof(DrawingVisual1), new FrameworkPropertyMetadata(Size.Empty,OnArrangedSizeChanged));

        public static DrawingGroup? DG;

        public Size ArrangedSize
        {
            get { return (Size) GetValue(ArrangedSizeProperty); }
            set { SetValue(ArrangedSizeProperty, value); }
        }

        private static void OnArrangedSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DrawingVisual1) d).OnArrangedSizeChanged((Size) e.OldValue, (Size) e.NewValue);
        }



        // ReSharper disable twice UnusedParameter.Global
        protected void OnArrangedSizeChanged(Size oldValue, Size newValue)
        {
            if (Fill?.Drawing?.Bounds.IsEmpty ?? true)
                return;
            DoRender();
        }
    
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Size AvailableSize { get; set; } = Size.Empty;
        public bool DebugMode { get; set; }
        public bool FitToSize { get; set; }

        public Size Measure(Size constraint)
        {
            return constraint;
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
}