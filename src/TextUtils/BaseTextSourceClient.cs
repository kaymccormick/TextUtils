using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace TextUtils
{
    public class BaseTextSourceClient : IBaseTextSourceClient
    {
        private FontFamily? _fontFamily;

        public BaseTextSourceClient(ICustomTextSource? textSource)
        {
            TextSource = textSource;
        }

        /// <inheritdoc />
        public ICustomTextSource? TextSource { get; }

        /// <inheritdoc />
        public double FontSize { get; set; }

        /// <inheritdoc />
        public FontWeight FontWeight { get; set; }

        /// <inheritdoc />
        public double PixelsPerDip { get; set; }

        /// <inheritdoc />
        public double OutputWidth { get; set; }

        /// <inheritdoc />
        public double XOffset { get; set; }

        /// <inheritdoc />
        public string? FontFamilyName { get; private set; }

        /// <inheritdoc />
        public FontFamily? FontFamily   
        {
            get { return _fontFamily; }
            set
            {
                _fontFamily = value;
                FontFamilyName = value?.FamilyNames[XmlLanguage.GetLanguage("en-US")] ?? null;
            }
        }
    }
}