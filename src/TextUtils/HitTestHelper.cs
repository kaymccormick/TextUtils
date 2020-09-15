using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public static class HitTestHelper
    {
        private static readonly IEnumerable<(DependencyObject, DependencyObject, IntersectionDetail?)> _NoResults = Enumerable.Empty<(DependencyObject, DependencyObject, IntersectionDetail?)>();

        static HitTestHelper()
        {
            HitTestHelper._tf = new Typeface(new FontFamily("Arial"), default, default, default);
            if (HitTestHelper._tf.TryGetGlyphTypeface(out var gtf))
                GlyphTypeface1 = gtf;

            var text = new[] {"No drawing content", "Visited element"};
            _ftRuns = text.Select(CreateFormattedText).ToArray();
        }

        private static IEnumerable<(DependencyObject, DependencyObject, IntersectionDetail?)> RecursiveTest(
            HitTestContext context, HitTestPacket packet,
            DependencyObject parent, HitTestParameters ps)
        {
            
            var drawing = packet.Drawing;
            if (drawing == null) throw new ArgumentNullException(nameof(drawing));
            Debug.WriteLine("Recursively trying to hti test elements of a drawing");
            Point p=default;
            Geometry? g = null;
            switch (ps)
            {
                case GeometryHitTestParameters geometryHitTestParameters:
                    g = geometryHitTestParameters.HitGeometry;
                    break;
                case PointHitTestParameters pointHitTestParameters:
                    p = pointHitTestParameters.HitPoint;
                    break;
            }

            p = context.TransformedPoint;
            if (drawing.Bounds.IsEmpty)
            {
                Point origin = packet.TransformToRoot.Transform(new Point(10, 10));
                context.DrawingContext1?.DrawText(_ftRuns[0], origin);
                Debug.WriteLine("Drawing has no bounds");
                return _NoResults;
            }
            var drawingBounds = packet.TransformToRoot.TransformBounds(drawing.Bounds);
            var contains = g != null ? drawingBounds.IntersectsWith(g.Bounds) || drawingBounds.Contains(g.Bounds) : drawingBounds.Contains(p);
            if (!contains)
            {
                // Debug.WriteLine("area is " + new RectangleGeometry(drawingBounds).GetArea());
                var endPoint = new Point(drawingBounds.X + drawingBounds.Width / 2,drawingBounds.Y + drawingBounds.Width / 2);
                switch (parent)
                    {
                    case Visual vv:
                        var offset = VisualTreeHelper.GetOffset(vv);
                        AddDrawing(context, new GeometryDrawing(null, new Pen(Brushes.Yellow, 2),
                            new LineGeometry(new Point(0, 0), new Point(offset.X, offset.Y))));

                        break;
                    default:
                        Debug.WriteLine("Parent of drawing is " + parent.GetType().Name);
                        Debug.WriteLine("Recursively trying to hti test elements of a drawing");
                        break;
                    }


                    drawingBounds = packet.TransformToRoot.TransformBounds(drawingBounds);
                    context.AddDrawing(new GeometryDrawing(null, new Pen(Brushes.Blue, 2),
                        new RectangleGeometry(drawingBounds, 0, 0)));
                    context.AddDrawing(new GeometryDrawing(null, new Pen(Brushes.Green, 2), new LineGeometry(packet.Transform(p), packet.Transform(endPoint))));

                if (g == null)
                {
                    if (drawingBounds.Left > p.X)
                    {
                        Debug.WriteLine("Rect is to the right of point");
                    } else if (drawingBounds.Top > p.Y)
                    {
                        Debug.WriteLine("Rect is to below of point");
                    }
                    else if (drawingBounds.Bottom < p.Y)
                    {
                        Debug.WriteLine("Rect is above point");
                    }
                    else if (drawingBounds.Right < p.X)
                    {
                        Debug.WriteLine("Rect is to left of point");
                    }
                }
            }
            var tmp = drawing.Bounds;
            if (g != null)
            {
                tmp.Intersect(g.Bounds);
                // Debug.WriteLine("geobounds" + RoslynCodeControl.RoundRect(drawingBounds));
                // Debug.WriteLine("drawingBounds " + RoslynCodeControl.RoundRect(g.Bounds));
                // Debug.WriteLine("intersection " + RoslynCodeControl.RoundRect(tmp));
            }

            if (contains)
            {
                // //Debug.WriteLine($"bounding rect contains point {p} ({drawing}, {parent})");
            }
            else
            {
            }

            switch (drawing)
            {
                case DrawingGroup drawingGroup:
                    // if (!contains) //Debug.WriteLine("Bailing on " + drawingGroup);
                    return _NoResults.Concat(drawingGroup.Children.SelectMany(d => RecursiveTest(context, (HitTestPacket) packet.Descendant(d), drawingGroup, ps)));
                case GeometryDrawing geometryDrawing:
                    var p1 = geometryDrawing.Geometry.GetWidenedPathGeometry(new Pen(Brushes.Black, 3), 0.01,
                        ToleranceType.Absolute);
                    if (contains)
                        return RecursiveTest(context, packet, geometryDrawing.Geometry, drawing, geometryDrawing.Brush,
                            geometryDrawing.Pen, ps);
                    break;
                case GlyphRunDrawing glyphRunDrawing:
#if false
                    var buildGeometry = glyphRunDrawing.GlyphRun.BuildGeometry();
                    var pen = new Pen(Brushes.Black, 2);
                    var widened = buildGeometry
                        .GetWidenedPathGeometry(pen, 0.01, ToleranceType.Absolute);

                    bool f = false;
                    if (g != null)
                    {
                        var strokeContainsWithDetail =
                            widened.StrokeContainsWithDetail(pen, g, 0.1, ToleranceType.Absolute);
                        if (strokeContainsWithDetail == IntersectionDetail.FullyInside ||
                            strokeContainsWithDetail == IntersectionDetail.Intersects)
                        {
                            f = true;
                        }
                    }
                    else
                    {
                        if (widened.StrokeContains(pen, p, 0.1, ToleranceType.Absolute))
                        {
                            f = true;
                        }
                    }
#else
                    bool f = contains;
#endif
                    if (f)
                    {
                        Debug.WriteLine("chars " +String.Join("", glyphRunDrawing.GlyphRun.Characters));
                        return _NoResults.Append((glyphRunDrawing, parent, null));
                    }

                    break;
                case ImageDrawing imageDrawing:
                    break;
                case VideoDrawing videoDrawing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(drawing));
            }

            return _NoResults;
        }

        private static void AddDrawing(HitTestContext context, Drawing drawing)
        {
            context.AddDrawing(drawing);
        }

        private static IEnumerable<(DependencyObject, DependencyObject, IntersectionDetail?)> RecursiveTest(
            HitTestContext context, HitTestPacket packet, Geometry geometry,
            DependencyObject parent, Brush geometryDrawingBrush, Pen? geometryDrawingPen, HitTestParameters ps)
        {
            var objs = Enumerable.Empty<(DependencyObject, DependencyObject, IntersectionDetail?)>();
            Point p = default;
            Geometry? g = null;
            switch (ps)
            {
                case GeometryHitTestParameters geometryHitTestParameters:
                    g = geometryHitTestParameters.HitGeometry;
                    break;
                case PointHitTestParameters pointHitTestParameters:
                    p = pointHitTestParameters.HitPoint;
                    break;
            }

            var t = context.RootVisual.TransformToDescendant(packet.Visual);
            p = t.Transform(context.TransformedPoint);
            var p1 = geometry.GetFlattenedPathGeometry(0.1, ToleranceType.Absolute);
            var pp = t.Inverse.Transform(new Point(0, 0));
            context.DrawingContext1?.PushTransform(new TranslateTransform(pp.X, pp.Y));
            var solidColorBrush = new SolidColorBrush( Colors.Pink) {Opacity = 0.25};
            context.DrawingContext1?.DrawGeometry(null, new Pen(solidColorBrush, 1), p1);
            context.DrawingContext1?.Pop();
            bool f = false;
            IntersectionDetail? x = default;
            if (geometryDrawingBrush is SolidColorBrush s && s.Color != Colors.Transparent)
            {
                if (g != null)
                {
                    x = p1.FillContainsWithDetail(g);
                    if (x == IntersectionDetail.Intersects || x == IntersectionDetail.FullyContains)
                    {
                        f = true;
                    }
                } else
                {
                    f = p1.FillContains(p);
                }
            }

            if (!f)
            {
                if (geometryDrawingPen?.Brush != null && geometryDrawingPen.Brush is SolidColorBrush s2 &&
                    s2.Color != Colors.Transparent)
                {
                    if (g != null)
                    {
                        x = p1.StrokeContainsWithDetail(geometryDrawingPen, g, 0.1, ToleranceType.Absolute);
                        if (x == IntersectionDetail.Intersects || x == IntersectionDetail.FullyContains)
                        {
                            f = true;
                        }
                    }
                    else
                    {
                        f = p1.StrokeContains(geometryDrawingPen, p, 0.1, ToleranceType.Absolute);
                    }

                }
            }
            if (!f)
                return objs;

            if (geometry is GeometryGroup gg)
                return objs.Append((gg, parent, x)).Concat(gg.Children.SelectMany(gc => RecursiveTest(context, packet, gc, gg, geometryDrawingBrush, geometryDrawingPen, ps)));
            return objs.Append((geometry, parent, x));

            
        }

        private static IEnumerable<(DependencyObject, DependencyObject, IntersectionDetail?)> RecursiveTest(
            HitTestContext context, Visual visual, DependencyObject? parent,
            HitTestParameters hitTestParameters)
        {
            var t1 = visual.TransformToAncestor((Visual) context.RootVisual);

            switch (visual)
            {
                case DrawingVisual1 dv1:
                    if (dv1.Drawing != null && dv1.Drawing.Bounds.IsEmpty)
                    {
                        var @as = dv1.ArrangedSize;
                        var rect1 = context.TransformToRoot.TransformBounds(new Rect(@as));
                        context.AddDrawing(new GeometryDrawing(null,
                            new Pen(Brushes.Brown,2),
                            new RectangleGeometry(rect1)));
                    }
                    else
                    {
                        if (dv1.Drawing != null)
                            return RecursiveTest(context, (HitTestPacket) context.CreatePacket(dv1.Drawing, visual), visual,
                                hitTestParameters);
                    }

                    break;
                default:
                    var r = Enumerable.Empty<(DependencyObject, DependencyObject, IntersectionDetail?)>();
                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
                    {
                        var c = (Visual) VisualTreeHelper.GetChild(visual, i);
                        if (c is DrawingVisual1 dv1)
                        {
                            if (!dv1.ArrangedSize.IsEmpty)
                            {
                                var rect = new Rect(new Point(dv1.Offset.X, dv1.Offset.Y), dv1.ArrangedSize);
                                var r1 = dv1.TransformToAncestor((Visual) context.RootVisual)
                                    .TransformBounds(rect);

                                RegisterRect(context, r1);
                                if (r1.Contains((Point) context.TransformedPoint))
                                {
                                    r = r.Concat(RecursiveTest(context, c, visual, hitTestParameters));
                                }
                            }
                        }
                        
                    }
                    return r;
            }

            return _NoResults;
        }

        private static void RegisterRect(HitTestContext context, Rect r1)
        {
            return;
            if (Double.IsNegativeInfinity(r1.X))
            {
                r1.X = 0;
            }

            if (Double.IsNegativeInfinity(r1.Y))
            {
                r1.Y = 0;
            }

            var pen = new Pen(Brushes.DeepPink, 3);
            var solidColorBrush = new SolidColorBrush(Colors.DeepSkyBlue){Opacity = 0.05};
            context.AddDrawing(new GeometryDrawing(null,pen, new RectangleGeometry(r1)));
            context.FlushDrawingContext();
        }

        public static Surface1 MyRect { get; set; }

        public static HitTestResult HitTestCore(DependencyObject coreSearchObject, PointHitTestParameters hitTestParameters)
        {
            Debug.WriteLine("Beginning new Hit Test Core for " + coreSearchObject);
            if (coreSearchObject is DrawingVisual1)
            {

            }
            var visual = (Visual) coreSearchObject;
            var context = new HitTestContext(coreSearchObject, hitTestParameters, MyRect);
            var b1 = VisualTreeHelper.GetDescendantBounds(visual);
            context.InitialDescendantBounds = context.TransformToRoot.TransformBounds(context.FixUpRect(b1));
            context.TransformedPoint = context.TransformToRoot.Transform(hitTestParameters.HitPoint);
            RegisterRect(context, context.InitialDescendantBounds);
            context.AddDrawing(new GeometryDrawing(Brushes.Blue, new Pen(Brushes.Black, 2),
                new EllipseGeometry(context.TransformedPoint, 3, 3)));

            context.AddDrawing(new GeometryDrawing(null, new Pen(Brushes.Red, 10), new RectangleGeometry(context.InitialDescendantBounds)));



            // visual.PointToScreen(hitTestParameters.HitPoint);
            var r = RecursiveTest(context, visual, null, hitTestParameters).GroupBy(z => z.Item2)
                .Select(z1 => (z1.Key, z1.Select(zz => zz.Item1)));
            if (r.Any())
            {

            }
            return new AppPointHitTestResult(visual, hitTestParameters.HitPoint, r);
        }

        public static Typeface? _tf;
        private static FormattedText[] _ftRuns;
        public static GlyphTypeface? GlyphTypeface1 { get; private set; }

        public static FormattedText CreateFormattedText(string textToFormat)
        {
            var formattedText = new FormattedText(textToFormat, CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Arial"), default, default, default), 18, Brushes.DarkOrchid, null,
                TextFormattingMode.Display, 1.0);
            return formattedText;
        }
    }
}