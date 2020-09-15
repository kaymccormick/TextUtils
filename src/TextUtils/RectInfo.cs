using System;
using System.Windows;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class RectInfo
    {
        private Rect _rect;
        private double _value;

        // ReSharper disable once UnusedMember.Global
        public RectInfo(Rect rect)
        {
            Rect = rect;
        }

        public RectInfo(Rect rect, in int offset, TextRun? triTextRun)
        {
            Rect = rect;
            Offset = offset;
            TextRun = triTextRun;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public TextRun? TextRun { get;  }

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public int Offset { get; }

        public double Value
        {
            get { return _value; }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        
        public Rect Rect
        {
            get { return _rect; }
            set
            {
                _rect = value;
                _value = _rect.Bottom * 1000 + _rect.Left;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Math.Round(Value, 3)} {Math.Round(Rect.Y, 2)} {this.Rect} {this.TextRun}";

        }
    }
}