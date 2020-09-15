using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.TextFormatting;
using TextLine = System.Windows.Media.TextFormatting.TextLine;

namespace TextUtils
{
    public interface 
        IInputTuple
    {
        TextLine? Item1 { get; }
        Point Item2 { get; }
        TextSource Item3 { get; }
        int Offset { get; }
        int? Item5 { get; }
        LinkedListNode<CharInfo>? Item6 { get; }
        TextChange? Item7 { get; }
    }
}