using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class BasicTextSource : TextSource, ICustomTextSource
    {
        private string? _text;

        public BasicTextSource(FontFamily fontFamily, double fontSize,  Brush? foregroundColor=null)
        {
            FontFamily = fontFamily;
            FontSize = fontSize;
            ForegroundColor = foregroundColor ?? Brushes.Black;
            var fontRendering = FontRendering.CreateInstance(fontSize, default, null, ForegroundColor,
                new Typeface(fontFamily, default, default, default));
            CurrentRendering = fontRendering;
            BaseProps =
                new GenericTextRunProperties(
                    fontRendering, PixelsPerDip);
        }

        /// <inheritdoc />
        public void Init()
        {
        }

        /// <inheritdoc />
        public int Length { get; protected set; }

        /// <inheritdoc />
        public GenericTextRunProperties? BaseProps { get; set; }

        /// <inheritdoc />
        public TextRunProperties BasicProps()
        {
            return new VeryBasicTextRunProperties(BaseProps);

        }

        /// <inheritdoc />
        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            var end = Length;
            if (OnGenericCharacters(textSourceCharacterIndex, out var customTextCharacters1, end))
                return customTextCharacters1;
            return OnEnd(textSourceCharacterIndex);
            

        }
        protected TextRun OnEnd(int textSourceCharacterIndex)
        {
            if (textSourceCharacterIndex < Length)
            {
                var len = Length - textSourceCharacterIndex;
                var buf = new char[len];
                Text.CopyTo(textSourceCharacterIndex, buf, 0, len);
                if (len == 2 && buf[0] == '\r' && buf[1] == '\n')
                {
                    var eol = new CustomTextEndOfLine(2) { Index = textSourceCharacterIndex };

                    return eol;
                }

                var t = string.Join("", buf);
                var customTextCharacters = new CustomTextCharacters(t, MakeProperties(new object(), t))
                    { Index = textSourceCharacterIndex };

                return customTextCharacters;
            }

            var endOfParagraph = new CustomTextEndOfParagraph(2) { Index = textSourceCharacterIndex };

            return endOfParagraph;
        }


        protected virtual bool OnGenericCharacters(int textSourceCharacterIndex, out TextRun? customTextCharacters1,
            int span1Start)
        {
            if (textSourceCharacterIndex < span1Start)
            {
                var len = span1Start - textSourceCharacterIndex;
                char[] buf;
                int start = 0;
                if (Text != null)
                {
                    buf = new char[len];
                    Text.CopyTo(textSourceCharacterIndex, buf, start, len);
                }
                else
                {
                    buf = TextBuffer;
                    start = textSourceCharacterIndex;
                }
                

                if (len == 2 && buf[start] == '\r' && buf[start + 1] == '\n')
                {
                    customTextCharacters1 = EndOfLine(textSourceCharacterIndex);
                    return true;
                }

                
                var t = string.Join("", buf);
                var nl = t.IndexOf("\r\n", start,StringComparison.Ordinal);
                if (nl != -1)
                {
                    t = t.Substring(start, nl - start);
                    if (t == "")
                    {
                        customTextCharacters1 = EndOfLine(textSourceCharacterIndex);
                        return true;
                    }

                    var ctc = TextCharacters(textSourceCharacterIndex, t);
                    customTextCharacters1 = ctc;
                    return true;
                }

                customTextCharacters1 = TextCharacters(textSourceCharacterIndex, t); ;
                return true;
            }

            customTextCharacters1 = null;
            return false;
        }

        protected virtual TextCharacters TextCharacters(int textSourceCharacterIndex, string t)
        {
            return new CustomTextCharacters(t, BasicProps())
                { Index = textSourceCharacterIndex };
        }

        protected virtual TextEndOfLine EndOfLine(int textSourceCharacterIndex)
        {
            return new CustomTextEndOfLine(EolLength) { Index = textSourceCharacterIndex };
        }


        /// <inheritdoc />
        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetTextEffectCharacterIndexFromTextSourceCharacterIndex(int textSourceCharacterIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public double PixelsPerDip { get; set; } = 1.0;

        /// <inheritdoc />
        public int EolLength { get; } = 2;

        /// <inheritdoc />
        public object BtRuns { get; }

        /// <inheritdoc />
        public FontRendering? CurrentRendering { get; }

        /// <inheritdoc />
        public ThreadLocal<FontRendering?> ThreadRendering { get; }

        /// <inheritdoc />
        public bool IsLoaded => true;
        

        /// <inheritdoc />
        public string? FamilyName { get; set; }

        /// <inheritdoc />
        public double FontSize { get; set; }

        public Brush? ForegroundColor { get; }

        /// <inheritdoc />
        public double LineHeight { get; set; }

        /// <inheritdoc />
        public FontFamily? FontFamily { get; set; }

        public string? Text
        {
            get { return _text; }
            set
            {
                _text = value;
                Length = _text?.Length ?? 0;
            }
        }

        public char[] TextBuffer { get; set; }
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int StartIndex { get; set; }

        /// <inheritdoc />
        public TextRunProperties MakeProperties(object arg, string text)
        {
            return BasicProps();
        }

        /// <inheritdoc />
        public Task<object> TextInputAsync(int? insertionPoint, InputRequest inputRequest)
        {
            return null;
        }

        /// <inheritdoc />
        public Task<object?> TextChangeAsync(TextChange change)
        {
            return null;
        }

        /// <inheritdoc />
        public TextSource AsTextSource()
        {
            return this;
        }

        /// <inheritdoc />
        public void Load()
        {
        }

        /// <inheritdoc />
        public bool TryLoad()
        {
            return true;
        }

        public void SetTextBuffer(char[] textBuffer, in int startIndex, in int length)
        {
            TextBuffer = textBuffer;
            StartIndex = startIndex;
            Length = length;
        }
    }

    public class TextChange
    {
    }
}