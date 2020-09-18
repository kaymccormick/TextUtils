using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.TextFormatting;
using JetBrains.Annotations;
using TextLine = System.Windows.Media.TextFormatting.TextLine;

namespace TextUtils
{
    public class InputTuple : IInputTuple
    {
        public void Deconstruct([CanBeNull] out TextLine? textLine, out Point position, [NotNull] out TextSource textSource, out int offset, out int? length, [CanBeNull] out LinkedListNode<CharInfo>? charInfo, out object? textChange, out int lineNo)
        {
            textLine = TextLine;
            position = Position;
            textSource = TextSource;
            offset = Offset;
            length = Length;
            charInfo = CharInfo;
            textChange = TextChange;
            lineNo = LineNo;
        }

        public InputTuple(TextLine? textLine, Point position, TextSource source, int offset, int? length,
            LinkedListNode<CharInfo>? charInfo, object? change, int lineNo)
        {
            TextLine = textLine;
            Position =  position;
            TextSource = source;
            Offset = offset;
            Length = length;
            CharInfo = charInfo;
            TextChange = change;
            LineNo = lineNo;
        }

        /// <inheritdoc />
        public TextLine? TextLine { get; set; }

        /// <inheritdoc />
        public Point Position { get; }

        /// <inheritdoc />
        public TextSource TextSource { get; }

        /// <inheritdoc />
        public int Offset { get; }

        /// <inheritdoc />
        public int? Length { get; }

        /// <inheritdoc />
        public LinkedListNode<CharInfo>? CharInfo { get; }

        /// <inheritdoc />
        public object? TextChange { get; }

        public int LineNo { get; }
    }
}