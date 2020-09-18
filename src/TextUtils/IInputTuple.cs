using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.TextFormatting;
using TextLine = System.Windows.Media.TextFormatting.TextLine;

namespace TextUtils
{
    public interface IInputTuple
    {
        TextLine? TextLine { get; }
        Point Position { get; }
        TextSource TextSource { get; }
        int Offset { get; }
        int? Length { get; }
        LinkedListNode<CharInfo>? CharInfo { get; }
        object? TextChange { get; }
    }
}