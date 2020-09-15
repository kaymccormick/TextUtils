using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace TextUtils
{
    public class BasicTextSourceClient : BaseTextSourceClient
    {
        [CanBeNull] private readonly BasicTextSource? _textSource;
        private string? _text;
        private TextRunProperties? _defaultProperties;

        /// <inheritdoc />
        public BasicTextSourceClient([CanBeNull] BasicTextSource? textSource) : base(textSource)
        {
            _textSource = textSource;
        }

        public string? Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _textSource.Text = _text;
            }
        }

        public TextRunProperties? DefaultProperties1
        {
            get { return _defaultProperties; }
            set { _defaultProperties = value; }
        }

        public void LoadSource() => TextSource.Load();

        public TextParagraphProperties DefaultProperties()
        {
            var t = new TextParagraphProperties1(_defaultProperties ?? new TextRunProperties1(null, BaselineAlignment.Top, CultureInfo.CurrentUICulture, 12.0, 12.0, Brushes.Black, new TextDecorationCollection(), new TextEffectCollection(),
                new Typeface(new FontFamily("Arial"), default, default, default)), true);

            if (ProxyHelper.IsProxyEnabled)
                return (TextParagraphProperties) ProxyHelper.ProxyGenerator.CreateClassProxyWithTarget(
                    typeof(TextParagraphProperties1), t,
                    new TextInterceptor());
            return t;
        }
    }

    public class TextInterceptor : IInterceptor
    {
        /// <inheritdoc />
        public void Intercept(IInvocation invocation)
        {
            Debug.WriteLine((string?) invocation.Method.Name);

            invocation.Proceed();
        }
    }

    public class ProxyHelper
    {
        private static readonly ProxyGenerator _proxyGenerator = new ProxyGenerator();

        public static ProxyGenerator ProxyGenerator
        {
            get { return _proxyGenerator; }
        }

        public static bool IsProxyEnabled => false;
    }
}