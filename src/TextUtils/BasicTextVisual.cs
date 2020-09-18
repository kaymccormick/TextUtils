using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class BasicTextVisual : DrawingVisual
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(BasicTextVisual), new PropertyMetadata(default(string?), OnTextChanged));

        public string? Text
        {
            get => (string?) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
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
            var drawingContext = this.RenderOpen();
            var width1 = width.GetValueOrDefault(this.ArrangedRect.Width);
            var d1 = TextRenderer1.CreateDelegate2(null!, (info, line) =>
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
                    TextRenderer1.FormatAndDrawLines(null,
                        0, textSource!, width1, Origin,0,
                        this.Client!.DefaultProperties(),
                        drawingContext,d1,
                        lineInfo2Bases, 0, 1,
                        drawingGroup, null, BatchAction, null);
            drawingContext.Close();
            if (Drawing != null)
            {
                var size = new Size(Drawing.Bounds.Width, Drawing.Bounds.Height);
                Size1 = size;
            }

            if (VisualParent is UIElement)
            {
                //    e.InvalidateMeasure();
            }

            VisualUpdated?.Invoke(this, EventArgs.Empty);
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

        // ReSharper disable once UnusedMember.Global
        public string OutputFormat { get; } = "jpg";

        public Size Size1
        {
            get => _size1;
            set
            {
                _size1 = value;
                Debug.WriteLine($"KTE: Setting Size1 to {value}");
            }
        }

        private Size _size1;

        /// <inheritdoc />
        public BasicTextVisual(BasicTextSourceClient? client = null)
        {
            Client = client ??
                     new BasicTextSourceClient2(new BasicTextSource2(new FontFamily("Arial"), 16.0));
            Source = Client.TextSource;
        }

        public ICustomTextSource? Source { get; }

        private BasicTextSourceClient? Client { get; }

        public void Arrange(Rect rect)
        {
            ArrangedRect = rect;

        }

        public Rect ArrangedRect { get; set; }

        public Point Origin { get; set; }
    }

}        