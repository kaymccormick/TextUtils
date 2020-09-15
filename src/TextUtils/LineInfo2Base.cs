using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public abstract class LineInfo2Base : ILineInfo2Base
    {
        protected LineInfo2Base(in int lineNumber, in TextSpan textSpan, Rect boundingRect)
        {
            LineNumber = lineNumber;
            TextSpan = textSpan;
            BoundingRect = boundingRect;
        }

        public Rect BoundingRect { get; }
        public virtual IEnumerable<CharInfo?> Characters { get; protected set; }
        public TextSpan TextSpan { get; }
        public int LineNumber { get; }
        public int Offset => TextSpan.Start;
        public Point Origin => BoundingRect.TopLeft;
        public double Height => BoundingRect.Height;
        public int Length => TextSpan.Length;
        public double Width => BoundingRect.Width;
        public abstract void AddCharacter(CharInfo info);
    }

    public class TextSpan
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public int End => Start + Length;
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }
    }
}