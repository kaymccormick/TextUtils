using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    internal class ListParagraphProperties : TextParagraphProperties
    {
        public ListParagraphProperties(TextMarkerProperties textMarkerProperties)
        {
            TextMarkerProperties = textMarkerProperties;
        }

        /// <inheritdoc />
        public override TextRunProperties DefaultTextRunProperties { get; }

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
        public override TextMarkerProperties TextMarkerProperties { get; }

        /// <inheritdoc />
        public override TextWrapping TextWrapping { get; }
    }
}