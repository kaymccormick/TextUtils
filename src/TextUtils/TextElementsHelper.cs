using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.TextFormatting; // ReSharper disable once RedundantUsingDirective

namespace TextUtils
{
    public static class TextElementsHelper
    {
        public static int ParseBlocks(IBasicTextSourceClient2 client, BlockCollection blockCollection,
            char[] textBuffer, in int textBufferOffset,
            int inCount,
            TextParagraphProperties paraProps, TextRunProperties props)
        {
            var left = inCount;
            var newOffset = textBufferOffset;
            foreach (Block? block in blockCollection)
            {
                if (block == null) continue;
                var count = Block(client,
                    block, textBuffer, newOffset, left, paraProps, props);
                // Debug.WriteLine("count for block is " + count);
                newOffset += count;
                left -= count;
            }

            return newOffset - textBufferOffset;
        }

        private static int Block(IBasicTextSourceClient2 client, Block block, char[] textBuffer, int textBufferOffset,
            in int inCount, TextParagraphProperties paraProps, TextRunProperties inProps)
        {
            var offset = textBufferOffset;
            var left = inCount;
            var props = inProps;
            switch (block)
            {
                case BlockUIContainer _:
                    break;
                case List list:
                    return List(client, list, textBuffer, textBufferOffset, left, paraProps, props);
                case Paragraph paragraph:
                    return Paragraph(client, paragraph, textBuffer, textBufferOffset, left, paraProps, props);
                case Section section:
                    foreach (var sectionBlock in section.Blocks)
                    {
                        var c = Block(client, sectionBlock, textBuffer, offset, left, paraProps, inProps);
                        left -= c;
                        offset += c;
                    }
                    break;
                case Table _:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(block));
            }

            return inCount- left;
        }

        private static int List(IBasicTextSourceClient2 client,
            List list, char[] textBuffer, in int textBufferOffset, int left,
            TextParagraphProperties paraProps, TextRunProperties props)
        {
            var offset = textBufferOffset;
            // TextParagraphProperties pp = new MarkerParagraphProperties(props, true);
               TextParagraphProperties pp = new TextParagraphProperties1(props, true);
            // var pp = new ListParagraphProperties(new TextSimpleMarkerProperties(list.MarkerStyle, list.MarkerOffset,
                // list.StartIndex, paraProps), props, true));

            var listMarkerOffset = list.MarkerOffset;
            if (double.IsNaN(listMarkerOffset))
                listMarkerOffset = 0.0;
            foreach (var item in list.ListItems)
            {
                var p = new ListParagraphProperties(new TextSimpleMarkerProperties(list.MarkerStyle, listMarkerOffset,
                    list.StartIndex, pp), props, true);
                foreach (var itemBlock in item.Blocks)
                {

                    var c = Block(client, itemBlock, textBuffer, offset, left, p, props);
                    left -= c;
                    offset += c;
                }
            }

            return offset - textBufferOffset;
        }

        private static int Paragraph(IBasicTextSourceClient2 client,
            Paragraph paragraph, char[] textBuffer, in int textBufferOffset, in int inLeft,
            TextParagraphProperties textParagraphProperties, TextRunProperties props)
        {
            var length = 1;
            var offset = textBufferOffset;
            var left = inLeft;
            client.AddSpan(new TextSpan<CharacteristicsImpl>(length,
                new CharacteristicsImpl(offset, length, (TextElement)paragraph) {StartParagraph = true, ParagraphProperties = textParagraphProperties}));
            offset += length;
            left -= length;
            foreach (var inline in paragraph.Inlines)
            {
                var c = Inline(client, inline, textBuffer, offset, left, props);
                // Debug.WriteLine("Count is " + c);
                offset  += c;
                left -= c;
            }

            if (left < 2)
            {
                throw new InvalidOperationException();
            }
            textBuffer[offset] = '\r';
            textBuffer[offset + 1] = '\n';
            client.AddSpan(new TextSpan<CharacteristicsImpl>(2, new CharacteristicsImpl(offset, 2, paragraph ) {EndOfParagraph = true}));
            offset += 2;
            return offset - textBufferOffset;
        }

