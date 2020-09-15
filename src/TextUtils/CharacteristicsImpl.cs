using System.Windows.Documents;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class CharacteristicsImpl : Characteristics
    {
        public TextParagraphProperties? ParagraphProperties { get; set; }
        public int StartIndex { get; }
        public int Length { get; }
        public TextElement TextElement { get; }
        public TextRunProperties? TextRunProperties { get; set; }
        public bool EndOfParagraph { get; set; }
        public TextEmbeddedObject? EmbeddedObject { get; set; }
        public bool StartParagraph { get; set; }

        /// <inheritdoc />
        public CharacteristicsImpl(int startIndex, int length, TextElement textElement)
        {
            StartIndex = startIndex;
            Length = length;
            TextElement = textElement;
        }

    }
}