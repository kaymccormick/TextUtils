using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;
using JetBrains.Annotations;
using TextLine = System.Windows.Media.TextFormatting.TextLine;

namespace TextUtils
{
    public class InputTuple : IInputTuple
    {
        public void Deconstruct([CanBeNull] out TextLine? item1, out Point item2, [NotNull] out TextSource item3, out int offset, out int? item5, [CanBeNull] [ItemNotNull] out LinkedListNode<CharInfo>? item6, out TextChange? item7, out int lineNo)
        {
            item1 = Item1;
            item2 = Item2;
            item3 = Item3;
            offset = Offset;
            item5 = Item5;
            item6 = Item6;
            item7 = Item7;
            lineNo = LineNo;
        }

        /// <inheritdoc />
        public InputTuple(TextLine? textLine, Point position, TextSource source, int offset, int? length,
            LinkedListNode<CharInfo>? charInfo, TextChange? change, int lineNo)
        {
            Item1 = textLine;
            Item2 =  position;
            Item3 = source;
            Offset = offset;
            Item5 = length;
            Item6 = charInfo;
            Item7 = change;
            LineNo = lineNo;
            //Debug.WriteLine("offset is " + offset);
        }

        /// <inheritdoc />
        public TextLine? Item1 { get; set; }

        /// <inheritdoc />
        public Point Item2 { get; }

        /// <inheritdoc />
        public TextSource Item3 { get; }

        /// <inheritdoc />
        public int Offset { get; }

        /// <inheritdoc />
        public int? Item5 { get; }

        /// <inheritdoc />
        public LinkedListNode<CharInfo>? Item6 { get; }

        /// <inheritdoc />
        public TextChange? Item7 { get; }

        public int LineNo { get; }
    }
}