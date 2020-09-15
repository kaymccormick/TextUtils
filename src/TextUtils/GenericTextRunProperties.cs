using System;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    /// <summary>
    /// Class used to implement TextRunProperties
    /// </summary>
    public class GenericTextRunProperties : TextRunProperties
    {
        #region Constructors

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeface"></param>
        /// <param name="pixelsPerDip"></param>
        /// <param name="size"></param>
        /// <param name="hintingSize"></param>
        /// <param name="textDecorations"></param>
        /// <param name="foregroundBrush"></param>
        /// <param name="backgroundBrush"></param>
        /// <param name="baselineAlignment"></param>
        /// <param name="culture"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public GenericTextRunProperties(
            Typeface? typeface,
            double pixelsPerDip,
            double? size,
            double? hintingSize,
            TextDecorationCollection? textDecorations,
            Brush? foregroundBrush,
            Brush? backgroundBrush,
            BaselineAlignment baselineAlignment,
            CultureInfo culture)
        {
            if (typeface == null)
                throw new ArgumentNullException(nameof(typeface));

            ValidateCulture(culture);

            PixelsPerDip = pixelsPerDip;
            _typeface = typeface;
            _emSize = size ?? 12.0;
            _emHintingSize = hintingSize ??_emSize;
            _textDecorations = textDecorations;
            _foregroundBrush = foregroundBrush;
            _backgroundBrush = backgroundBrush;
            _baselineAlignment = baselineAlignment;
            _culture = culture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRender"></param>
        /// <param name="pixelsPerDip"></param>
        /// <param name="textDecorations"></param>
        /// <param name="foregroundBrush"></param>
        /// <param name="style"></param>
        /// <param name="weight"></param>
        /// <param name="backgroundBrush"></param>
        public GenericTextRunProperties(FontRendering? newRender,
            double pixelsPerDip, TextDecorationCollection? textDecorations = null, Brush? foregroundBrush = null,
            FontStyle? style = null, FontWeight? weight = null,
            Brush backgroundBrush = null)
        {
            Typeface? typeface=newRender?.Typeface;
            if (style.HasValue || weight.HasValue)
            {
                typeface = new Typeface(new FontFamily(newRender?.Typeface.FontFamily.FamilyNames[XmlLanguage.GetLanguage("en-US")]),
                    style ?? newRender?.Typeface.Style ?? default,
                    weight ?? newRender?.Typeface.Weight ?? default, newRender?.Typeface.Stretch ?? default);
            }

            _typeface = typeface;
            _emSize = newRender?.FontSize ??  12.0;
            _emHintingSize = _emSize;
            _textDecorations = textDecorations ?? newRender?.TextDecorations?? new TextDecorationCollection();
            _foregroundBrush = foregroundBrush ?? Brushes.Black;
            _backgroundBrush = backgroundBrush;
            // if (backgroundBrush != null && backgroundBrush.Opacity > 0.5)
            // {
                // _backgroundBrush = new SolidColorBrush(backgroundBrush.Color) {Opacity = 0.40};
            // }
            _baselineAlignment = BaselineAlignment.Baseline;
            _culture = CultureInfo.CurrentUICulture;
            PixelsPerDip = pixelsPerDip;
        }

        #endregion

        #region Private Methods

        private static void ValidateCulture(CultureInfo culture)
        {
            if (culture == null)
                throw new ArgumentNullException(nameof(culture));
            if (culture.IsNeutralCulture || culture.Equals(CultureInfo.InvariantCulture))
                throw new ArgumentException("Specific Culture Required", nameof(culture));
        }

        private static void ValidateFontSize(double emSize)
        {
            if (emSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(emSize), "Parameter Must Be Greater Than Zero.");
            //if (emSize > MaxFontEmSize)
            //   throw new ArgumentOutOfRangeException("emSize", "Parameter Is Too Large.");
            if (double.IsNaN(emSize))
                throw new ArgumentOutOfRangeException(nameof(emSize), "Parameter Cannot Be NaN.");
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public override Typeface Typeface
        {
            get { return _typeface!; }
        }

        /// <inheritdoc />
        public override double FontRenderingEmSize
        {
            get { return _emSize; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override double FontHintingEmSize
        {
            get { return _emHintingSize; }
        }

        /// <inheritdoc />
        public override TextDecorationCollection TextDecorations
        {
            get { return 
                    _textDecorations; }
        }

        /// <inheritdoc />
        public override Brush ForegroundBrush
        {
            get { return _foregroundBrush!; }
        }

        /// <inheritdoc />
        public override Brush BackgroundBrush
        {
            get { return _backgroundBrush!; }
        }

        /// <inheritdoc />
        public override BaselineAlignment BaselineAlignment
        {
            get { return _baselineAlignment; }
        }

        public override CultureInfo CultureInfo
        {
            get { return _culture; }
        }

        /// <inheritdoc />
        public override TextRunTypographyProperties TypographyProperties
        {
            get { return null!; }
        }

        /// <inheritdoc />
        public override TextEffectCollection TextEffects
        {
            get { return null!; }
        }

        /// <inheritdoc />
        public override NumberSubstitution NumberSubstitution
        {
            get { return null!; }
        }

        // public SymbolDisplayPart SymbolDisplaYPart { get; set; }
        // public ITypeSymbol TypeSymbol { get; set; }
        // public SyntaxToken SyntaxToken { get; set; }
        // public SyntaxTrivia SyntaxTrivia { get; set; }
        // public string Text { get; set; }

        #endregion

        #region Private Fields

        private readonly Typeface? _typeface;
        private double _emSize;
        private readonly double _emHintingSize;
        private readonly TextDecorationCollection? _textDecorations;
        private readonly Brush? _foregroundBrush;
        private readonly Brush? _backgroundBrush;
        private readonly BaselineAlignment _baselineAlignment;
        private readonly CultureInfo _culture;

        #endregion

        /// <inheritdoc />
        public void SetFontRenderingEmSize(double emSize)
        {
             _emSize = emSize;
        }
    }
}