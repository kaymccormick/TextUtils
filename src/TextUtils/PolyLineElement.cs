using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TextUtils
{
    public sealed class PolyLineElement : GenericGraphicsElement
    {

        public ObservableCollection<PointElement> Points { get; } = new ObservableCollection<PointElement>();

        static PolyLineElement()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PolyLineElement),
                new FrameworkPropertyMetadata(typeof(PolyLineElement)));
        }

        public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush),
            typeof(PolyLineElement), new PropertyMetadata(default(Brush)));


        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            Rectangle = (Rectangle?) GetTemplateChild("Rectangle");
            DrawingGroup = (DrawingGroup?) GetTemplateChild("DrawingGroup");
            if (DrawingGroup != null) DrawingGroup.Changed += DrawingGroupOnChanged;
            DrawingGroup2 = (DrawingGroup?) GetTemplateChild("DrawingGroup2");
            DrawingGroup3 = (DrawingGroup?) GetTemplateChild("DrawingGroup3");
            PathGeometry = (PathGeometry?) GetTemplateChild("PathGeometry");
            var i = 0;
            var firstPoint = Points[i];
            LastPoint = firstPoint;
            var pathFigure = new PathFigure();
            BindingOperations.SetBinding(pathFigure, PathFigure.StartPointProperty,
                new Binding("Point") {Source = firstPoint});
            PathGeometry?.Figures.Add(pathFigure);

            for (; i < Points.Count; i++)
            {
                var point = Points[i];
                ProcessPoint(point);
                LastPoint = point;
            }
        }

        public DrawingGroup? DrawingGroup3 { get; set; }

        private PointElement? LastPoint { get; set; }

        public static readonly DependencyProperty StrokePenProperty = DependencyProperty.Register(
            "StrokePen", typeof(Pen), typeof(PolyLineElement), new PropertyMetadata(default(Pen)));

        private Point _dragStart;
        private Point _posStart;
        private Point _posEnd;

        public Pen? StrokePen
        {
            get => (Pen) GetValue(StrokePenProperty);
            set => SetValue(StrokePenProperty, value);
        }

        public DrawingGroup? DrawingGroup2 { get; set; }

        public Rectangle? Rectangle { get; set; }

        private void DrawingGroupOnChanged(object? sender, EventArgs e)
        {
            if (DrawingGroup == null)
            {
                return;
            }

            if (double.IsInfinity(DrawingGroup.Bounds.Width) == false &&
                double.IsInfinity(DrawingGroup.Bounds.Height) == false)
            {
                if (Rectangle != null)
                {
                    Rectangle.Width = DrawingGroup.Bounds.Width + 5;
                    Rectangle.Height = DrawingGroup.Bounds.Height + 5;
                }

                ViewportElementLeft = DrawingGroup.Bounds.Left - 2.5;
                ViewportElementTop = DrawingGroup.Bounds.Top - 2.5;
            }
        }

        public DrawingGroup? DrawingGroup { get; set; }

        public PathGeometry? PathGeometry { get; set; }

        public Brush Fill
        {
            get => (Brush) GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (DesignSurface!.InOperation)
                return;
            if (Dragging == null) return;
            Debug.WriteLine("Dragging complete");
            ReleaseMouseCapture();
            Dragging = null;
            var parent = (MyRect1) VisualParent;
            parent.Dragging = null;
        }

        /// <inheritdoc />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (DesignSurface!.InOperation)
                return;

            Hover = null;
            var parent = (MyRect1) VisualParent;
            parent.Hover = null;

        }

        public static readonly DependencyProperty SelectedPointProperty = DependencyProperty.Register(
            "SelectedPoint", typeof(PointElement), typeof(PolyLineElement),
            new PropertyMetadata(default(PointElement)));

        // ReSharper disable once UnusedMember.Global
        public PointElement SelectedPoint
        {
            get => (PointElement) GetValue(SelectedPointProperty);
            set => SetValue(SelectedPointProperty, (object) value);
        }

        public static readonly DependencyProperty SelectedPointBrushProperty = DependencyProperty.Register(
            "SelectedPointBrush", typeof(Brush), typeof(PolyLineElement), new PropertyMetadata(default(Brush)));

        public Brush SelectedPointBrush
        {
            get => (Brush) GetValue(SelectedPointBrushProperty);
            set => SetValue(SelectedPointBrushProperty, value);
        }

        public override Drawing AsDrawing()
        {
            IEnumerable<Point> points = Points.Skip(1).Select(p => p.Point);
            IEnumerable<PathSegment> segments = new[] {new PolyLineSegment(points, true)};
            IEnumerable<PathFigure> figures = new List<PathFigure>() {new PathFigure(Points[0].Point, segments, false)};
            return new GeometryDrawing(Fill, StrokePen, new PathGeometry(figures));
        }

        public override Visual? GetVisual(int param)
        {
            return this;
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            var parent = (MyRect1) VisualParent;
            var p2 = e.GetPosition(parent);
            DrawDebugPoint(p2);
            if (DesignSurface!.InOperation)
                return;
            if (!Completed)
                return;

            if (Dragging == null)
            {
                // Debug.WriteLine("Clearing drawing group 3");
                if (DrawingGroup3 != null)
                {
                    DrawingGroup3.Children.Clear();
                    foreach (var pointElement in Points)
                    {
                        const double radius = 5.0;
                        if (!new EllipseGeometry(pointElement.Point, radius, radius).FillContains(p2)) continue;
                        if (DrawingGroup2 != null)
                            foreach (var drawingGroup2Child in DrawingGroup2.Children)
                            {
                                if (!(drawingGroup2Child is GeometryDrawing gd)) continue;
                                if (!(gd.Geometry is LineGeometry lg)) continue;
                                if (!lg.GetWidenedPathGeometry(new Pen(Brushes.Black, 5)).FillContains(p2))
                                {
                                    gd.Pen = StrokePen;
                                }
                            }

                        Debug.WriteLine("Hovering over point " + pointElement.Point);
                        var ellipseGeometry = new EllipseGeometry() {RadiusX = radius, RadiusY = radius};
                        BindingOperations.SetBinding(ellipseGeometry, EllipseGeometry.CenterProperty,
                            new Binding("Point") {Source = pointElement, Mode = BindingMode.TwoWay});
                        DrawingGroup3.Children.Add(new GeometryDrawing(SelectedPointBrush, null, ellipseGeometry));
                        Hover = pointElement;
                        parent.Hover = Hover;
                        e.Handled = true;
                        return;
                    }
                }
            }

            if (Hover != null &&
                e.LeftButton == MouseButtonState.Pressed
                && Dragging == null)
            {

                e.Handled = true;

                Debug.WriteLine("Dragging begun");
                CaptureMouse();
                _dragStart = p2;
                switch (Hover)
                {
                    case GeometryDrawing gd:
                    {
                        var lineGeometry = (LineGeometry) gd.Geometry;
                        _posStart = lineGeometry.StartPoint;
                        _posEnd = lineGeometry.EndPoint;
                        break;
                    }
                    case PointElement pe1:
                    {
                        _posStart = pe1.Point;
                        break;
                    }
                }

                parent.Dragging = Hover;
                Dragging = Hover;
                return;
            }

            if (Dragging != null)
            {
                e.Handled = true;
                var dragStartX = p2.X - _dragStart.X;
                var dragStartY = p2.Y - _dragStart.Y;
                switch (Dragging)
                {
                    case GeometryDrawing gd:
                    {
                        var draggingGeometry = (LineGeometry) gd.Geometry;
                        draggingGeometry.StartPoint = new Point(_posStart.X + dragStartX, _posStart.Y + dragStartY);
                        draggingGeometry.EndPoint = new Point(_posEnd.X + dragStartX, _posEnd.Y + dragStartY);
                        break;
                    }
                    case PointElement pe:
                        pe.Point = new Point(_posStart.X + dragStartX, _posStart.Y + dragStartY);
                        break;
                }

                return;
            }

            e.Handled = true;
            if (DrawingGroup2 == null) return;
            foreach (var drawingGroup2Child in DrawingGroup2.Children)
            {
                if (!(drawingGroup2Child is GeometryDrawing gd)) continue;
                if (!(gd.Geometry is LineGeometry lg)) continue;
                if (!lg.GetWidenedPathGeometry(new Pen(Brushes.Black, 5)).FillContains(p2))
                {
                    gd.Pen = StrokePen;
                    continue;
                }

                Hover = gd;
                parent.Hover = gd;
                Debug.WriteLine("over " + lg.StartPoint + " - " + lg.EndPoint);
                var pen = gd.Pen.Clone();
                pen.Brush = Brushes.Red;
                gd.Pen = pen;
            }
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var p = hitTestParameters.HitPoint;
            var hit = false;
            foreach (var pointElement in Points)
            {
                const double radius = 5.0;
                if (new EllipseGeometry(pointElement.Point, radius, radius).FillContains(p))
                    hit = true;

            }

            if (!hit)
                return new PointHitTestResult(null, p);
            return new PointHitTestResult(this, p);
        }

        public void AddPoint(PointElement element)
        {
            Points.Add(element);
            ProcessPoint(element);
        }

        private void ProcessPoint(PointElement point)
        {
            if (PathGeometry != null)
            {
                var figure = PathGeometry.Figures[0];
                var lineGeometry = new LineGeometry();
                BindingOperations.SetBinding(lineGeometry, LineGeometry.StartPointProperty,
                    new Binding("Point") {Source = LastPoint, Mode = BindingMode.TwoWay});
                BindingOperations.SetBinding(lineGeometry, LineGeometry.EndPointProperty,
                    new Binding("Point") {Source = point, Mode = BindingMode.TwoWay});
                var lineSegment = new LineSegment {IsStroked = true};
                figure.Segments.Add(lineSegment);
                BindingOperations.SetBinding(lineSegment, LineSegment.PointProperty,
                    new Binding("Point") {Source = point, Mode = BindingMode.TwoWay});
                DrawingGroup2?.Children.Add(new GeometryDrawing(null, StrokePen, lineGeometry));
            }

            LastPoint = point;
        }
    }
}