// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class CharInfo
    {
        public int LineIndex { get; }
        public int RunIndex { get; set; }
        public char Character { get; set; }
        public double AdvanceWidth { get; set; }
        [JsonIgnore]
        public bool? CaretStop { get; }
        public double XOrigin { get; set; }
        public double YOrigin { get; set; }
        public Geometry Geometry { get; }
        public Rect AlignmentBox { get; }
        public Point BaselineOrigin { get; }
        public ushort GlyphIndex { get; }
        public GlyphTypeface GlyphTypeface { get; }
        public int Index { get; set; }
        public int LineNumber { get; set; }

        public CharInfo(in int lineNo, in int index, in int lineIndex, in int runIndex, char character,
            double advanceWidth,
            bool? caretStop,
            double xOrigin, double yOrigin, double height)
        {
        }

        public CharInfo(in int lineNo, in int storePosition, in int lineCharIndex, in int runIndex, in char glCharacter,
            in double glAdvanceWidth, bool? glCaretStop, in double xOrigin, in double linePositionY,
            double glFontRenderingEmSize, Rect alignmentBox, Point baselineOrigin, ushort glyphIndex,
            GlyphTypeface glyphTypeface, double renderingEmSize, int lineRun)
        {
            BoundingRect = new Rect(xOrigin, linePositionY, glAdvanceWidth, glFontRenderingEmSize);
            LineNumber = lineNo;
            Index = storePosition;
            LineIndex = lineCharIndex;
            RunIndex = runIndex;
            Character = glCharacter;
            AdvanceWidth = glAdvanceWidth;
            CaretStop = glCaretStop;
            XOrigin = xOrigin;
            YOrigin = linePositionY;
        
            AlignmentBox = alignmentBox;
            BaselineOrigin = baselineOrigin;
            GlyphIndex = glyphIndex;
            GlyphTypeface = glyphTypeface;
            RenderingEmSize = renderingEmSize;
            LineRun = lineRun;
        }

        public Rect BoundingRect { get; set; }
        public double RenderingEmSize { get; }
        public int LineRun { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return
                $"{nameof(Index)}: {Index}, {nameof(LineNumber)}: {LineNumber}, {nameof(LineIndex)}: {LineIndex}, {nameof(RunIndex)}: {RunIndex}, {nameof(Character)}: {Character}, {nameof(AdvanceWidth)}: {AdvanceWidth:N1}, {nameof(CaretStop)}: {CaretStop}, {nameof(XOrigin)}: {XOrigin}, {nameof(YOrigin)}: {YOrigin}";
        }
    }
}