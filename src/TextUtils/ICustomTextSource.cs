using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICustomTextSource : IAppTextSource
    {
        /// <summary>
        /// 
        /// </summary>
        int EolLength { get; }

        object BtRuns { get; }
         FontRendering? CurrentRendering { get; }
         ThreadLocal<FontRendering?> ThreadRendering { get; }
         bool IsLoaded { get; }
         string? FamilyName { get; set; }
         double FontSize { get; set; }
         double LineHeight { get; set; }
         FontFamily? FontFamily { get; set; }
         char[] TextBuffer { get; set; }


         TextRunProperties MakeProperties(object arg, string text);

        Task<object> TextInputAsync(int? insertionPoint, InputRequest inputRequest);
        Task<object?> TextChangeAsync(TextChange change);
        TextSource AsTextSource();
        void Load();
        bool TryLoad();
    }


}