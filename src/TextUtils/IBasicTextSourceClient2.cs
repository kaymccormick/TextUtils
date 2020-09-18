using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public interface IBasicTextSourceClient2
    {
        void AddSpan(TextSpan<CharacteristicsImpl> textSpan);
    }
}