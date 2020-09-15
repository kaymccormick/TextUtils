using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class TextParagraphProperties1 : TextParagraphProperties
    {
        /// <inheritdoc />
        public TextParagraphProperties1()
        {
        }

        /// <inheritdoc />
        public override bool AlwaysCollapsible { get; } = false;


        /// <inheritdoc />
        public override double ParagraphIndent { get; } = 40.0;


        public TextParagraphProperties1(TextRunProperties defaultTextRunProperties, bool firstLineInParagraph)
        {
            DefaultTextRunProperties = defaultTextRunProperties;
            FirstLineInParagraph = firstLineInParagraph;
        }

        /// <inheritdoc />
        public override TextRunProperties DefaultTextRunProperties { get; }

        /// <inheritdoc />
        public override bool FirstLineInParagraph { get; }

        /// <inheritdoc />
        public override FlowDirection FlowDirection { get; } = FlowDirection.LeftToRight;

        /// <inheritdoc />
        public override double Indent { get; } = 0.0;

        /// <inheritdoc />
        public override double LineHeight { get; }

        /// <inheritdoc />'/
        public override TextAlignment TextAlignment { get; } = TextAlignment.Left;

        /// <inheritdoc />
        public override TextMarkerProperties TextMarkerProperties { get; }

        /// <inheritdoc />
        public override TextWrapping TextWrapping { get; }= TextWrapping.Wrap;
    
    }
}