using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace TextUtils
{
    public class DesignSurface : Control
    {

        private DrawingVisualDuo _duo;
        private DrawingVisual1 _dv1;
        private Point _startPoint;
        private bool _inOperation;
        private PolyLineElement? _polyLineElement;
        private bool _leftPressed;
        private Point _elemStart;
        private Surface2? _surface;
        private DrawingVisual1? _drawingVisual1;
        private Color _foregroundColor;
        public int ElementNum { get; set; } = 0;

        static DesignSurface()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignSurface), new FrameworkPropertyMetadata(typeof(DesignSurface)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Surface = (Surface2?) GetTemplateChild("Surface");
            Populate();
        }

        private bool Populate()
        {
            if (Surface == null) return false;
            foreach (var graphicsElement in GraphicsElements)
            {
                Colors.MoveNext();
                graphicsElement.ElementNum = ElementNum++;
                graphicsElement.DebugColor = Colors.Current;
                var visual = graphicsElement.GetVisual(1);
                Lines(graphicsElement);
                foreach (var graphicsElementPoint in graphicsElement.Points)
                {
                    AddPoint(graphicsElement, graphicsElementPoint);

                }
                PointAdorners(graphicsElement);

                if (visual != null) Surface.Children.Add(visual);
            }

            return true;
        }

        public Surface2? Surface
        {
            get => _surface;
            set
            {
                _surface = value;
                if (value == null) return;
                _duo = value.CreateVisual(100);
                DrawingVisual1 = (DrawingVisual1) _duo.PrimaryVisual;
            }
        }

        public DrawingVisual1? DrawingVisual1
        {
            get => _drawingVisual1;
            set
            {
                _drawingVisual1 = value;
                OperationVisual = value;
            }
        }

        public DesignSurface()
        {
            Colors = ColorHelper.GetColors();
            PolyLineTool = new PolyLineTool(this);
        }

        public IEnumerator<Color> Colors { get; set; }

        /// <inheritdoc />
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            if (e.Handled)
                return;
            var point = e.GetPosition(Surface);
            Tool.OnMouseDown(this, e);
            // if (!new Rect(Surface!.RenderSize).Contains(point))
            // return;
            // e.Handled = true;
            // if (_polyLineElement != null && _inOperation)
            // {
               
            // _startPoint = point;

            // }

            // if (!(e.OriginalSource is GraphicsElement ge)) return;
            // if (ge.OverBorder)
            // ge.Pressed = true;

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Handled)
                return;
            Tool.OnMouseUp(this, e);
            // _dv1.RenderOpen().Close();
            // _inOperation = false;
            // PlaceGraphicsElement(p);
            // Tool = Tools.Pointer;
            // break;
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            Debug.WriteLine($"{nameof(DesignSurface)}: HitTestCore: {hitTestParameters.HitPoint}");
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        public Color ForegroundColor
        {
            get => _foregroundColor;
            set
            {
                _foregroundColor = value;
                PolyLineTool.ForegroundColor = value;
            }
        }

        public static readonly DependencyProperty ToolProperty = DependencyProperty.Register(
            "Tool", typeof(Tool), typeof(DesignSurface), new PropertyMetadata(default(Tool), ToolChanged));

        private static void ToolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var designSurface = (DesignSurface)d;
            var newTool = (Tool?)e.NewValue;
            var            enabled = false;
            if (newTool is PointerTool)
                enabled = true;
            var designSurfaceSurface = designSurface.Surface;
            if (designSurfaceSurface != null)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(designSurfaceSurface);
                if (adornerLayer != null)
                {
                    var enumerable = adornerLayer.GetAdorners(designSurfaceSurface);
                    if (enumerable != null)
                        foreach (var adorner in enumerable)
                        {
                            switch (adorner)
                            {
                                case PointAdorner pa:
                                    pa.IsEnabled = enabled;
                                    pa.InvalidateVisual();
                                    break;
                                case LineAdorner la:
                                    la.IsEnabled = enabled;
                                    la.InvalidateVisual();
                                    break;
                            }
                        }
                }
            }
        }

        public Tool Tool
        {
            get => (Tool) GetValue(ToolProperty);
            set => SetValue(ToolProperty, (object) value);
        }
        private void PlaceGraphicsElement(Point p)
        {
            var height = Math.Abs(p.Y - _elemStart.Y);
            var width = Math.Abs(p.X - _elemStart.X);
            var tb = new TextBox
            {
                Text = "Hello",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // tb.PreviewKeyDown += TbOnPreviewKeyDown;
            // tb.GotKeyboardFocus += TbOnGotKeyboardFocus;
            var b = new Border
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Child = tb
            };
            const double i = 2.5;
            var left = Math.Min(p.X, _elemStart.X) - i;
            var top = Math.Min(p.Y, _elemStart.Y) - i;
            var graphicsElement = new GraphicsElement
            {
                Width = width + i * 2,
                Height = height + i * 2,
                ViewportElementLeft = left,
                ViewportElementTop = top,
                Rect = new Rect(0, 0, width, height)
            };

            b.SetBinding(WidthProperty, new Binding("ActualWidth") { Source = graphicsElement });
            b.SetBinding(HeightProperty, new Binding("ActualHeight") { Source = graphicsElement });
            b.SetBinding(Canvas.LeftProperty, new Binding("ViewportElementLeft") { Source = graphicsElement });
            b.SetBinding(Canvas.TopProperty, new Binding("ViewportElementTop") { Source = graphicsElement });
            // Canvas.Children.Add(b);
            // Canvas.Children.Add(graphicsElement);
            graphicsElement.Focus();
        }

        /// <inheritdoc />
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            // Debug.WriteLine(e.GetPosition(this));
            if (e.Handled)
                return;
            if (Surface!.Hover != null || Surface.Dragging != null)
                return;
            if (e.OriginalSource is GraphicsElement)
                Debug.WriteLine("original source is graphics element");
            else if (e.Source is GraphicsElement) Debug.WriteLine("source is graphics element");
            var p = e.GetPosition(Surface);
            var r = VisualTreeHelper.HitTest(Surface, p);
            if (r != null)
                if (r.VisualHit is Surface2)
                {
                }
                else
                {
                    Debug.WriteLine(r.VisualHit);

                }
            if (!new Rect(Surface.RenderSize).Contains(p))
            {
                // Debug.WriteLine("ignoring event because outside of surface");
                return;
            }

            Tool.OnPreviewMouseMove(this, e);
            // var d = new GeometryDrawing(Brushes.Black, null, new EllipseGeometry(p, 3, 3));
            // ProtoLogger2.Instance.LogAction(JsonSerializer.Serialize(new ClientRenderRequest
            // { DrawingXaml = XamlWriter.Save(d), DrawingIdentifier = "new1" }));

            // if (!_inOperation) return;
            // switch (Tool)
            // {
            // case TextTool _:
            // {
            // var dc = _dv1.RenderOpen();
            // var rect = new Rect(_elemStart, p);
            // dc.DrawRectangle(null, new Pen(Brushes.Black, 4) { DashStyle = DashStyles.Dash }, rect);
            // dc.Close();
            // break;
            // }
            // case PolyLineTool _:
            // {
            // var dc = _dv1.RenderOpen();
            // dc.DrawLine(new Pen(Brushes.Black, 4) { DashStyle = DashStyles.Dash }, _startPoint, p);
            // dc.Close();
            // break;
            // }
            // }
        }

        private void OnSaveExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var dg = new DrawingGroup();
            foreach (var basicsChild in Surface!.Children)
            {
                if (!(basicsChild is IGraphicsElement ge)) continue;
                var g = ge.AsDrawing();
                dg.Children.Add(g);
            }

            using var fileStream = new FileStream("drawing.xaml", FileMode.Create);
            XamlWriter.Save(dg, fileStream);

        }

        private void OnSetBrushCommand(object sender, ExecutedRoutedEventArgs e)
        {
            Window w = new ColorPickerWindow();
            w.ShowDialog();
        }

        public void Load(string drawingXaml)
        {
            GraphicsElements.Clear();
            try
            {
                var d = (DrawingGroup) XamlReader.Load(new FileStream(drawingXaml, FileMode.Open));
                foreach (var child in d.Children)
                {
                    NewMethod(child);
                }

                if (Surface != null)
                {
                    Populate();
                }
            }
            catch(IOException ex)

            {
                // ignored
            }
        }

        private void NewMethod(Drawing child)
        {
            switch (child)
            {
                case DrawingGroup dg:
                    foreach (var dgChild in dg.Children)
                    {.
                        ++
                            NewMethod(dgChild);
                    }

                    break;
                case GeometryDrawing geometryDrawing:
                    switch (geometryDrawing.Geometry)
                    {
                        case CombinedGeometry _:
                            break;
                        case EllipseGeometry eg:
                            
                            var g = new GeometryElement(geometryDrawing.Geometry);
                            List<Point> points = new List<Point>();
                            var pointElement = new PointElement(eg.Center);
                            BindingOperations.SetBindi3ng(eg, EllipseGeometry.CenterProperty, new Binding("Point"){Source=pointElement});
                            g.AddPoint(pointElement);
#if false
                            foreach (var pathFigure in geometryDrawing.Geometry.GetFlattenedPathGeometry().Figures)
                            {
                                points.Add(pathFigure.StartPoint);
                                foreach (var pathFigureSegment in pathFigure.Segments)
                                {
                                    switch(pathFigureSegment)
                                    {
                                        case LineSegment lineSegment:
                                            points.Add(lineSegment.Point);
                                            break;
                                        case PolyLineSegment polyLineSegment:
                             foreach (var point in polyLineSegment.Points)
                             {
                                 points.Add(point);
                             }
                                            break;
                                        default:
                                            throw new ArgumentOutOfRangeException(nameof(pathFigureSegment));
                                    }
                                }
                            }
                            foreach (var point in points)
                            {
                                g.AddPoint(point);
                            }
#endif
                            PointAdorners(g);
                            GraphicsElements.Add(g);
                            break;
                        case GeometryGroup _:
                            break;
                        case LineGeometry _:
                            break;
                        case PathGeometry pathGeometry:
                            var figure = pathGeometry.Figures[0];
                            var pe = PolyLineTool.CreatePolyLineElement(this);
                            var p = figure.StartPoint;
                            pe.AddPoint(p);
                            foreach (var figureSegment in figure.Segments)
                            {
                                switch (figureSegment)
                                {
                                    case ArcSegment _:
                                        break;
                                    case BezierSegment _:
                                        break;
                                    case LineSegment _:
                                        break;
                                    case PolyBezierSegment _:
                                        break;
                                    case PolyLineSegment polyLineSegment:
                                        foreach (var point in polyLineSegment.Points)
                                        {
                                            pe.AddPoint(point);
                                        }

                                        break;
                                    case PolyQuadraticBezierSegment _:
                                        break;
                                    case QuadraticBezierSegment _:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException(nameof(figureSegment));
                                }
                            }

                            pe.Fill = geometryDrawing.Brush;
                            pe.Complete();
                            GraphicsElements.Add(pe);
                            break;
                        case RectangleGeometry _:
                            break;
                        case StreamGeometry _:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case GlyphRunDrawing _:
                    break;
                case ImageDrawing _:
                    break;
                case VideoDrawing _:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(child));
            }
        }

        public PolyLineTool PolyLineTool { get; set; }

        public ObservableCollection<IGraphicsElement> GraphicsElements { get; set; } = new ObservableCollection<IGraphicsElement>();
        public DrawingVisual? OperationVisual { get; set; }
        public bool InOperation => Tool.InOperation;

        public void AddElement(Visual visual, IGraphicsElement item)
        {
            GraphicsElements.Add(item);
            Surface?.Children.Add(visual);
        }

        public void AddPoint(IGraphicsElement graphicsElement, PointElement pointElement)
        {
            if (Surface == null) return;
            // var al = AdornerLayer.GetAdornerLayer(Surface);
            // var adorner = new PointAdorner(Surface, pointElement);
            // al.Add(adorner);
        }

        public void Complete(IGraphicsElement g)
        {
            if (Surface == null) return;
            Lines(g);
            
            PointAdorners(g);
        }

        private void PointAdorners(IGraphicsElement g)
        {
            if (Surface == null) return;
            foreach (var pointElement in g.Points)
            {
                {
                    var al = AdornerLayer.GetAdornerLayer(Surface);
                    var adorner = new PointAdorner(Surface, pointElement);
                    adorner.IsEnabled = Tool is PointerTool;
                    al.Add(adorner);
                }
            }
        }

        private void Lines(IGraphicsElement g)
        {
            if (g.Lines != null)
                foreach (var lineGeometry in g.Lines)
                {
                    var al = AdornerLayer.GetAdornerLayer(Surface);
                    Debug.WriteLine((string?) lineGeometry.ToString());
                    var adorner = new LineAdorner(Surface, lineGeometry);
                    adorner.IsEnabled = Tool is PointerTool;
                    al.Add(adorner);
                }
        }
    }
}