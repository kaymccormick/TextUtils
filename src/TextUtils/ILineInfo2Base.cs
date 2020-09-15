using System.Collections.Generic;
using System.Windows;

namespace TextUtils
{
    public interface ILineInfo2Base
    {
        Rect BoundingRect { get; }
        IEnumerable<CharInfo?> Characters { get; }
        TextSpan TextSpan { get; }
        int LineNumber { get; }
        int Offset { get; }
        Point Origin { get; }
        double Height { get; }
        int Length { get; }
        double Width { get; }
        void AddCharacter(CharInfo info);
    }
}