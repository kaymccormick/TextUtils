using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace TextUtils
{
    public class AppPointHitTestResult : PointHitTestResult
    {
        public IEnumerable<(DependencyObject Key, IEnumerable<DependencyObject>)> Results { get; }

        /// <inheritdoc />
        public AppPointHitTestResult(Visual visualHit, Point pointHit,
            IEnumerable<(DependencyObject Key, IEnumerable<DependencyObject>)> results) : base(visualHit, pointHit)
        {
            Results = results;
        }
    }
}