using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TextUtils
{
    public static class LayoutHelper
    {

        public static void SetCodeViewZIndex(DependencyObject d, int value)
        {
            d.SetValue(CodeViewZIndexProperty, value);
        }

        public static int GetCodeViewZIndex(DependencyObject d)
        {
            return (int)d.GetValue(CodeViewZIndexProperty);
        }8
        public static readonly DependencyProperty IsAppHitTestVisibleProperty =
            DependencyProperty.RegisterAttached("IsAppHitTestVisible", typeof(bool), typeof(LayoutHelper),
                new PropertyMetadata(true));

        public static readonly DependencyProperty CodeViewZIndexProperty = DependencyProperty.RegisterAttached(
            "CodeViewZIndex", typeof(int), typeof(LayoutHelper),
            new PropertyMetadata(default(int)));

        public static readonly DependencyProperty ShrinkToFitProperty = DependencyProperty.RegisterAttached(
            "ShrinkToFit", typeof(bool), typeof(LayoutHelper), new PropertyMetadata(default(bool)));

        public static void SetShrinkToFit(DependencyObject d, bool value)
        {
            d.SetValue(ShrinkToFitProperty, value);
        }

        public static bool GetShrinkToFit(DependencyObject d)
        {
            return (bool)d.GetValue(ShrinkToFitProperty);
        }

        public static readonly DependencyProperty CodeViewPort2Property = DependencyProperty.RegisterAttached(
            "ViewPort", typeof(ViewPort), typeof(LayoutHelper),
            new PropertyMetadata(default(ViewPort), OnViewPortChanged));

        public static readonly DependencyProperty IsManagedProperty = DependencyProperty.RegisterAttached(
            "IsManaged", typeof(bool), typeof(LayoutHelper), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty LineHeightProperty = DependencyProperty.RegisterAttached(
            "LineHeight", typeof(double), typeof(LayoutHelper),
            new FrameworkPropertyMetadata(default(double),
                FrameworkPropertyMetadataOptions.Inherits));

        // ReSharper disable once MemberCanBePrivate.Global
        public static void SetIsManaged(DependencyObject d, bool value)
        {
            d.SetValue(IsManagedProperty, value);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static bool GetIsManaged(DependencyObject d)
        {
            return (bool)d.GetValue(IsManagedProperty);
        }


        public static void PerformMeasure(Size constraint, int i, UIElement child)
        {
            if (double.IsInfinity(constraint.Width)) constraint.Width = 0;
            if (double.IsInfinity(constraint.Height)) constraint.Height = 0;

            child.Measure(constraint);
        }

        public static Rect? PerformArrange(Size finalSize, DependencyObject child, int i, Vector minVector, Vector v1,
            Size size, Action<string>? add, double viewportTop, double viewportLeft, List<string> arrangeList,
            bool shrinkToFit = false)
        {
            var zIndex = 0;//RoslynCodeBase.GetCodeViewZIndex(child);
            var managed = false;//RoslynCodeBase.GetIsManaged(child);
            Action<string>? addAction = s =>
            {
                add?.Invoke(s);
                //Debug.WriteLine(s);
            };

            addAction?.Invoke($"Child {i} is {child.GetType().Name} (Managed = {managed}; zIndex = {zIndex})");

            Rect finalRect;
            switch (child)
            {
                case DrawingVisual1 dv1:
                {
                    if (dv1.Fill?.Drawing != null)
                    {
                        var inverseBounds = GetInverseBounds(dv1.Drawing);
                        var valueTuple = GetBrushParameters(0.0, inverseBounds, finalSize,
                            minVector, viewportTop, viewportLeft, size, shrinkToFit);

                        Translated(dv1.Fill.Drawing as DrawingGroup,
                            zIndex, managed,
                            dv1.Fill, viewportLeft, arrangeList,
                            valueTuple,
                            inverseBounds, arrangeList.Add);
                        var ss = new Size(Math.Max(0, size.Width - v1.X),
                            Math.Max(0, finalSize.Height - v1.Y));
                        // //Debug.WriteLine(ss);
                        finalRect = new Rect(new Point(v1.X, v1.Y), ss);
                        dv1.Offset = new Vector(-1 * finalRect.X, -1 * finalRect.Y);
                        // //Debug.WriteLine("FinalRect = " + finalRect);
                    }
                    else
                    {
                    }
                }
                    break;
                case Rectangle shape when shape.Fill is DrawingBrush brush && brush?.Drawing is DrawingGroup g:
                {
                    addAction?.Invoke("Child is Rectangle with DrawingBrush and DrawingGroup");
                    var valueTuple = GetBrushParameters(0.0, GetInverseBounds(g), finalSize,
                        minVector, viewportTop, viewportLeft, size);

                    Translated(g,
                        zIndex, managed,
                        brush, viewportLeft, arrangeList,
                        valueTuple, GetInverseBounds(g), arrangeList.Add);
                    var ss = new Size(Math.Max(0, size.Width - v1.X),
                        Math.Max(0, finalSize.Height - v1.Y));
                    // //Debug.WriteLine(ss);
                    finalRect = new Rect(new Point(v1.X, v1.Y), ss);

                    // //Debug.WriteLine("FinalRect = " + finalRect);
                    break;
                }
                // case MyRect1 _:
                    // finalRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
                    // 9D3ebug.WriteLine("MyRect arrange " + finalRect);
                    // break;
                default:
                    var v1Y = 0.0;
                {
                    // double bX = 0;
                    // double bY = 0;

                    // bX = IsRulerEnabled && IsVerticalRulerEnabled ? -45.0 : 0.0;
                    // bY = IsRulerEnabled && IsHorizontalRulerEnabled ? -35.0 : 0.0;


                    var isRuler = false;//child is Ruler;
                    var left = GetViewportElementLeft(child);
                    var top = GetViewportElementTop(child);
                    var uiElement = (UIElement) child;
                    var width = uiElement.DesiredSize.Width;
                    var height = uiElement.DesiredSize.Height;
                    var @fixed = GetPosition(uiElement) == Position.Fixed;
                    viewportTop = @fixed ? 0.0 : 1 * viewportTop;
                    var v1X = 0.0;

                    if (!isRuler)
                    {
                        v1Y = v1.Y;
                        v1X = v1.X;
                        if (height >= v1Y) height -= v1Y;

                        if (width > v1X) width -= v1X;
                    }

                    var s = new Size(width, height);
                    var x = left -
                        (@fixed ? 0 : 1 * viewportLeft) + v1X;

                    var y = top - viewportTop +
                            v1Y;
                    var location = new Point(x, y);

                    finalRect = new Rect(location, s);

                    break;
                }
            }

            addAction?.Invoke("Arranging at " + DebugUtils.RoundRect(finalRect));

            if (child is UIElement uie)
                uie.Arrange(finalRect);
            return finalRect;
        }

        private static void Translated(DrawingGroup? drawingGroup, int zIndex, bool managed, DrawingBrush brush, double viewportLeft,
            List<string> arrangeList, (Rect, Rect) brushParameters,
            Rect gBounds, Action<string>? action = null)
        {
            // static void DumpChildren(DrawingGroup g)
            // {
            // foreach (var gChild in g.Children)
            // //Debug.WriteLine("" + gChild + " " + gChild.Bounds);
            // if (gChild is DrawingGroup dg1)
            // DumpChildren(dg1);
            // }

            var d = gBounds.IsEmpty ? 0 : -1 * gBounds.Top;

            var t = GetTransform();
            if (drawingGroup != null)
            {
                drawingGroup.Transform = new TranslateTransform(t.X, t.Y);
                action?.Invoke("Transform is " + Math.Round((double) t.X) + ", " + Math.Round((double) t.Y));

                // //Debug.WriteLine(RoslynCodeControl.RoundPoint(new Point(t.X, t.Y)));
                if (managed || zIndex == 0) NewMethod(arrangeList, brushParameters, d, brush, zIndex);

                if (!managed)
                {
                }
                else
                {
                    // DumpChildren(drawingGroup);
                }
            }
        }

        public static Rect GetInverseBounds(DrawingGroup drawingGroup)
        {
            var gBounds = drawingGroup?.Bounds ?? Rect.Empty;
            gBounds = drawingGroup?.Transform?.Inverse.TransformBounds(gBounds) ?? gBounds;
            return gBounds;
        }

        private static Vector GetTransform()
        {
            Vector returnVector;
            // returnVector.X = -1 * viewportLeft - vector.X;
            // returnVector.Y = -1 * viewportTop - vector.Y;
            returnVector.X = 0.0; // v1.X;
            returnVector.Y = 0.0; //v1.Y;

            return returnVector;
        }

        private static void NewMethod(List<string> arrangeList, (Rect, Rect) getBrushParameters, double d,
            TileBrush brush, int zIndex)
        {
            var (brushViewbox, brushViewport) =
                getBrushParameters;

            arrangeList.Add($"\t[{zIndex:D3}] Brush Viewbox = {DebugUtils.RoundRect(brushViewbox)}");
            arrangeList.Add($"\t[{zIndex:D3}] Brush Viewport = {DebugUtils.RoundRect(brushViewport)}");
            brush.Viewport = brushViewport;
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Viewbox = brushViewbox;
            brush.ViewboxUnits = BrushMappingMode.Absolute;
        }

        private static (Rect, Rect) GetBrushParameters(double d,
            Rect gBounds,
            Size size1, Vector vector, double viewportTop,
            double viewportLeft,
            Size toFit = default,
            bool shrinkToFit = false)
        {
            if (shrinkToFit)
                return GetBrushParametersShrinkToFit(gBounds, toFit);
            else
                return GetBrushParameters0(d, gBounds, size1, vector,
                    viewportTop, viewportLeft);
        }

        private static (Rect, Rect) GetBrushParameters0(double d,
            Rect gBounds,
            Size size1, Vector vector, double viewportTop, double viewportLeft)
        {
            if (Math.Abs(size1.Width) < 0.1) size1.Width = double.NaN;

            if (Math.Abs(size1.Height) < 0.1)
                size1.Height = double.NaN;
            var gBoundsLeft = gBounds.IsEmpty ? 0 : -1 * gBounds.Left;
            var left = Math.Max(size1.Width + vector.X + gBoundsLeft, 0);
            size1.Width = gBounds != null && (double.IsNaN(gBounds.Width) ||
                                              double.IsInfinity(gBounds.Width))
                ? left
                : Math.Max(gBounds.Width, left);

            var size1Height = size1.Height + vector.Y + d;
            if (!double.IsNaN(gBounds.Height) && !double.IsInfinity(gBounds.Height))
                size1Height = size1.Height + vector.Y + d;

            if (size1Height < 0) size1Height = 0;

            size1.Height = size1Height;

            var brushViewboxWidth = size1.Width;
            var brushViewboxHeight = size1.Height;

            var boundsLeft = size1.Width;
            var boundsTop = size1.Height;

            var boundsWidth = 0.0;
            var boundsHeight = 0.0;

            var brushViewport = new Rect(
                boundsWidth - viewportLeft,
                boundsHeight - viewportTop, boundsLeft,
                boundsTop);

            var brushViewbox = new Rect(0, 0, brushViewboxWidth, brushViewboxHeight);
            return (brushViewbox, brushViewport);
        }

        private static (Rect, Rect) GetBrushParametersShrinkToFit(Rect gBounds,
            Size toFit)
        {
            var brushViewport = new Rect(0.0,
                0.0, gBounds.Width, gBounds.Height);

            var brushViewbox = new Rect(0.0,
                0.0,
                toFit.Width,
                toFit.Height);
            return (brushViewbox, brushViewport);
        }

        public static readonly DependencyProperty ViewportElementLeftProperty = DependencyProperty.RegisterAttached(
            "ViewportElementLeft", typeof(double), typeof(LayoutHelper),
            new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty ViewportElementTopProperty = DependencyProperty.RegisterAttached(
            "ViewportElementTop", typeof(double), typeof(LayoutHelper),
            new FrameworkPropertyMetadata(default(double), FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static readonly DependencyProperty ViewportTopProperty = DependencyProperty.RegisterAttached(
            "ViewportTop", typeof(double), typeof(LayoutHelper),
            new FrameworkPropertyMetadata(default(double),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits, OnViewportTopChanged));

        private static void OnViewportTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.RegisterAttached("Position",
            typeof(Position), typeof(LayoutHelper), new PropertyMetadata(default(Position)));

        public static Position GetPosition(DependencyObject element)
        {
            return (Position) element.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject element, Position value)
        {
            element.SetValue(PositionProperty, (object) value);
        }

        public static void SetViewportElementLeft(DependencyObject d, in double value)
        {
            d.SetValue(ViewportElementLeftProperty, value);
        }

        public static double GetViewportElementLeft(DependencyObject d)
        {
            return (double) d.GetValue(ViewportElementLeftProperty);
        }

        public static void SetViewportElementTop(DependencyObject d, in double value)
        {
            d.SetValue(ViewportElementTopProperty, value);
        }

        public static double GetViewportElementTop(DependencyObject d)
        {
            return (double) d.GetValue(ViewportElementTopProperty);
        }

        public static bool GetIsAppHitTestVisible(DependencyObject element)
        {
            return (bool) element.GetValue(IsAppHitTestVisibleProperty);
        }
    }
}