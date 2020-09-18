﻿using System.Windows.Media.TextFormatting;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TextUtils
{
    
    /// <summary>
    /// 
    /// </summary>
    public class CustomTextEndOfLine : TextEndOfLine, ICustomSpan
    {
        private TextSpan _span;
        public int? Index { get; set; }

        public TextSpan Span
        {
            get { return _span; }
            set { _span = value; }
        }

        /// <inheritdoc />
        public bool Partial { get; set; }

        /// <inheritdoc />
        public bool FinalPartial { get; } = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="span"></param>
        public CustomTextEndOfLine(int length, TextSpan span) : base(length)
        {
            _span = span;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <param name="textRunProperties"></param>
        /// <param name="span"></param>
        public CustomTextEndOfLine(int length, TextRunProperties textRunProperties, TextSpan span) : base(length, textRunProperties)
        {
        }

        public CustomTextEndOfLine(int length) : base(length)
        {
        }
    }
}