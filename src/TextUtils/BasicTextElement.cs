using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.TextFormatting; // ReSharper disable once RedundantUsingDirective

namespace TextUtils
{
    [Localizability(LocalizationCategory.Inherit, Readability = Readability.Unreadable)]
    [ContentProperty("Blocks")]
    public class BasicTextElement : FrameworkElement

    {
        protected VisualCollection Children { get; }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(ArrangeOverride)}: {finalSize}");

            var rect = new Rect(finalSize);
            foreach (var visual in Children)
            {
                if (visual is UIElement u)
                {
                    if (_measures.TryGetValue(u, out var r))
                    {
                        double left = 0;
                        double top=0.0;
                        if (u is TextCaret2 tc)
                        {
                            left = tc.ViewportElementLeft;
                            top = tc.ViewportElementTop;
                        }
                        var finalRect = new Rect(new Point(left,top),r);
                        Debug.WriteLine("Arranging wiht " + finalRect);
                        u.Arrange(finalRect);
                        rect.Union(finalRect);
                    }

                }
                else if (visual is BasicTextVisual v)
                {
                    var rect1 = new Rect(v.Size1);
                    Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(ArrangeOverride)}: calling arrange on {v} with {rect1}");
                    v.Arrange(rect1);
                    Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(ArrangeOverride)}: calling UpdateVisual on {v}");
                    v.UpdateVisual();
                    var newSize1 = v.Size1;
                    Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(ArrangeOverride)}: new Size1 is {newSize1}");
                    rect1.Width = newSize1.Width;
                    rect1.Height = newSize1.Height;
                    rect.Union(rect1);
                }
            }
            var arrangeOverride = finalSize;
            Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(ArrangeOverride)}: returning {arrangeOverride}");

            return arrangeOverride;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(null, new Pen(Brushes.Blue, 1), new Rect(DesiredSize));
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            if (_inMeasure)
            {
                return availableSize;
            }

            Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(MeasureOverride)}: {availableSize}");
            _inMeasure = true;
            AvailableSize = availableSize;
            _measures.Clear();
            Rect mySize = new Rect(new Size(0, 0));
            foreach (var visual in Children)
            {
                if (visual is UIElement u)
                {
                    u.Measure(availableSize);
                    var z = u.DesiredSize;
                    _measures[u] = z;
                    mySize.Union(new Rect(z));
                }
                else if (visual is BasicTextVisual v)
                {
                    v.UpdateVisual(availableSize.Width);
                    
                    var rect = new Rect(v.Size1);
                    Debug.WriteLine($"KTE: Trying to determine size of {v} is {v.Size1}");
                    if (v.Size1.Width > availableSize.Width)
                    {
                        rect.Width = availableSize.Width;
                    }

                    if (v.Size1.Height > availableSize.Height)
                    {
                        rect.Height = availableSize.Height;
                    }

                    v.Size1 = rect.Size;
                    mySize.Union(rect);
                }
            }

            _inMeasure = false;
            var mySizeSize = mySize.Size;
            Debug.WriteLine($"KTE: {nameof(BasicTextElement)}: {nameof(MeasureOverride)}: returning {mySizeSize}");
            // return mySizeSize;
            return availableSize;
        }

        static BasicTextElement()
        {
        }

        public static readonly DependencyProperty ForegroundProperty = TextElement.ForegroundProperty.AddOwner(typeof(BasicTextVisual));
        public static readonly DependencyProperty BackgroundProperty = TextElement.BackgroundProperty.AddOwner(typeof(BasicTextVisual));
        public static readonly DependencyProperty TextEffectsProperty = TextElement.TextEffectsProperty.AddOwner(typeof(BasicTextVisual));

        public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(BasicTextVisual));
        public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(BasicTextVisual));
        public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(BasicTextVisual));
        public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(BasicTextVisual));
        public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(BasicTextVisual));
        // public static readonly DependencyProperty VerticalContentAlignmentProperty = TextElement.VerticalContentAlignmentProperty.AddOwner(typeof(BasicTextVisual));
        private BasicTextSourceClientImpl? _basicTextSourceClientImpl;
        private Dictionary<UIElement, Size> _measures = new Dictionary<UIElement, Size>();
        private bool _inMeasure;
        private BasicTextPanel? _panel;
        private BasicTextVisual? _basicTextVisual;
        public Size AvailableSize { get; private set; }

        [Bindable(true)]
        [Category("Appearance")]
        public Brush Foreground
        {
            get => (Brush)this.GetValue(BasicTextElement.ForegroundProperty);
            set => this.SetValue(BasicTextElement.ForegroundProperty, (object)value);
        }
        [Bindable(true)]
        [Category("Appearance")]
        [Localizability(LocalizationCategory.Font)]
        public FontFamily FontFamily
        {
            get => (FontFamily) this.GetValue(BasicTextElement.FontFamilyProperty);
            set => this.SetValue(BasicTextElement.FontFamilyProperty, (object) value);
        }

        [TypeConverter(typeof (FontSizeConverter))]
        [Bindable(true)]
        [Category("Appearance")]
        [Localizability(LocalizationCategory.None)]
        public double FontSize
        {
            get => (double) this.GetValue(BasicTextElement.FontSizeProperty);
            set => this.SetValue(BasicTextElement.FontSizeProperty, (object) value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        public FontStretch FontStretch
        {
            get => (FontStretch) this.GetValue(BasicTextElement.FontStretchProperty);
            set => this.SetValue(BasicTextElement.FontStretchProperty, (object) value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        public FontStyle FontStyle
        {
            get => (FontStyle) this.GetValue(BasicTextElement.FontStyleProperty);
            set => this.SetValue(BasicTextElement.FontStyleProperty, (object) value);
        }

        [Bindable(true)]
        [Category("Appearance")]
        public FontWeight FontWeight
        {
            get => (FontWeight) this.GetValue(BasicTextElement.FontWeightProperty);
            set => this.SetValue(BasicTextElement.FontWeightProperty, (object) value);
        }
        public TextEffectCollection TextEffects
        {
            get => (TextEffectCollection)this.GetValue(TextEffectsProperty);
            set => this.SetValue(TextEffectsProperty, (object)value);
        }


        public BasicTextElement()
        {
            Children = new VisualCollection(this);
            Client = new
                BasicTextSourceClientImpl(new BasicTextSourceImpl(FontFamily, FontSize));
            BasicTextVisual = new BasicTextVisual(Client);
            Children.Add(BasicTextVisual);
        }

        
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public Rect ArrangedRect { get; set; }

        /// <inheritdoc />
        protected override Visual GetVisualChild(int index)
        {
            return Children[index];
        }

        /// <inheritdoc />
        protected override int VisualChildrenCount => Children.Count;

        /// <inheritdoc />
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (Panel != null)
            {
                Update();
            }
        }

        private TextRunProperties InitialProperties()
        {
            var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            FontRendering rendering = 
                
                FontRendering.CreateInstance(FontSize, TextAlignment.Left,
                new TextDecorationCollection(), Foreground, typeface);
            double pixelsPerDip = PixelsPerDip;
            // BaselineAlignment baselineAlignment = BaselineAlignment.Top;
            CultureInfo culture = CultureInfo.CurrentUICulture;
            var genericTextRunProperties = new GenericTextRunProperties(rendering, pixelsPerDip, 
                new TextDecorationCollection(), Foreground,
                FontStyle, FontWeight, Background);
            return genericTextRunProperties;

        }

        public Brush Background
        {
            get => (Brush)this.GetValue(TextElement.BackgroundProperty);
            set => this.SetValue(TextElement.BackgroundProperty, (object)value);
        }


        public double PixelsPerDip { get; set; } = 1.0;


        public List<UIElement> UiElements { get; } = new List<UIElement>();


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BlockCollection Blocks { get;  } = new BlockCollection();

        protected BasicTextVisual? BasicTextVisual
        {
            get => _basicTextVisual;
            set
            {
                if (_basicTextVisual != null) _basicTextVisual.VisualUpdated -= BasicTextVisualOnVisualUpdated;
                _basicTextVisual = value;
                if (_basicTextVisual != null) _basicTextVisual.VisualUpdated += BasicTextVisualOnVisualUpdated;
            }
        }

        protected virtual void BasicTextVisualOnVisualUpdated(object? sender, EventArgs e)
        {
            
        }

        public BasicTextSourceClientImpl? Client
        {
            get { return _basicTextSourceClientImpl; }
            set { _basicTextSourceClientImpl = value; }
        }

        public BasicTextPanel? Panel
        {
            get { return _panel; }
            set
            {
                _panel = value;
                if (IsInitialized)
                {
                    Update();
                }
            }
        }

        protected void Update()
        {
            int startIndex = 0;
            var inCount = 10240;
            char[] textBuffer = new char[inCount];
            var props = InitialProperties();
            Client.ClearSpans();
            Client.DefaultProperties1 = props;
            var paraProps = Client.DefaultProperties();
            
            var count  = TextElementsHelper.ParseBlocks(Client, Blocks, textBuffer, startIndex, inCount, paraProps, props);

            foreach (var uiElement in UiElements)
            {
                var p = (InlineUIContainer) LogicalTreeHelper.GetParent(uiElement);
                if (p != null) p.Child = null;
                Panel.Children.Insert(0, uiElement);
            }

            Client.SetTextBuffer(textBuffer, startIndex, count);
            Client.LoadSource();
        }
    }
}