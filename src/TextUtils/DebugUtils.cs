using System;
using System.Windows;

namespace TextUtils
{
    public static class DebugUtils
    {
        public static Rect RoundRect(Rect rect)
        {
            var rectS = rect;
            if (rect.IsEmpty) return rect;
            rectS.X = Math.Round(rectS.X);
            rectS.Y = Math.Round(rectS.Y);
            rectS.Width = Math.Round(rectS.Width);
            rectS.Height = Math.Round(rectS.Height);
            return rectS;
        }
    }
}