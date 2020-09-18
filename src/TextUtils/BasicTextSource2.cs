using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using JetBrains.Annotations;
using Castle.DynamicProxy;

namespace TextUtils
{
    public class BasicTextSource2 : BasicTextSource
    {
        /// <inheritdoc />
        public BasicTextSource2([NotNull] FontFamily fontFamily, double fontSize, [CanBeNull] Brush? foregroundColor = null) : base(fontFamily, fontSize, foregroundColor)
        {
        }

        /// <inheritdoc />
        public override TextRun GetTextRun(int textSourceCharacterIndex)
        {
            Debug.WriteLine($"{nameof(GetTextRun)}: {textSourceCharacterIndex}");

            var span = Spans.FirstOrDefault(z => z.Value.StartIndex + z.Value.Length > textSourceCharacterIndex);
            if (span == null) return OnEnd(textSourceCharacterIndex);
            // var textRunProperties = new Modifier(span.Length,p);
            if (OnGenericCharacters(textSourceCharacterIndex, out var customTextCharacters1, span.Value.StartIndex))
                return customTextCharacters1;

            // var typeface = CurrentRendering.Typeface;
            var v = span.Value;
            // var typeface1 = new Typeface(typeface.FontFamily,v.FontStyle,typeface.Weight, typeface.Stretch);
            // var fontRendering = FontRendering.CreateInstance(CurrentRendering.FontSize, CurrentRendering.TextAlignment, CurrentRendering.TextDecorations, CurrentRendering.TextColor, typeface1);
            var textRunProperties = v.TextRunProperties ?? BasicProps();
            var paragraphProperties = v.ParagraphProperties;

            if (v.EndOfParagraph)
            {
                return EndOfParagraph(v);
            }
            if(v.StartParagraph)
            {
                return new TextModifier1(v.Length, textRunProperties);
            }   
            if (v.EmbeddedObject != null)
            {
                return v.EmbeddedObject;
            }

            var count = span.Length - (textSourceCharacterIndex - v.StartIndex);
            if (count == 0)
            {

            }
            return new TextCharacters(TextBuffer!,textSourceCharacterIndex, 
                count,textRunProperties);

        }

        /// <inheritdoc />
        public override TextSpan<CultureSpecificCharacterBufferRange> GetPrecedingText(int textSourceCharacterIndexLimit)
        {
            Debug.WriteLine($"{nameof(GetPrecedingText)}: {textSourceCharacterIndexLimit}");
            var span = Spans.FirstOrDefault(z => z.Value.StartIndex + z.Value.Length == textSourceCharacterIndexLimit);
            if (span != null)
            {
                return new TextSpan<CultureSpecificCharacterBufferRange>(span.Length, new CultureSpecificCharacterBufferRange(CultureInfo.CurrentUICulture, new CharacterBufferRange(TextBuffer, span.Value.StartIndex, span.Length)));
            }

            span = Spans.LastOrDefault(z => z.Value.StartIndex < textSourceCharacterIndexLimit);
            var offset = (span.Value.StartIndex + span.Value.Length);
            return new TextSpan<CultureSpecificCharacterBufferRange>(offset - textSourceCharacterIndexLimit , new CultureSpecificCharacterBufferRange(CultureInfo.CurrentUICulture, new CharacterBufferRange(TextBuffer, offset, offset - textSourceCharacterIndexLimit)));
        }

        public override TextParagraphProperties? GetTextParagraphProperties(in int textStorePosition)
        {
            var i = textStorePosition;
            var span = Spans.FirstOrDefault(z => z.Value.StartIndex + z.Value.Length > i);
            if (span != null)
            {
                var p = span.Value.ParagraphProperties;

                if (ProxyHelper.IsProxyEnabled && p != null)
                    return (TextParagraphProperties)ProxyHelper.ProxyGenerator.CreateClassProxyWithTarget(
                        p.GetType(), p,
                        new TextInterceptor());
            }
            return base.GetTextParagraphProperties(in textStorePosition);
        }

        private TextRun EndOfParagraph(CharacteristicsImpl characteristicsImpl)
        {
            return new CustomTextEndOfParagraph(2);
        }

        public List<TextSpan<CharacteristicsImpl>> Spans { get; } = new List<TextSpan<CharacteristicsImpl>>();

        public void AddSpan(TextSpan<CharacteristicsImpl> textSpan)
        {
            Spans.Add(textSpan);
        }
    }
}