using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class AppGeometryHitTestResult : GeometryHitTestResult
    {
        public IEnumerable<(DependencyObject, DependencyObject, IntersectionDetail?)> ValueTuples { get; }

        /// <inheritdoc />
        public AppGeometryHitTestResult(Visual visualHit, IntersectionDetail intersectionDetail,
            IEnumerable<(DependencyObject, DependencyObject, IntersectionDetail?)> valueTuples) : base(visualHit, intersectionDetail)
        {
            ValueTuples = valueTuples;
        }
    }
}