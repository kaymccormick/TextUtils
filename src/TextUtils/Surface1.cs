using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using JetBrains.Annotations;

namespace TextUtils
{
    [ContentProperty("Children")]
    public class Surface1 : FrameworkElement, INotifyPropertyChanged
    {

        public static readonly RoutedEvent DrawingAdded = EventManager.RegisterRoutedEvent("DrawingAdded",
            RoutingStrategy.Bubble, typeof(DrawingAddedEventHandler), typeof(Surface1));
        /// <inheritdoc />
        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            if (DebugArrange) Debug.WriteLine("Child desired size changed " + child.DesiredSize);
        }

        /// <inheritdoc />
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            InvalidateVisual();
        }

        /// <inheritdoc />
        protected override void OnIsKeyboardFocusedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusedChanged(e);
            if ((bool) e.NewValue) Debug.WriteLine("Got kkeyboasrd focus");
            if (((bool?) e.OldValue).GetValueOrDefault()) Debug.WriteLine("Had keyboard focus");
        }

        /// <inheritdoc />
        public Surface1()
        {
            Children = new ObservableCollection<Visual>();
            Children.CollectionChanged += ChildrenOnCollectionChanged;
            _children = new VisualCollection(this);
            Drawings = new DrawingCollection(10);
            Drawings.Changed += DrawingsOnChanged;
            _colors = ColorHelper.GetColors();
            var x1 = new ObjectAnimationUsingKeyFrames
            {
                // RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(3))
            };

            var c = new ObjectKeyFrameCollection
            {
                new DiscreteObjectKeyFrame(Visibility.Hidden),
                new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromPercent(.2)),
                new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromPercent(.4 / 3)),
                new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromPercent(.2)),
                new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromPercent(.4 / 3)),
                new DiscreteObjectKeyFrame(Visibility.Visible, KeyTime.FromPercent(.2)),
                new DiscreteObjectKeyFrame(Visibility.Hidden, KeyTime.FromPercent(.4 / 3))
            };
            x1.KeyFrames = c;
            BlinkAnimationTimeline = x1;
        }

        private void DrawingsOnChanged(object? sender, EventArgs e)
        {
            foreach (var drawing in Drawings)
            {
                FindOrCreateVisual(drawing, false, out var duo, out _);
                if (duo.PrimaryVisual is
                    DrawingVisual1 dv1 && dv1.SourceDrawing is DrawingGroup dg)
                    if (!dg.Children.Contains(drawing))
                    {
                        var found = false;
                        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(duo.PrimaryVisual); i++)
                        {
                            var c = VisualTreeHelper.GetChild(this, i);
                            if (c is DrawingVisualDuo d1)
                                if (d1.PrimaryVisual is DrawingVisual1 dv2)
                                    if (((DrawingGroup) dv2.SourceDrawing).Children.Contains(drawing))
                                        found = true;
                        }

                        if (found)
                            Debug.WriteLine("found in unexpected place");
                        else
                            // Debug.WriteLine(dv1.Fill.Drawing.Bounds);
                        {
                            dg.Children.Add(drawing);
                            RaiseEvent(new DrawingAddedEventArgs(drawing, duo,this));

                        }

                        // Debug.WriteLine(dv1.Fill.Drawing.Bounds);
                    }
            }

            // _duos.Select(d=>d.PrimaryVisual).OfType<DrawingVisual1>().
        }

        public Brush Stroke
        {
            get => (Brush) GetValue(Shape.StrokeProperty);
            set => SetValue(Shape.StrokeProperty, value);
        }

        public double StrokeThickness
        {
            get => (double) GetValue(Shape.StrokeThicknessProperty);
            set => SetValue(Shape.StrokeThicknessProperty, value);
        }

        public static readonly DependencyProperty StrokeThicknessProperty = Shape.StrokeThicknessProperty;

        static Surface1()
        {
            FocusableProperty.OverrideMetadata(typeof(Surface1),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

            Shape.StrokeProperty.AddOwner(typeof(Surface1),
                new FrameworkPropertyMetadata(Brushes.DodgerBlue,FrameworkPropertyMetadataOptions.AffectsRender));
            Shape.StrokeThicknessProperty.AddOwner(typeof(Surface1),
                new FrameworkPropertyMetadata(5.0d,
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsArrange));

            DrawingHelper.DrawingsProperty.AddOwner(typeof(Surface1));
            LayoutHelper.CodeViewPort2Property.AddOwner(typeof(Surface1));
            LayoutHelper.LineHeightProperty.AddOwner(typeof(Surface1));
            LayoutHelper.IsManagedProperty.AddOwner(typeof(Surface1));
        }

        /// <inheritdoc />
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // Debug.WriteLine(e.GetPosition(this));
            // return;
// #if MOUSEMOVE
            results.Clear();
            VisualTreeHelper.HitTest(this, FilterCallback, ResultCallback,
                new PointHitTestParameters(e.GetPosition(this)));
            var i = 0;
            var startAnims = new List<UIElement>();
            foreach (var (visualHit, _, hit) in results)
                // //Debug.WriteLine($"{i} {visualHit}");

            // //Debug.WriteLine("Submatched object " + hit + "child of " + parent);

            foreach (var valueTuple in results.Select(z => z.VisualHit).Distinct())
            {
                switch (valueTuple)
                {
                    case UIElement fe:
                        startAnims.Add(fe);
                        break;
                    case DrawingVisual1 _:
                        // //Debug.WriteLine(dv1.Drawing.Bounds);
                        // //Debug.WriteLine(XamlWriter.Save(dv1));

                        break;
                    default:
                        throw new Exception($"Unexpected {valueTuple}");
                }

                Debug.WriteLine("startAnims contains " + startAnims.Count);

                foreach (var u in startAnims)
                    if (!anims.Contains(u))
                    {
                        Debug.WriteLine("Starting animation on " + u);
                        u.BeginAnimation(VisibilityProperty, BlinkAnimationTimeline);
                        anims.Add(u);
                    }

                List<UIElement> removeanims = new List<UIElement>();
                foreach (var u in anims)
                    if (!startAnims.Contains(u))
                    {
                        Debug.WriteLine("Ending animation on " + u);
                        u.BeginAnimation(VisibilityProperty, null);
                        removeanims.Add(u);

                    }

                foreach (var y in removeanims)
                {

                    anims.Remove(y);
                }
                
            }

// #endif
        }

        private HitTestFilterBehavior FilterCallback(DependencyObject potentialhittesttarget)
        {
            // if (potentialhittesttarget is GriddedView) return HitTestFilterBehavior.ContinueSkipSelf;
            var ui1 = potentialhittesttarget as UIElement;
            if (ui1 != null && ui1.Visibility != Visibility.Visible)
                // //Debug.WriteLine($"Skipping not visible {ui1}");
                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;

            var isAppHitTestVisible = LayoutHelper.GetIsAppHitTestVisible(potentialhittesttarget);
            var ui1IsHitTestVisible = ui1?.IsHitTestVisible ?? true;
            if (!(isAppHitTestVisible || ui1IsHitTestVisible))
            {
                Debug.WriteLine("Skipping " + potentialhittesttarget + " because IsAppHitVisible is false");
                return HitTestFilterBehavior.ContinueSkipSelf;
            }

            var zindex = LayoutHelper.GetCodeViewZIndex(potentialhittesttarget);
            // //Debug.WriteLine("ZIndex is " + zindex);

            // var lineNo = LayoutHelper.GetLineNumber(potentialhittesttarget);
            // if (lineNo != null) //Debug.WriteLine("Found entity with line number " + lineNo.Value);
            // {
            // }

            // if (potentialhittesttarget is SourceCodeVisual sourceCodeVisual)
            // {
                // return HitTestFilterBehavior.Continue;
            // }
            // else 
            if (potentialhittesttarget is DrawingVisual drawingVisual)
            {
                // //Debug.WriteLine("Found drawingVisual");
                return HitTestFilterBehavior.Continue;
            }
            else if (potentialhittesttarget is HostVisual hostVisual)
            {
            }
            else if (potentialhittesttarget is ContainerVisual cv)

            {
                return HitTestFilterBehavior.Continue;
            }


            if (potentialhittesttarget is Surface1 _)
                return HitTestFilterBehavior.Continue;
            else if (potentialhittesttarget is Panel _)
                return HitTestFilterBehavior.Continue;
            else if (potentialhittesttarget is UIElement _)
                return HitTestFilterBehavior.Continue;
            else if (potentialhittesttarget is DrawingVisual _)
                return HitTestFilterBehavior.Continue;
            else
                throw new Exception("Unexpected " + potentialhittesttarget.GetType().Name);

            // //Debug.WriteLine(potentialhittesttarget + " " + isAppHitTestVisible + " " + uIsHitTestVisible);
            return HitTestFilterBehavior.Continue;
        }

        private HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            IEnumerable<(DependencyObject Key, IEnumerable<DependencyObject>)> r =
                Enumerable.Empty<(DependencyObject, IEnumerable<DependencyObject>)>();

            switch (result)
            {
                case GeometryHitTestResult _:
                    break;
                case AdornerHitTestResult _:
                    break;
                case AppPointHitTestResult appPointHitTestResult:
                    r = appPointHitTestResult.Results;

                    break;
                case RayMeshGeometry3DHitTestResult _:
                    break;
                case RayHitTestResult _:
                    break;
                case PointHitTestResult _:

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }

            var valueTuples = r.SelectMany(z => z.Item2.Select(z1 => (result.VisualHit, z.Key, z1)));
            results.AddRange(valueTuples);

            return HitTestResultBehavior.Continue;
        }


        /// <inheritdoc />
        public override void EndInit()
        {
            base.EndInit();
            foreach (var drawing in Drawings) HandleDrawing(drawing);
            InvalidateMeasure();
        }

        /// <inheritdoc />
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return base.HitTestCore(hitTestParameters);
            // if(VisualTreeHelper.GetContentBounds(this).Contains(hitTestParameters.HitPoint))
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        // /// <inheritdoc />
        // protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        // {
        // base.OnPreviewTextInput(e);
        // ShowInput(e.Text);
        // e.Handled = true;
        // }

        private void ShowInput(string text)
        {
            var glyphTypeface = HitTestHelper.GlyphTypeface1;
            if (glyphTypeface != null)
            {
                var g = glyphTypeface.GetGlyphOutline(glyphTypeface.CharacterToGlyphMap[text.First()], 20.0, 20.0);
                var gd
                    = new GeometryDrawing(Brushes.Black, null, g);
                LayoutHelper.SetViewportElementLeft(gd, 10);
                LayoutHelper.SetViewportElementTop(gd, 000);
                Drawings.Add(gd);
            }
        }

        /// <inheritdoc />
        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
        }

        protected void HandleDrawing(Drawing drawing)
        {
            if (DebugManagement)
                Debug.WriteLine("Handling drawing " + drawing);
#if false
            var portZIndex = ViewPort.GetPortZIndex(drawing);
            var firstOrDefault =
                DrawingRects.FirstOrDefault(r => Panel.GetZIndex(r) == portZIndex && RoslynCodeBase.GetIsManaged(r));
            if (firstOrDefault != null)
            {
                if (firstOrDefault.Fill is DrawingBrush b)
                {
                    if (b.Drawing is DrawingGroup dg)
                    {
                        if (!dg.Children.Contains(drawing))
                            dg.Children.Add(drawing);
                    }
                }
            }
            else
            {
                ViewPort.CreateRect(this, DrawingRects, 0.0, 0.0, drawing, portZIndex);
            }

            // var b = new DrawingBrush(drawing)
            // {
                // Stretch = Stretch.None, AlignmentX = AlignmentX.Left, AlignmentY = AlignmentY.Top,
                // TileMode = TileMode.None
            // };

#else
            FindOrCreateVisual(drawing, true, out _, out _);

            // _brushThings.Add(b);
#endif
        }

        private bool DebugManagement { get; } = true;

        // ReSharper disable once UnusedMethodReturnValue.Local
        private bool FindOrCreateVisual(Drawing drawing, bool add, out DrawingVisualDuo? duo, out int zIndex)
        {
            var rs = false;
            if (FindVisual(drawing, out zIndex, out duo))
            {
                if (add)
                {
                    if (duo.PrimaryVisual is DrawingVisual1 dv1)
                        if (dv1.SourceDrawing is DrawingGroup dgg)
                        {
                            RaiseEvent(new DrawingAddedEventArgs(drawing, duo, this));
                            dgg.Children.Add(drawing);
                            // ReSharper disable once RedundantAssignment
                            rs = true;
                        }

                    rs = false;
                }
                else
                {
                }
            }
            else
            {
                duo = CreateVisual(zIndex);
                if (add)
                    if (duo.PrimaryVisual is DrawingVisual1 dv12)
                    {

                        ((DrawingGroup) dv12.SourceDrawing).Children.Add(drawing);
                        RaiseEvent(new DrawingAddedEventArgs(drawing, duo, this));
                        rs = true;
                    }
            }

            if (!rs) return rs;
            var shrinkToFit = LayoutHelper.GetShrinkToFit(drawing);
            var rectangle = new Rect(0, 0, ActualWidth, ActualHeight);
            Redraw(duo.PrimaryVisual, rectangle, _children.Count, zIndex);
            return rs;
        }

        public DrawingVisualDuo CreateVisual(int zIndex)
        {
            var sourceDrawing = new DrawingGroup();
            var duo = new DrawingVisualDuo() {IsCustomHitTesting = IsCustomHitTesting, DebugMode=DebugMode, FitToSize=FitToSize};
            AddChild(duo, zIndex);
            var dv12 = new DrawingVisual1() {IsCustomHitTesting = IsCustomHitTesting,DebugMode=DebugMode,SourceDrawing = sourceDrawing, FitToSize=FitToSize};
            duo.PrimaryVisual = dv12;
            duo.SecondaryVisual = new DrawingVisual();

            LayoutHelper.SetCodeViewZIndex(duo, zIndex);
            LayoutHelper.SetCodeViewZIndex(dv12, zIndex);
            _dv1.Add(dv12);
            DuoDocuments.Add(duo);

            InvalidateMeasure();

            return duo;
        }

        public bool FindVisual(Drawing drawing, out int zIndex, out DrawingVisualDuo? duo)
        {
            (duo, zIndex) = FindVisualByZindex(drawing);

            if (duo != null) return true;

            return false;
        }

        private (DrawingVisualDuo? firstOrDefault, int codeViewZIndex) FindVisualByZindex(Drawing drawing)
        {
            var codeViewZIndex = LayoutHelper.GetCodeViewZIndex(drawing);
            var zz = DuoDocuments.Where(z => LayoutHelper.GetCodeViewZIndex(z) == codeViewZIndex);
            var firstOrDefault = zz.FirstOrDefault();
            return (firstOrDefault, codeViewZIndex);
        }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            var visualChild = _children[index];

            return visualChild;
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount
        {
            get { return _children.Count; }
        }


        public static readonly DependencyProperty LineHeightProperty = LayoutHelper.LineHeightProperty;

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {


            if (DebugArrange) Debug.WriteLine("MyRect1 ArrangeOverride Size is " + finalSize);

            var size = finalSize;


            var childrenCount = VisualTreeHelper.GetChildrenCount(this);

            if (childrenCount == 0) return finalSize;

            var rect = new Rect(size);
            if (rect.Width > StrokeThickness * 2)
            {
                rect.Width -= StrokeThickness * 2;
                rect.X += StrokeThickness;
            }

            if (rect.Height > StrokeThickness * 2)
            {
                rect.Height -= StrokeThickness * 2;
                rect.Y += StrokeThickness;
            }

            if (DebugArrange)
            {
                Debug.WriteLine("Initial size is " + finalSize);
                Debug.WriteLine("Rect after stroke compaction is " + DebugUtils.RoundRect(rect));
            }

            size = rect.Size;
            var uRect = rect;
            Vector minVector = default; //FindMin(childrenCount);
            Vector v1 = default; //GetRulerVector(childrenCount);
            // //Debug.WriteLine("arrange pass");
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(this, i);
                Rect? finalRect = null;
                var shrinkToFit = LayoutHelper.GetShrinkToFit(child);
                if (DebugArrange)
                {
                    var shrink = shrinkToFit ? "Shrink to fit" : "";
                    Debug.WriteLine($"Found child of type {child.GetType().Name} {shrink}");
                }

                switch (child)
                {
                    case DrawingVisualDuo duo:
                        if (DebugArrange)
                            Debug.WriteLine("Calling custom arrange method on visual with " +
                                            DebugUtils.RoundRect(rect));


                        var rectCopy = rect;
                        rectCopy.Y -= ViewportTop;
                        rectCopy.Height += ViewportTop;
                        duo.Top = rectCopy.Y;
                        duo.Arrange(rectCopy);
                        break;
                    // ReSharper disable once RedundantAssignment
                    case DrawingVisual1 _:
                    {
                        throw new InvalidOperationException();
#if false
                            var viewportTop = ViewportTop;
                        var viewportLeft = ViewportLeft;
                        visualDrawing1.Offset = new Vector(-1 * StrokeThickness, -1 * StrokeThickness);
                        visualDrawing1.ArrangedSize = finalSize;

                        // finalRect = ViewPort.PerformArrange(finalSize, visualDrawing1, i, minVector,
                        // v1, size, null, viewportTop, viewportLeft, ArrangeList,
                        // verticalRulerEnabled,
                        // isRulerEnabled, isHorizontalRulerEnabled
                        // , rect, shrinkToFit
                        // );
                        // Redraw(visualDrawing1, rect, i,
                        // RoslynCodeBase.GetCodeViewZIndex(visualDrawing1), shrinkToFit);

                        break;
#endif
                    }

                    case UIElement uiE:
                    {
                        var viewportTop = ViewportTop;
                        var viewportLeft = ViewportLeft;
                        if (DebugArrange)
                            Debug.WriteLine("Calling generic custom perform arrange");
                        finalRect = LayoutHelper.PerformArrange(finalSize, uiE, i, minVector,
                            v1, size, ss => { }, viewportTop, viewportLeft, ArrangeList
                        );
                        break;
                    }
                }

                if (finalRect != null) uRect.Union(finalRect.Value);
            }

            if (DebugArrange)
            {
                Debug.WriteLine("end arrange pass");


                Debug.WriteLine("MyRect1 ArrangeOverride returning " + uRect.Size);
            }

            return uRect.Size;
        }

        private bool DebugArrange { get; } = false;

        private double ViewportLeft
        {
            get => _viewportLeft;
            set
            {
                _viewportLeft = value;
                InvalidateArrange();
            }
        }

        public double ViewportTop
        {
            get => _viewportTop;
            set
            {
                _viewportTop = value;
                InvalidateArrange();
                // InvalidateVisual();
            }
        }

        private List<string> ArrangeList { get; } = new List<string>();

        // ReSharper disable once SuggestBaseTypeForParameter
        protected virtual DrawingBrush? Redraw([NotNull] Visual vv, Rect rectangle1, int index,
            int getCodeViewZIndex)
        {
            if (vv == null) throw new ArgumentNullException(nameof(vv));
            if (vv is DrawingVisual1 v0)
            {
                v0.DoRender();
                return v0.Fill;
            }

            return null;
        }


        protected static void DrawInfo(DrawingContext dc, Rect r2,
            FormattedText formattedText,
            Point origin)
        {
            dc.DrawRectangle(Brushes.White, new Pen(Brushes.Green, 1), r2);
            dc.DrawText(formattedText,
                origin);
        }


        /// <inheritdoc />
        protected override Size MeasureOverride(Size constraint)
        {
            if (DebugMeasure)
                Debug.WriteLine("MyRect1 MeasureOverride Constraint " + constraint);

            var childrenCount = VisualTreeHelper.GetChildrenCount(this);
            var v = GetRulerVector();
            if (!double.IsNaN(constraint.Width) && !double.IsInfinity(constraint.Width))
                constraint.Width -= v.X;
            else
                constraint.Width = 0;
            if (!double.IsNaN(constraint.Height) && !double.IsInfinity(constraint.Height))
                constraint.Height -= v.Y;
            else
                constraint.Height = 0;
#if false
            if (constraint.Width > StrokeThickness.Left + StrokeThickness.Right)
            {
                constraint.Width -= StrokeThickness.Left + StrokeThickness.Right;
            }
            if (constraint.Height > StrokeThickness.Top + StrokeThickness.Bottom)
            {
                constraint.Height -= StrokeThickness.Top + StrokeThickness.Bottom;
            }
#else
            if (constraint.Width > StrokeThickness * 2) constraint.Width -= StrokeThickness * 2;
            if (constraint.Height > StrokeThickness * 2) constraint.Height -= StrokeThickness * 2;
#endif
            // constraint = new Size(constraint.Width + v.X, constraint.Height + v.Y);
            for (var i = 0; i < childrenCount; i++)
            {
                var dependencyObject = VisualTreeHelper.GetChild(this, i);
                if (dependencyObject is UIElement uiElement)
                {
                    LayoutHelper.PerformMeasure(constraint, i, uiElement);
                    if (DebugMeasure)
                        Debug.WriteLine(uiElement.DesiredSize);
                }
                else if (dependencyObject is Visual vv)
                {
                    if (vv is DrawingVisual1 dv01) dv01.Measure(constraint);
                }
            }

            return constraint;
        }

        private bool DebugMeasure { get; } = false;

        private Vector GetRulerVector()
        {
            return default;
        }

        // ReSharper disable once MemberCanBeProtected.Global
        public double? LineHeight
        {
            get { return (double?) GetValue(LineHeightProperty); }
            // ReSharper disable once UnusedMember.Local
            private set { SetValue(LineHeightProperty, value); }
        }


        public static readonly DependencyProperty DrawingsProperty = DrawingHelper.DrawingsProperty;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DrawingCollection Drawings
        {
            get => (DrawingCollection) GetValue(DrawingsProperty);
            set => SetValue(DrawingsProperty, value);
        }

        protected readonly VisualCollection _children;

        // ReSharper disable once CollectionNeverQueried.Local
        private readonly List<Visual> _dv1 = new List<Visual>();
        private double _viewportTop;
        private double _viewportLeft;


        private void ChildrenOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (Visual item in e.OldItems)
                    _children.Remove(item);

            if (e.NewItems != null)
                foreach (Visual item in e.NewItems)
                    if (item != null)
                    {
                        var zindex = LayoutHelper.GetCodeViewZIndex(item);
                        AddChild(item, zindex);
                    }

            if (e.OldItems?.Count > 0 || e.NewItems?.Count > 0) InvalidateMeasure();
        }

        private void AddChild(Visual item, int zindex)
        {
            if (DebugTree) Debug.WriteLine("Adding " + item);

            if (item is UIElement)
            {
                // uie.OpacityMask = OpacityMask0;
            }
            else if (item is ContainerVisual)
            {
                // cv.OpacityMask = OpacityMask0;
            }

            for (var i = 0; i < _children.Count; i++)
            {
                var codeViewZIndex = LayoutHelper.GetCodeViewZIndex(_children[i]);
                if (codeViewZIndex > zindex)
                {
                    _children.Insert(i, item);
                    return;
                }
            }

            _children.Add(item);
            if (item is DrawingVisualDuo)
            {
                // Debug.WriteLine(VisualTreeHelper.GetContentBounds(item));
            }
        }

        public bool DebugTree { get; } = true;

        public DrawingBrush? OpacityMask0 { get; set; }

        public ObservableCollection<Visual> Children { get; set; }


        public static readonly DependencyProperty RenderModeProperty = DependencyProperty.Register(
            "RenderMode", typeof(RenderMode), typeof(Surface1),
            new FrameworkPropertyMetadata(default(RenderMode),
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender |
                FrameworkPropertyMetadataOptions.AffectsArrange));

        protected readonly IEnumerator<Color> _colors;
        public AnimationTimeline BlinkAnimationTimeline { get; }

        private readonly List<(DependencyObject VisualHit, DependencyObject Key, DependencyObject z1)> results =
            new List<(DependencyObject VisualHit, DependencyObject parent, DependencyObject r)>();

        private readonly List<UIElement> anims = new List<UIElement>();
        public ObservableCollection<DrawingVisualDuo> DuoDocuments { get; } = new ObservableCollection<DrawingVisualDuo>();
        public RenderMode RenderMode
        {
            get { return (RenderMode) GetValue(RenderModeProperty); }
            set { SetValue(RenderModeProperty, (object) value); }
        }

        public bool IsCustomHitTesting { get; set; } = true;
        public bool DebugMode { get; set; } = true;
        public bool FitToSize { get; set; }
        public object? Dragging { get; set; }
        public object? Hover { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = null!;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnDrawingsChanged(DrawingCollection eOldValue, DrawingCollection eNewValue)
        {
            foreach (var drawing in eNewValue)
            {
            }
        }

        /// <inheritdoc />
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
        }

        /// <inheritdoc />
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            var point1 = new Point(0, 0);
            var point3 = new Point(ActualWidth, ActualHeight);
            // var point1 = new Point(StrokeThickness / 2, StrokeThickness / 2);
            // var point3 = new Point(ActualWidth - StrokeThickness / 2, ActualHeight - StrokeThickness / 2);

