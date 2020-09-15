using System;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media.TextFormatting;
using JetBrains.Annotations;

namespace TextUtils
{
    public class BasicTextSourceClientImpl : BasicTextSourceClient, IBasicTextSourceClientImpl
    {
        [CanBeNull] private readonly BasicTextSourceImpl? _textSource;

        /// <inheritdoc />
        public BasicTextSourceClientImpl([CanBeNull] BasicTextSourceImpl? textSource) : base(textSource)
        {
            _textSource = textSource;
        }

        public void AddSpan(TextSpan<CharacteristicsImpl> textSpan)
        {
            _textSource.AddSpan(textSpan);
        }


        public void SetTextBuffer(char[] textBuffer, int startIndex, in int length)
        {
            _textSource.SetTextBuffer(textBuffer, startIndex, length);
        }

        public void Arrange(Rect rect)
        {
            
        }

        public void Insert(in int cursorPosition, string text)
        {
            var i = cursorPosition;
            var sp = Enumerable.FirstOrDefault<TextSpan<CharacteristicsImpl>>(_textSource.Spans, z => z.Value.StartIndex + z.Value.Length > i); switch (sp.Value.TextElement)
            {
                case BlockUIContainer blockUiContainer:
                    break;
                case List list:
                    break;
                case Paragraph paragraph:

                    break;
                case Section section:
                    break;
                case Table table:
                    break;
                case Block block:
                    break;
                case Figure figure:
                    break;
                case Floater floater:
                    break;
                case AnchoredBlock anchoredBlock:
                    break;
                case Bold bold:
                    break;
                case Hyperlink hyperlink:
                    break;
                case InlineUIContainer inlineUiContainer:
                    break;
                case Italic italic:
                    break;
                case LineBreak lineBreak:
                    break;
                case Run run:
                    var i1 = cursorPosition - sp.Value.StartIndex;
                    run.Text = run.Text.Substring(0, i1) + text + run.Text.Substring(i1);

                    break;
                case Underline underline:
                    break;
                case Span span:
                    break;
                case Inline inline:
                    break;
                case ListItem listItem:
                    break;
                case TableCell tableCell:
                    break;
                case TableRow tableRow:
                    break;
                case TableRowGroup tableRowGroup:
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