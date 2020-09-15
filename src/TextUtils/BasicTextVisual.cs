using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class BasicTextVisual : DrawingVisual
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(BasicTextVisual), new PropertyMetadata(default(string?), OnTextChanged));

        private Guid _guid = Guid.NewGuid();
        public string? Text
        {
            get { return (string?) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BasicTextVisual) d).OnTextChanged((string?) e.OldValue, (string?) e.NewValue);
        }



        protected virtual void OnTextChanged(string? oldValue, string? newValue)
        {
            if (Client == null) return;
            Client.Text = newValue;
            Client.LoadSource();
            UpdateVisual();
        }

        public void UpdateVisual(double? width=null)
        {
            Origin = new Point(0, 0);
            ICustomTextSource? textSource = this.Source;
            var d = @"C:\temp\text\" + this._guid;
            // if (!Directory.Exists(dir))
            // {
                // Directory.CreateDirectory(dir);
            // }/
            d += "\\" + _renderCount;
            var drawingContext = this.RenderOpen();
            var width1 = width.GetValueOrDefault(this.ArrangedRect.Width);
            int lineNo = 0;
            var d1 = TR.CreateDelegate2(null, (info, line) =>
            {
                line.AddCharacter(info);
            }, null);

            Lines.Clear();  
            // var size = FormattingHelper.FormatAndDrawLines(d, drawingContext, 
                // this.Client.Default88Properties(), width1, this.TextFormatter1, 
                // textSource.AsTextSource(), this.OutputFormat, textSource.TextBuffer, this.Origin, textSource.Length,
                // (line, point, offset) =>
                // {
                    var lineInfo2Bases = new ILineInfo2Base[10];
                    var drawingGroup = new DrawingGroup();
                    TR.NewMethod2(null,
                        0, textSource!, width1, Origin,0,
                        this.Client.DefaultProperties(),
                        drawingContext,d1,
                        lineInfo2Bases, 0, 1,
                        drawingGroup, null, BatchAction, null);
            drawingContext.Close();
            var size = new Size(Drawing.Bounds.Right, Drawing.Bounds.Bottom);
            Size1 = size;
            if (VisualParent is UIElement e)
            {
                //    e.InvalidateMeasure();
            }

            VisualUpdated?.Invoke(this, EventArgs.Empty);;
            _renderCount++;
        }

        public event EventHandler? VisualUpdated;


        private bool BatchAction(DrawingContext? arg1, 
            DrawingGroup arg2, ILineInfo2Base[] arg3, int arg4, bool arg5, object? arg6)
        {
            foreach (var lineInfo2Base in arg3.Take(arg4))
            {
                Lines.Add(lineInfo2Base);
            }

            return false;
        }

        public ICollection<ILineInfo2Base> Lines { get; set; } = new List<ILineInfo2Base>();

        public string OutputFormat { get; } = "jpg";

        /// <inheritdoc />
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
        }

        public Size Size1
        {
            get { return _size1; }
            set
            {
                _size1 = value;
                Debug.WriteLine($"KTE: Setting Size1 to {value}");
            }
        }

        public TextFormatter TextFormatter1 { get; } = TextFormatter.Create(TextFormattingMode.Ideal);

        private BasicTextSource? _basicTextSource;
        private Point _origin;
        private Size _size1;
        private int _renderCount;
        private Rect _arrangedRect;

        /// <inheritdoc />
        public BasicTextVisual(BasicTextSourceClient? client = null)
        {
            Client = client ??
                     new BasicTextSourceClient(_basicTextSource = new BasicTextSource(new FontFamily("Arial"), 16.0));
            Source = Client.TextSource;
        }

        public ICustomTextSource? Source { get; }

        private BasicTextSourceClient? Client { get; }

        public void Arrange(Rect rect)
        {
            ArrangedRect = rect;

        }

        public Rect ArrangedRect
        {
            get => _arrangedRect;
            set { _arrangedRect = value; }
        }

        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
    }

}        