#if false
            var topPen = new Pen(Stroke, StrokeThickness.Top);
            var point2 = new Point(ActualWidth - StrokeThickness.Right / 2, StrokeThickness.Top / 2);
            drawingContext.DrawLine(topPen, point1, point2);
            topPen.Thickness = StrokeThickness.Right;
            
            var point4 = new Point(StrokeThickness/ 2, ActualHeight - StrokeThickness/ 2);
            topPen.Thickness = StrokeThickness.Bottom;
            drawingContext.DrawLine(topPen, point2, point3);
            topPen.Thickness = StrokeThickness.Bottom;
            drawingContext.DrawLine(topPen, point3, point4);
            topPen.Thickness = StrokeThickness.Left;
            drawingContext.DrawLine(topPen, point4, point1);
#else

            var bb = Stroke.Clone();
            //bb.Opacity = 0.3;
            var pen = new Pen(bb, StrokeThickness);
            var rectangle = new Rect(point1, point3);
            var g = new RectangleGeometry(rectangle);
            var solidColorBrush = new SolidColorBrush(Colors.Gray) {Opacity = 1};
            var d = new GeometryDrawing(null, pen, g);

            var t = HitTestHelper.CreateFormattedText(HasEffectiveKeyboardFocus ? "Keyboard" : "");
            drawingContext.DrawText(t, new Point(3, 3));
            // var p = g.GetFlattenedPathGeometry(0.1, ToleranceType.Absolute);

            // //Debug.WriteLine(XamlWriter.Save(p));
            // drawingContext.DrawGeometry(null, pen, p);
            drawingContext.DrawRectangle(null, pen, rectangle);


#endif
            
        }

        /// <inheritdoc />
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            switch (e.Key)
            {
                case Key.Tab when Keyboard.Modifiers == ModifierKeys.Control:
                    var zIndices = DuoDocuments.Select(LayoutHelper.GetCodeViewZIndex).GroupBy<int, int>(v => v).Select(z => z.Key)
                        .ToArray();
                    Debug.WriteLine(string.Join(", ", zIndices));

                    break;
            }
        }
    }
}