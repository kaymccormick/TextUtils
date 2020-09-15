using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace TextUtils
{
    [ContentProperty("TextElement")]
    public class BasicTextPanel : Panel
    {
        private readonly Dictionary<UIElement, Size> _measures = new Dictionary<UIElement, Size>();

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            Debug.WriteLine($"KTE: {nameof(BasicTextPanel)}: {nameof(ArrangeOverride)}:called with  finalSize of {finalSize}");

            Rect rect = new Rect(0,0,0,0);
            foreach (UIElement? child in Children)
            {
                if (child == null)
                    continue;
                if(_measures.TryGetValue(child, out var r))
                {
                    var childRect = new Rect(r);
                    Debug.WriteLine(
                        $"KTE: {nameof(BasicTextPanel)}: {nameof(ArrangeOverride)}: calling arrange on {child} with {childRect}");

                    child.Arrange(childRect);
                    rect.Union(childRect);
                } else
                {
                    child.Arrange(new Rect(finalSize));
                    rect.Union(new Rect(finalSize));
                }


                
            }
            var resultSize = finalSize;

            Debug.WriteLine($"KTE: {nameof(BasicTextPanel)}: {nameof(ArrangeOverride)}:returning value of {resultSize}");
            return resultSize;
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            Debug.WriteLine($"KTE: {nameof(BasicTextPanel)}: {nameof(MeasureOverride)}: {availableSize}");
            _measures.Clear();
            Rect mySize = new Rect(new Size(0, 0));
            foreach (UIElement? u in Children)
            {
                if (u == null) continue;
                u.Measure(availableSize);
                var z = u.DesiredSize;
                Debug.WriteLine($"KTE: {nameof(BasicTextPanel)}: {nameof(MeasureOverride)}: desiredSize of {u} is {u.DesiredSize}");
                _measures[u] = z;
                mySize.Union(new Rect(z));
            }



            Debug.WriteLine($"KTE: {nameof(BasicTextPanel)}: {nameof(MeasureOverride)}: returning {mySize.Size}");
            return mySize.Size;
        }
        

        private BasicTextElement? _textElement;

        /// <inheritdoc />
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            // foreach (var u in TextElement.UiElements)
            // {
            //     Children.Add(u);
            // }
            if (TextElement != null) Children.Add(TextElement);
        }

        public BasicTextElement? TextElement
        {
            get { return _textElement; }
            set
            {
                _textElement = value;
                if (_textElement != null) _textElement.Panel = this;
            }
        }
    }
}