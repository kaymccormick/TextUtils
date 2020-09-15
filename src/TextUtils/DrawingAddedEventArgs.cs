using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class DrawingAddedEventArgs : RoutedEventArgs
    {
        public Drawing Drawing { get; }
        public DrawingVisualDuo Duo { get; }

        public DrawingAddedEventArgs(Drawing drawing, DrawingVisualDuo duo, object sender)  : base(Surface1.DrawingAdded, sender)
        {
            Drawing = drawing;
            Duo = duo;
        }
    }
}