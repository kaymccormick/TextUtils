using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class HitTestPacket 
    {
        public Drawing Drawing { get; }
        public Visual Visual { get; }
        public GeneralTransform TransformToRoot { get; }
        public HitTestContext Context { get; }

        public HitTestPacket(Drawing drawing, Visual visual, GeneralTransform transformToRoot, HitTestContext context)
        {
            Drawing = drawing;
            Visual = visual;
            TransformToRoot = transformToRoot;
            Context = context;
        }

        public Point Transform(Point point)
        {
            return TransformToRoot.Transform(point);
        }

        public HitTestPacket Descendant(Drawing drawing)
        {
            return Context.CreatePacket(drawing, Visual);
        }
    }

    
}