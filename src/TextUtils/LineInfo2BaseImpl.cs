using System.Collections.Generic;
using System.Windows;
using TextLine = System.Windows.Media.TextFormatting.TextLine;

namespace TextUtils
{
    public class LineInfo2BaseImpl : LineInfo2Base
    {
        public TextLine TextLine { get; }
        private List<CharInfo?> _characters = new List<CharInfo?>();

        public LineInfo2BaseImpl(in int lineNumber, in TextSpan textSpan, Rect boundingRect, TextLine textLine) : base(in lineNumber, in textSpan, boundingRect)

        {
            TextLine = textLine;
            Characters = _characters;
        }

        public override void AddCharacter(CharInfo info)
        {
            _characters.Add(info);
        }
    }
}