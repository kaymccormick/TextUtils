using System.Collections.Generic;
using System.Windows.Media;

namespace TextUtils
{
    public static class ColorHelper
    {
        public static IEnumerator<Color> GetColors()
        {
            for (;;)
            {
                yield return Colors.DarkBlue;
                yield return Colors.Cyan;
                yield return Colors.Violet;
                yield return Colors.YellowGreen;
                yield return Colors.Brown;
                yield return Colors.CadetBlue;
                yield return Colors.Tomato;
                yield return Colors.SkyBlue;
            }
        }
    }
}