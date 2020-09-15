using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public interface IBaseTextSourceClient
    {
        ICustomTextSource? TextSource { get; }

        double FontSize { get; set; }
        FontWeight FontWeight { get; set; }
        double PixelsPerDip { get; set; }
        double OutputWidth { get; set; }
        double XOffset { get; set; }
        string? FontFamilyName { get; }
        FontFamily? FontFamily { get; set; }

    }
}