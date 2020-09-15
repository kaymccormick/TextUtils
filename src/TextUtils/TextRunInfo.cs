using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class TextRunInfo
    {
        public TextRun? TextRun { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[{Value:D5} {nameof(TextRun)}: {TextRun}, {nameof(Rect)}: {Rect}";
        }

        public Rect Rect { get; set; }
        
        public int Value { get; }

        public TextRunInfo(TextRun? textRun, int value)
        {
            TextRun = textRun;
            Value = value;
        }

        public TextRunInfo(int value)
        {
            Value = value;
        }
    }
}