using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.TextFormatting;

namespace TextUtils
{
    public class TextEmbeddedObject1 : TextEmbeddedObject
    {
        private Rect _computeBoundingBox;

        public TextEmbeddedObject1(CharacterBufferReference characterBufferReference, int length, TextRunProperties properties,
            bool hasFixedSize, UIElement uiElement)
        {
            CharacterBufferReference = characterBufferReference;
            Length = length;
            Properties = properties;
            HasFixedSize = hasFixedSize;
            UiElement = uiElement;
        }

        /// <inheritdoc />
        public override CharacterBufferReference CharacterBufferReference { get; }

        /// <inheritdoc />
        public override int Length { get; }

        /// <inheritdoc />
        public override TextRunProperties Properties { get; }

        /// <inheritdoc />
        public override Rect ComputeBoundingBox(bool rightToLeft, bool sideways)
        {
            UiElement.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            _computeBoundingBox = new Rect(UiElement.DesiredSize);
            return _computeBoundingBox;
        }

        /// <inheritdoc />
        public override void Draw(DrawingContext drawingContext, Point origin, bool rightToLeft, bool sideways)
        {
            // UiElement.Arrange(new Rect(_computeBoundingBox.Size));
            // VisualBrush b = new VisualBrush(UiElement);
            // b.AutoLayoutContent = true;
            
            var rectangle = new Rect(origin, _computeBoundingBox.Size);
            drawingContext.DrawRectangle(Brushes.Green, null, rectangle);
        }

        /// <inheritdoc />
        public override TextEmbeddedObjectMetrics Format(double remainingParagraphWidth)
        {
            var r = ComputeBoundingBox(false, false);
            return new TextEmbeddedObjectMetrics(r.Width, r.Height, 0);
        }

        /// <inheritdoc />
        public override LineBreakCondition BreakAfter { get; }

        /// <inheritdoc />
        public override LineBreakCondition BreakBefore { get; }

        /// <inheritdoc />
        public override bool HasFixedSize { get; }

        public UIElement UiElement { get; }
    }
}