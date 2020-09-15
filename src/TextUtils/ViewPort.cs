using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public sealed class ViewPort : DependencyObject
    {
        // public static readonly DependencyProperty TextGuideLinesPenThicknessProperty = DependencyProperty.Register(
        // "TextGuideLinesPenThickness", typeof(double), typeof(ViewPort),
        // new PropertyMetadata(1.0, TextGuideLinesPenThicknessPropertyChanged));

        // public static readonly DependencyProperty ViewportLeftProperty = DependencyProperty.Register(
        // "ViewportLeft", typeof(double), typeof(ViewPort),
        // new FrameworkPropertyMetadata(default(double),
        // FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsArrange,
        // OnViewportLeftChanged));

        // public double ViewportLeft
        // {
        // get { return (double) GetValue(ViewportLeftProperty); }
        // set { SetValue(ViewportLeftProperty, value); }
        // }

        // private static void OnViewportLeftChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        // {
        // ((ViewPort) d).OnViewportLeftChanged((double) e.OldValue, (double) e.NewValue);
        // }


        // protected virtual void OnViewportLeftChanged(double oldValue, double newValue)
        // {
        // }

        // private static void TextGuideLinesPenThicknessPropertyChanged(DependencyObject d,
        // DependencyPropertyChangedEventArgs e)
        // {

        // var codeViewPort = ((ViewPort) d);
        // codeViewPort.CoerceValue(TextGuideLinesPenProperty);
        // var textGuideLinesPen = codeViewPort.TextGuideLinesPen;
        // if (textGuideLinesPen != null)
        // textGuideLinesPen.Thickness = (double) e.NewValue;
        // else
        // {

        // codeViewPort.TextGuideLinesPen = new Pen(codeViewPort.LineGuideLinesBrush, (double) e.NewValue);
        // }
        // }

        // public double TextGuideLinesPenThickness
        // {
        // get { return (double) GetValue(TextGuideLinesPenThicknessProperty); }
        // set { SetValue(TextGuideLinesPenThicknessProperty, value); }
        // }

        static ViewPort()
        {
            // RoslynCodeBase.IsRulerEnabledProperty.AddOwner(typeof(ViewPort),
                // new FrameworkPropertyMetadata(false,
                    // FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));
            // RoslynCodeBase.IsRulerTextLineGuideLinesEnabledProperty.AddOwner(typeof(ViewPort));
            // RoslynCodeBase.IsRulerHorizontalLinesEnabledProperty.AddOwner(typeof(ViewPort),
                // new FrameworkPropertyMetadata(RoslynCodeBase.IsRulerEnabledProperty.DefaultMetadata.DefaultValue,
                    // FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsMeasure));

            // RoslynCodeBase.IsRulerVerticalLinesEnabledProperty.AddOwner(typeof(ViewPort));

            LayoutHelper.LineHeightProperty.AddOwner(typeof(ViewPort));
        }


        public static readonly RoutedEvent EffectiveViewportTopChangedEvent =
            EventManager.RegisterRoutedEvent("EffectiveViewportTopChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>), typeof(ViewPort));

        public static readonly RoutedEvent EffectiveViewportLeftChangedEvent =
            EventManager.RegisterRoutedEvent("EffectiveViewportLeftChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>),
                typeof(ViewPort));

        // public static readonly DependencyProperty NumHorizontalTicksPerMajorTickProperty = DependencyProperty.Register(
        // "NumHorizontalTicksPerMajorTick", typeof(int), typeof(ViewPort), new PropertyMetadata(5));

        // public int NumHorizontalTicksPerMajorTick
        // {
        // get { return (int) GetValue(NumHorizontalTicksPerMajorTickProperty); }
        // set { SetValue(NumHorizontalTicksPerMajorTickProperty, value); }
        // }

        // public static readonly DependencyProperty NumVerticalTicksPerMajorTickProperty = DependencyProperty.Register(
        // "NumVerticalTicksPerMajorTick", typeof(int), typeof(ViewPort),
        // new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // public int NumVerticalTicksPerMajorTick
        // {
        // get { return (int) GetValue(NumVerticalTicksPerMajorTickProperty); }
        // set { SetValue(NumVerticalTicksPerMajorTickProperty, value); }
        // }

        // public static readonly DependencyProperty VerticalMinorTickPeriodProperty = DependencyProperty.Register(
        // "VerticalMinorTickPeriod", typeof(double), typeof(ViewPort),
        // new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // public double VerticalMinorTickPeriod
        // {
        // get { return (double) GetValue(VerticalMinorTickPeriodProperty); }
        // set { SetValue(VerticalMinorTickPeriodProperty, value); }
        // }

        // public static readonly DependencyProperty HorizontalMinorTickPeriodProperty = DependencyProperty.Register(
        // "HorizontalMinorTickPeriod", typeof(double), typeof(ViewPort),
        // new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        // public double HorizontalMinorTickPeriod
        // {
        // get { return (double) GetValue(HorizontalMinorTickPeriodProperty); }
        // set { SetValue(HorizontalMinorTickPeriodProperty, value); }
        // }


        public ViewPort(Visual? parent)
        {
            Drawings = new DrawingCollection(10);
            TextDrawingGroup = new DrawingGroup();
            TextDrawingGroup.Children.Add(new DrawingGroup());
            
            LayoutHelper.SetCodeViewZIndex(TextDrawingGroup, 0);

            if (parent != null)
            {
                ReparentUiElement = parent;
            }
        }


        public DrawingGroup TextDrawingGroup { get; }

        public double ViewportTop
        {
            get { return (double) GetValue(LayoutHelper.ViewportTopProperty); }
            set { SetValue(LayoutHelper.ViewportTopProperty, value); }
        }


        private void OnViewportTopChanged(double oldValue, double newValue)
        {
        }



        public bool RenderTestRectangle =>false;


        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DrawingCollection Drawings
        {
            get { return (DrawingCollection) GetValue(DrawingHelper.DrawingsProperty); }
            set { SetValue(DrawingHelper.DrawingsProperty, value); }
        }

        // private static void OnDrawingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        // {
        //     ViewPort? v = null;
        //     if (d is ViewPort cvp2)
        //     {
        //         v = cvp2; //.OnDrawingsChanged((Collection<Drawing>)e.OldValue, (Collection<Drawing>)e.NewValue);
        //     }
        //     else
        //     {
        //         if (d is CodeViewportPanel2 p2) v = p2.CodeViewport2;
        //     }
        //
        //     if (v != null) v.OnDrawingsChanged((DrawingCollection) e.OldValue, (DrawingCollection) e.NewValue);
        //     else if (d is MyRect1 mr1)
        //         mr1.OnDrawingsChanged((DrawingCollection) e.OldValue, (DrawingCollection) e.NewValue);
        //
        //     // ((ViewPort) d).OnDrawingsChanged((Collection<Drawing>) e.OldValue, (Collection<Drawing>) e.NewValue);
        // }
        //
   


        public static readonly DependencyProperty DrawingsSourceProperty = DependencyProperty.Register(
            "DrawingsSource", typeof(IEnumerable), typeof(ViewPort),
            new PropertyMetadata(default(IEnumerable), OnDrawingsBindingChanged));

        public IEnumerable DrawingsSource
        {
            get { return (IEnumerable) GetValue(DrawingsSourceProperty); }
            set { SetValue(DrawingsSourceProperty, value); }
        }

        private static void OnDrawingsBindingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewPort) d).OnDrawingsBindingChanged((IEnumerable) e.OldValue, (IEnumerable) e.NewValue);
        }


        protected void OnDrawingsBindingChanged(IEnumerable oldValue, IEnumerable newValue)
        {
        }


        public static readonly DependencyProperty VerticalLineSpacingProperty = DependencyProperty.Register(
            "VerticalLineSpacing", typeof(double), typeof(ViewPort),
            new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double VerticalLineSpacing
        {
            get { return (double) GetValue(VerticalLineSpacingProperty); }
            set { SetValue(VerticalLineSpacingProperty, value); }
        }

        public static readonly DependencyProperty HorizontalLineSpacingProperty = DependencyProperty.Register(
            "HorizontalLineSpacing", typeof(double), typeof(ViewPort),
            new FrameworkPropertyMetadata(100.0,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.Inherits));

        public double HorizontalLineSpacing
        {
            get { return (double) GetValue(HorizontalLineSpacingProperty); }
            set { SetValue(HorizontalLineSpacingProperty, value); }
        }

        public static readonly DependencyProperty HorizontalLabelSpacingProperty = DependencyProperty.Register(
            "HorizontalLabelSpacing", typeof(double), typeof(ViewPort),
            new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsMeasure));


        public double HorizontalLabelSpacing
        {
            get { return (double) GetValue(HorizontalLabelSpacingProperty); }
            set { SetValue(HorizontalLabelSpacingProperty, value); }
        }

        public static readonly DependencyProperty VerticalLabelSpacingProperty = DependencyProperty.Register(
            "VerticalLabelSpacing", typeof(double), typeof(ViewPort),
            new FrameworkPropertyMetadata(50.0, FrameworkPropertyMetadataOptions.AffectsMeasure));

        public double VerticalLabelSpacing
        {
            get { return (double) GetValue(VerticalLabelSpacingProperty); }
            set { SetValue(VerticalLabelSpacingProperty, value); }
        }

        public static readonly DependencyProperty TextGuideLinesPenProperty = DependencyProperty.Register(
            "TextGuideLinesPen", typeof(Pen), typeof(ViewPort),
            new FrameworkPropertyMetadata(new Pen(Brushes.Lime, 1) {DashStyle = new DashStyle(new double[] {6, 6}, 3)},
                FrameworkPropertyMetadataOptions.AffectsMeasure, PropertyChangedCallback1));

        public static readonly DependencyProperty TextGuideLinesDashStyleProperty = DependencyProperty.Register(
            "TextGuideLinesDashStyle", typeof(DashStyle), typeof(ViewPort),
            new PropertyMetadata(new DashStyle(new double[] {6, 6}, 3), OnpropertyNameChanged));

        public DashStyle TextGuideLinesDashStyle
        {
            get { return (DashStyle) GetValue(TextGuideLinesDashStyleProperty); }
            set { SetValue(TextGuideLinesDashStyleProperty, value); }
        }

        private static void OnpropertyNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ViewPort) d).OnpropertyNameChanged((DashStyle) e.OldValue, (DashStyle) e.NewValue);
        }


        private void OnpropertyNameChanged(DashStyle oldValue, DashStyle newValue)
        {
            CoerceValue(TextGuideLinesPenProperty);
        }



        private static void PropertyChangedCallback1(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }


        public Pen TextGuideLinesPen
        {
            get { return (Pen) GetValue(TextGuideLinesPenProperty); }
            set { SetValue(TextGuideLinesPenProperty, value); }
        }
        public bool DebugArrange { get; } = true;

        public double ViewportLeft
        {
            get { return _viewportLeft; }
            set { _viewportLeft = value; }
        }

        private Visual ReparentUiElement
        {
            get { return _reparentUiElement; }
            set
            {
                _reparentUiElement = value;
            }
        }

        private List<string> ArrangeList
        {
            get { return _arrangeList; }
        }

        // private Vector FindMin(int childrenCount)
        // {
        //     var minTop = 0.0;
        //     var minLeft = 0.0;
        //
        //     for (var i = 0; i < childrenCount; i++)
        //     {
        //         var child = (UIElement) VisualTreeHelper.GetChild(this, i);
        //         if (!(child is Rectangle shape) || !(shape.Fill is DrawingBrush brush) ||
        //             !(brush?.Drawing is DrawingGroup g)) continue;
        //         var zz = g.Transform?.Inverse.TransformBounds(g.Bounds) ?? g.Bounds;
        //         minTop = Math.Min(minTop, zz.Top);
        //         minLeft = Math.Min(minLeft, zz.Left);
        //     }
        //
        //     return new Vector(minLeft, minTop);
        // }
        //
        //
        public static readonly DependencyProperty LineGuideLinesBrushProperty = DependencyProperty.Register(
            "LineGuideLinesBrush", typeof(Brush), typeof(ViewPort),
            new FrameworkPropertyMetadata(Brushes.LawnGreen, FrameworkPropertyMetadataOptions.AffectsMeasure,
                PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var codeViewPort = (ViewPort) d;
            codeViewPort.CoerceValue(TextGuideLinesPenProperty);
            // var textGuideLinesPen = codeViewPort.TextGuideLinesPen;
            // if (textGuideLinesPen != null)
            // textGuideLinesPen.Brush = (Brush) e.NewValue;
            // else
            // {

            // codeViewPort.TextGuideLinesPen = new Pen((Brush) e.NewValue, codeViewPort.TextGuideLinesPenThickness);
            // }
        }

        public static readonly DependencyProperty EffectiveViewportLeftProperty = DependencyProperty.Register(
            "EffectiveViewportLeft", typeof(double), typeof(ViewPort), new PropertyMetadata(default(double), d2));

        private static void d2(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ((ViewPort) d).RaiseEvent(new RoutedPropertyChangedEventArgs<double>((double) e.OldValue,
            // (double) e.NewValue, EffectiveViewportLeftChangedEvent));
        }

        public double EffectiveViewportLeft
        {
            get { return (double) GetValue(EffectiveViewportLeftProperty); }
            set { SetValue(EffectiveViewportLeftProperty, value); }
        }

        public static readonly DependencyProperty EffectiveViewportTopProperty = DependencyProperty.Register(
            "EffectiveViewportTop", typeof(double), typeof(ViewPort),
            new PropertyMetadata(default(double), OnEffectiveViewportTopChanged));


        private readonly List<string> _arrangeList = new List<string>();
        private Visual? _reparentUiElement;
        private double _viewportLeft;

        private static void OnEffectiveViewportTopChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // ((ViewPort) d).RaiseEvent(new RoutedPropertyChangedEventArgs<double>((double) e.OldValue,
            // (double) e.NewValue, EffectiveViewportTopChangedEvent));
        }

        public double EffectiveViewportTop
        {
            get { return (double) GetValue(EffectiveViewportTopProperty); }
            set { SetValue(EffectiveViewportTopProperty, value); }
        }

        public Brush LineGuideLinesBrush
        {
            get { return (Brush) GetValue(LineGuideLinesBrushProperty); }
            set { SetValue(LineGuideLinesBrushProperty, value); }
        }

        private double PixelsPerDip { get; } = 1;


        public Vector GetRulerVector(int? childrenCount)
        {
            childrenCount ??= VisualTreeHelper.GetChildrenCount(this);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = (UIElement) VisualTreeHelper.GetChild(this, i);

                if (child is Ruler r) return new Vector(LayoutHelper.GetViewportElementLeft(r), LayoutHelper.GetViewportElementTop(r));

                // //Debug.WriteLine(child.DesiredSize);
            }

            return new Vector(0, 0);
        }
    }
}