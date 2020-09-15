using System.Diagnostics;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using JetBrains.Annotations;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace TextUtils
{
    public class HitTestContext
    {
        private int _dcItems;
        private int _zIndex;
        private DrawingVisualDuo? _duoVisual;
        public DrawingVisual? PrimaryDrawingVisual { get; set; }
        public DependencyObject CoreSearchObject { get; }
        public PointHitTestParameters HitTestParameters {   get; }
        public Surface1 MyRect { get; }
        public DependencyObject? CommonAncestor { get; set; }
        public GeneralTransform? TransformToAncestor { get; set; }

        public HitTestContext(DependencyObject coreSearchObject, PointHitTestParameters hitTestParameters,
            [NotNull] Surface1 myRect)
        {
            
            CoreSearchObject = coreSearchObject;
            DependencyObject parent;
            for (parent = CoreSearchObject;
                VisualTreeHelper.GetParent(parent) != null;
                parent = VisualTreeHelper.GetParent(parent)) ;
            RootVisual = (Visual) parent;
            TransformToRoot = ((Visual) CoreSearchObject).TransformToAncestor(RootVisual);
            HitTestParameters = hitTestParameters;
            MyRect = myRect;
            _zIndex = 1;
            DuoVisual = MyRect?.CreateVisual(_zIndex);
            

        }

        public DrawingContext? DrawingContext1 { get; private set; }

        public DrawingVisualDuo? DuoVisual
        {
            get { return _duoVisual; }
            set
            {
                if (DrawingContext1 != null)
                {
                    DrawingContext1.Close();
                    DrawingContext1 = null;
                }
                _duoVisual = value;

                if (_duoVisual != null)
                {
                    PrimaryDrawingVisual = new DrawingVisual();
                    DrawingContext1 = PrimaryDrawingVisual.RenderOpen();
                    _duoVisual.PrimaryVisual = PrimaryDrawingVisual;
                }
            }
        }

        public GeneralTransform TransformToRoot { get; }

        public Visual RootVisual { get; }
        public Rect InitialDescendantBounds { get; set; }
        public Point TransformedPoint { get; set; }

        public void AddDrawing(Drawing drawing)
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                XamlWriter.Save(drawing);
                ProtoLogger2.Instance.LogAction(JsonSerializer.Serialize(new ClientRenderRequest
                    {DrawingXaml = XamlWriter.Save(drawing), DrawingIdentifier = "hittest"}));
            }

            _dcItems++;
            Debug.WriteLine("trying to draw drawing " + drawing.GetType().Name);
            var b = drawing.Bounds;
            Debug.WriteLine(DebugUtils.RoundRect(b));
            DrawingContext1?.DrawDrawing(drawing);
            if (_dcItems % 5 == 0)
            {
                FlushDrawingContext();
            }
            // RoslynCodeBase.SetShrinkToFit(drawing, true);
            // HitTestHelper.MyRect?.Drawings.Add(drawing);
        }

        public void FlushDrawingContext()
        {
            DuoVisual = MyRect.CreateVisual(_zIndex++);
        }

        public HitTestPacket CreatePacket(Drawing d, Visual v)
        {
            return new HitTestPacket(d, v, v.TransformToAncestor(RootVisual), this);
        }

        public Rect FixUpRect(Rect b1)
        {
            if (double.IsNegativeInfinity(b1.X))
            {
                b1.X = 0;
            }
            if(double.IsNegativeInfinity(b1.Y))
            {
                b1.Y = 0;

            }

            return b1;
        }
    }
}