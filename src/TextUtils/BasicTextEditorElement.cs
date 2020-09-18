using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class BasicTextEditorElement : BasicTextElement, IDisposable
    {
        private DrawingVisual? _drawingVisual;
        private AnimationTimeline _blink;
        private int _cursorPosition = 1;

        static BasicTextEditorElement()
        {
            FocusableProperty.OverrideMetadata(typeof(BasicTextEditorElement), new FrameworkPropertyMetadata(true));
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            if (Client == null)
            {
                return;
            }

            Client.Insert(CursorPosition, e.Text);

            CursorPosition++;
            Update();
            BasicTextVisual?.UpdateVisual(AvailableSize.Width);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            CaptureMouse();
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (_drawingVisual != null)
            {
                var dc = _drawingVisual.RenderOpen();

                var position = e.GetPosition(this);
                var d = position.X;
                foreach (var line in BasicTextVisual.Lines)
                {
                    Debug.WriteLine($"{line.LineNumber} {line.BoundingRect} - {position}");
                }
                foreach (var line in BasicTextVisual.Lines)
                {
                    if (!line.BoundingRect.Contains(position)) continue;
                    Debug.WriteLine("in " + line.LineNumber);
                    if (!(line is LineInfo2BaseImpl i)) continue;
                    // for (int i2 = 0; i2 < 200; i2 += 5)
                    // {
                    // var hitFromDistance = i.TextLine.GetCharacterHitFromDistance(i2);
                    // Debug.WriteLine(hitFromDistance.FirstCharacterIndex);
                    // }
                    var textLineStart = d;//- i.TextLine.Start;
                    Debug.WriteLine(textLineStart);
                    var h = i.TextLine.GetNextCaretCharacterHit(new CharacterHit(line.Offset, 0));
                    h = i.TextLine.GetPreviousCaretCharacterHit(h);
                    var char0 = line.Characters.Skip(h.FirstCharacterIndex - line.Offset).FirstOrDefault();
                    if (char0 != null)
                    {
                        var r = char0.BoundingRect;
                        dc.DrawRectangle(null, new Pen(Brushes.Green, 2), r);
                    }
                    var characterHitFromDistance = i.TextLine.GetCharacterHitFromDistance(textLineStart);
                    var dd = i.TextLine.GetDistanceFromCharacterHit(characterHitFromDistance);
                    Debug.WriteLine((object?) dd);
                    Debug.WriteLine((object?) characterHitFromDistance.TrailingLength);
                    var firstCharacterIndex = characterHitFromDistance.FirstCharacterIndex;
                    Debug.WriteLine((object?) firstCharacterIndex);
                    var char1 = line.Characters.Skip(firstCharacterIndex - line.Offset).FirstOrDefault();
                    if (char1 != null)
                    {
                        var r = char1.BoundingRect;
                        dc.DrawRectangle(null, new Pen(Brushes.Green, 2),r );
                        Debug.WriteLine(char1.Character);
                    }
                }
                dc.Close();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            ReleaseMouseCapture();
        }

        public BasicTextEditorElement()
        {
            TextCaret = new TextCaret1(){LineHeight = 100.0,CaretWidth = 4,Name1="BasicTextEditorElement"};
            Children.Add(TextCaret);
            _drawingVisual = new DrawingVisual();
            Children.Insert(0,_drawingVisual);
            
            var objectKeyFrameCollection = new ObjectKeyFrameCollection()
            {
                new DiscreteObjectKeyFrame(System.Windows.Visibility.Hidden, KeyTime.FromPercent(0)),
                new DiscreteObjectKeyFrame(System.Windows.Visibility.Visible, KeyTime.FromPercent(0.5))
            };
            _blink = new ObjectAnimationUsingKeyFrames() { KeyFrames = objectKeyFrameCollection, Duration = new Duration(new TimeSpan(0,0,1)),RepeatBehavior = RepeatBehavior.Forever};


        }

        public TextCaret1 TextCaret { get; set; }

        public int CursorPosition
        {
            get => _cursorPosition;
            set
            {
                _cursorPosition = value;
                var hit = new CharacterHit(value, 0);

                var lineInfo2BaseImpl = BasicTextVisual.Lines.OfType<LineInfo2BaseImpl>().First(z => z.TextSpan.Contains(value));
                if (value >= lineInfo2BaseImpl.TextSpan.End - 2)
                {
                    CursorPosition = lineInfo2BaseImpl.TextSpan.End;
                    return;
                }
                var c1 =  lineInfo2BaseImpl.Characters
                    .First(c => c.Index == value);
               TextCaret.SetCaretPosition(c1.BoundingRect.X, c1.BoundingRect.Y);
               TextCaret.LineHeight = c1.RenderingEmSize * c1.GlyphTypeface.Height;
               TextCaret.CaretWidth = c1.AdvanceWidth;
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
            if (e.Key == Key.Right)
            {
                e.Handled = true;
                CursorPosition++;
            }
        }

        protected override void OnTextInput(TextCompositionEventArgs e)
        {
            base.OnTextInput(e);
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            TextCaret.BeginAnimation(UIElement.VisibilityProperty, _blink);
        }

        protected override void OnLostKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            TextCaret.BeginAnimation(UIElement.VisibilityProperty, null);

        }

        protected override void BasicTextVisualOnVisualUpdated(object? sender, EventArgs e)
        {
            base.BasicTextVisualOnVisualUpdated(sender, e);
            CursorPosition = CursorPosition;
            // var charInfo = BasicTextVisual.Lines.First().Characters.First();
            // var boundingRect = charInfo.BoundingRect;
            // TextCaret.SetCaretPosition(boundingRect.X, boundingRect.Y);
        }

        public void Dispose()
        {
            TextCaret.Dispose();
        }
    }
}