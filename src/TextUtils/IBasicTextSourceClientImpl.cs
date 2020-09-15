using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public interface IBasicTextSourceClientImpl
    {
        void AddSpan(TextSpan<CharacteristicsImpl> textSpan);
    }
}