        private static int Inline(IBasicTextSourceClient2 client, Inline inline, char[] textBuffer,
            int textBufferOffset, int left, TextRunProperties inProps)
        {
            var properties = inProps;
            var paragraphInline = inline;
            if (paragraphInline is Span span)
            {
                return Span(client, span, textBuffer, textBufferOffset, left, properties);
            }

            switch (paragraphInline)
            {
                case Figure figure:
                    
                    break;
                case Floater floater:
                    break;
                case AnchoredBlock anchoredBlock:
                    break;
                case Hyperlink hyperlink:
                    break;
                case InlineUIContainer inlineUiContainer:
                    return InlineUiContainer(client, inlineUiContainer, textBuffer, textBufferOffset, left, inProps);
                case LineBreak lineBreak:
                    return LineBreak1(client, lineBreak, textBuffer, textBufferOffset,left);
                case Run run:
                    return Run(client, textBuffer, textBufferOffset, run, properties, left);

                default:
                    throw new ArgumentOutOfRangeException(nameof(paragraphInline));
            }

            return 0;
        }

        private static int InlineUiContainer(IBasicTextSourceClient2 client, InlineUIContainer inlineUiContainer,
            char[] textBuffer, int textBufferOffset, int left, TextRunProperties inProps)
        {

            const int c = 1;
            if (left < c)
            {
                throw new InvalidOperationException();
            }
            var uiElement = inlineUiContainer.Child;
            TextEmbeddedObject? rr = new TextEmbeddedObject1(
                new CharacterBufferReference(textBuffer, textBufferOffset), c, inProps, true, uiElement);
            // basicTextElement.UiElements.Add(uiElement);
            client.AddSpan(new TextSpan<CharacteristicsImpl>(c,
                new CharacteristicsImpl(textBufferOffset, c, inlineUiContainer) {EmbeddedObject = rr}));
            return 1;
        }

        private static int Span(IBasicTextSourceClient2 client, Span span, char[] textBuffer, in int textBufferOffset,
            int left, TextRunProperties properties)
        {
            var r = textBufferOffset;
            foreach (var spanInline in span.Inlines)
            {
                var c = Inline(client, spanInline, textBuffer, r, left, properties);
                left -= c;
                r += c;
            }

            return r - textBufferOffset;
        }

        private static int LineBreak1(IBasicTextSourceClient2 client, LineBreak lineBreak,
            char[] textBuffer, in int textBufferOffset, in int left)
        {
            if(left < 2)
            {
                throw new InvalidOperationException();
            }
            textBuffer[textBufferOffset] = '\r';
            textBuffer[textBufferOffset + 1] = '\n';
            return 2;
        }

        private static int Run(IBasicTextSourceClient2 client, char[] textBuffer,
            int textBufferOffset, Run textElement, TextRunProperties properties, int left)
        {
            var count1 = textElement.ContentStart.GetTextInRun(LogicalDirection.Forward, textBuffer, textBufferOffset,
                left);
            var textRunProperties = TextRunProperties1(textElement);
            var i = new TextSpan<CharacteristicsImpl>(count1,
                new CharacteristicsImpl(textBufferOffset, count1, textElement) {TextRunProperties = textRunProperties});
            client.AddSpan(i);
            return count1;
        }

        private static TextRunProperties TextRunProperties1(TextElement textElement)
        {
            BaselineAlignment baselineAlignment=default;
            CultureInfo Culture  = CultureInfo.CurrentUICulture;
            TextEffectCollection textEffects=new TextEffectCollection();
            Typeface typeface = new Typeface(textElement.FontFamily, textElement.FontStyle, textElement.FontWeight,
                textElement.FontStretch);

            TextDecorationCollection textDecorations = new TextDecorationCollection();
            return new TextRunProperties1(textElement.Background, baselineAlignment,Culture,textElement.FontSize, textElement.FontSize, textElement.Foreground, textDecorations, textEffects, typeface);
        }

    }

    internal class MarkerParagraphProperties : TextParagraphProperties
    {
        public MarkerParagraphProperties(TextRunProperties defaultTextRunProperties, bool firstLineInParagraph)
        {
            DefaultTextRunProperties = defaultTextRunProperties;
            FirstLineInParagraph = firstLineInParagraph;
        }

        public override bool AlwaysCollapsible { get; }
        public override double DefaultIncrementalTab { get; }
        public override TextRunProperties DefaultTextRunProperties { get; }
        public override bool FirstLineInParagraph { get; }
        public override FlowDirection FlowDirection { get; }
        public override double Indent { get; }
        public override double LineHeight { get; }
        public override double ParagraphIndent { get; }
        public override IList<TextTabProperties> Tabs { get; }
        public override TextAlignment TextAlignment { get; }
        public override TextDecorationCollection TextDecorations { get; } = new TextDecorationCollection();
        public override TextMarkerProperties TextMarkerProperties { get; }
        public override TextWrapping TextWrapping { get; }
    }
}