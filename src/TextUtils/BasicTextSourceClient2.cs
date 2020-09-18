using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using JetBrains.Annotations;
using Castle.DynamicProxy;

namespace TextUtils
{
    public class BasicTextSourceClient2 : BasicTextSourceClient, IBasicTextSourceClient2
    {
        [CanBeNull] private readonly BasicTextSource2 _textSource;

        /// <inheritdoc />
        public BasicTextSourceClient2([NotNull] BasicTextSource2? textSource) : base(textSource)
        {
            _textSource = textSource ?? throw new ArgumentNullException(nameof(textSource));
        }

        public void AddSpan(TextSpan<CharacteristicsImpl> textSpan)
        {
            _textSource.AddSpan(textSpan);
        }


        public void SetTextBuffer(char[] textBuffer, int startIndex, in int length)
        {
            _textSource.SetTextBuffer(textBuffer, startIndex, length);
        }

        /* Insert text into buffer */
        public void Insert(in int cursorPosition, string text)
        {
            var i = cursorPosition;
            var sp = _textSource.Spans.FirstOrDefault(z => z.Value.StartIndex + z.Value.Length > i); switch (sp?.Value.TextElement)
            {
                case BlockUIContainer _:
                    break;
                case List _:
                    break;
                case Paragraph _:

                    break;
                case Section _:
                    break;
                case Table _:
                    break;
                case Block _:
                    break;
                case Figure _:
                    break;
                case Floater _:
                    break;
                case AnchoredBlock _:
                    break;
                case Bold _:
                    break;
                case Hyperlink _:
                    break;
                case InlineUIContainer _:
                    break;
                case Italic _:
                    break;
                case LineBreak _:
                    break;
                case Run run:
                    var i1 = cursorPosition - sp.Value.StartIndex;
                    run.Text = run.Text.Substring(0, i1) + text + run.Text.Substring(i1);

                    break;
                case Underline _:
                    break;
                case Span _:
                    break;
                case Inline _:
                    break;
                case ListItem _:
                    break;
                case TableCell _:
                    break;
                case TableRow _:
                    break;
                case TableRowGroup _:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ClearSpans()
        {
            _textSource.Spans.Clear();
        }
    }
}