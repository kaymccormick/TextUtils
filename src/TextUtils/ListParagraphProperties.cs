using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class ListParagraphProperties : TextParagraphProperties
    {
        public ListParagraphProperties()
        {
        }

        public override double ParagraphIndent { get; }

        public ListParagraphProperties(TextMarkerProperties textMarkerProperties, TextRunProperties textRunProperties, bool firstLineInParagraph)
        {
            TextMarkerProperties = textMarkerProperties;
            DefaultTextRunProperties = textRunProperties;
            FirstLineInParagraph = firstLineInParagraph;
            TextDecorations = new TextDecorationCollection();
        }

        public override TextDecorationCollection TextDecorations { get; }

        /// <inheritdoc />
        public override TextRunProperties DefaultTextRunProperties { get; } = null!;

        /// <inheritdoc />
        public override bool FirstLineInParagraph { get; }

        /// <inheritdoc />
        public override FlowDirection FlowDirection { get; }

        /// <inheritdoc />
        public override double Indent { get; }

        /// <inheritdoc />
        public override double LineHeight { get; }

        /// <inheritdoc />
        public override TextAlignment TextAlignment { get; }

        /// <inheritdoc />
        public override TextMarkerProperties TextMarkerProperties { get; } = null!;

        /// <inheritdoc />
        public override TextWrapping TextWrapping { get; }
    }
}