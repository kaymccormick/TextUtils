using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class TextRunProperties1 : TextRunProperties
    {
        private readonly Brush _backgroundBrush;
        private readonly BaselineAlignment _baselineAlignment;
        private readonly NumberSubstitution _numberSubstitution;
        private readonly TextRunTypographyProperties _typographyProperties;
        private readonly CultureInfo _cultureInfo;
        private readonly double _fontHintingEmSize;
        private readonly double _fontRenderingEmSize;
        private readonly Brush _foregroundBrush;
        private readonly TextDecorationCollection _textDecorations;
        private readonly TextEffectCollection _textEffects;
        private readonly Typeface _typeface;

        /// <inheritdoc />
        public TextRunProperties1(Brush backgroundBrush, BaselineAlignment baselineAlignment, CultureInfo cultureInfo, double fontHintingEmSize, double fontRenderingEmSize, Brush foregroundBrush, TextDecorationCollection textDecorations, TextEffectCollection textEffects, Typeface typeface)
        {
            _backgroundBrush = backgroundBrush;
            _baselineAlignment = baselineAlignment;
            _cultureInfo = cultureInfo;
            _fontHintingEmSize = fontHintingEmSize;
            _fontRenderingEmSize = fontRenderingEmSize;
            _foregroundBrush = foregroundBrush;
            _textDecorations = textDecorations;
            _textEffects = textEffects;
            _typeface = typeface;
        }

        /// <inheritdoc />
        public override Brush BackgroundBrush
        {
            get
            {
                //Debug.WriteLine($"Request for {nameof(BackgroundBrush)}");
                return _backgroundBrush;
            }
        }

        /// <inheritdoc />
        public override BaselineAlignment BaselineAlignment
        {
            get
            {
                //Debug.WriteLine($"Request for {nameof(BaselineAlignment)}");

                return _baselineAlignment;
            }
        }

        /// <inheritdoc />
        public override NumberSubstitution NumberSubstitution
        {
            get { //Debug.WriteLine($"Request for {nameof(NumberSubstitution)}");
                  return _numberSubstitution;
                  }
        }

        /// <inheritdoc />
        public override TextRunTypographyProperties TypographyProperties
        {
            get { //Debug.WriteLine($"Request for {nameof(TypographyProperties)}");
                  return _typographyProperties; }
        }

        /// <inheritdoc />
        public override CultureInfo CultureInfo
        {
            get { //Debug.WriteLine($"Request for {nameof(CultureInfo)}");
                  return _cultureInfo; }
        }

        /// <inheritdoc />
        public override double FontHintingEmSize
        {
            get { //Debug.WriteLine($"Request for {nameof(FontHintingEmSize)}");
                  return _fontHintingEmSize; }
        }

        /// <inheritdoc />
        public override double FontRenderingEmSize
        {
            get { //Debug.WriteLine($"Request for {nameof(FontRenderingEmSize)}");
                  return _fontRenderingEmSize; }
        }

        /// <inheritdoc />
        public override Brush ForegroundBrush
        {
            get { //Debug.WriteLine($"Request for {nameof(ForegroundBrush)}");
                  return _foregroundBrush; }
        }

        /// <inheritdoc />
        public override TextDecorationCollection TextDecorations
        {
            get { //Debug.WriteLine($"Request for {nameof(TextDecorations)}");
                  return _textDecorations; }
        }

        /// <inheritdoc />
        public override TextEffectCollection TextEffects
        {
            get { //Debug.WriteLine($"Request for {nameof(TextEffects)}");
                  return _textEffects; }
        }

        /// <inheritdoc />
        public override Typeface Typeface
        {
            get { //Debug.WriteLine($"Request for {nameof(Typeface)}");
                  return _typeface; }
        }
    }
}