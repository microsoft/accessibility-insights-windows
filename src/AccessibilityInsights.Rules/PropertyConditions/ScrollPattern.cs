// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    /// <summary>
    /// Contains commonly used conditions for testing against the Scroll pattern of an IA11yElement.
    /// </summary>
    static class ScrollPattern
    {
        // pattern property values
        public const string HorizontallyScrollableProperty = "HorizontallyScrollable";
        public const string HorizontalViewSizeProperty = "HorizontalViewSize";
        public const string HorizontalScrollPercentProperty = "HorizontalScrollPercent";
        public const string VerticallyScrollableProperty = "VerticallyScrollable";
        public const string VerticalViewSizeProperty = "VerticalViewSize";
        public const string VerticalScrollPercentProperty = "VerticalScrollPercent";

        public static Condition Null = Condition.Create(IsNull);
        public static Condition NotNull = ~Null;
        public static Condition HorizontallyScrollable = Condition.Create(IsHorizontallyScrollable);
        public static Condition NotHorizontallyScrollable = ~HorizontallyScrollable;
        public static Condition VerticallyScrollable = Condition.Create(IsVerticallyScrollable);
        public static Condition NotVerticallyScrollable = ~VerticallyScrollable;

        private static bool IsNull(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var scrollPattern = e.GetPattern(PatternType.UIA_ScrollPatternId);
            return scrollPattern == null;
        }

        private static bool IsHorizontallyScrollable(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var scrollPattern = e.GetPattern(PatternType.UIA_ScrollPatternId);
            if (scrollPattern == null) return false;

            var horizontalScrollPercent = scrollPattern.GetValue<double>(HorizontalScrollPercentProperty);

            // DirectUI framework returns "NaN" instead of setting IsXXXScrollable to false in case it is not scrollable.
            return scrollPattern.GetValue<bool>(HorizontallyScrollableProperty)
                && !double.IsNaN(horizontalScrollPercent);
        }

        private static bool IsVerticallyScrollable(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            var scrollPattern = e.GetPattern(PatternType.UIA_ScrollPatternId);
            if (scrollPattern == null) return false;

            var verticalScrollPercent = scrollPattern.GetValue<double>(VerticalScrollPercentProperty);

            // DirectUI framework returns "NaN" instead of setting IsXXXScrollable to false in case it is not scrollable.
            return scrollPattern.GetValue<bool>(VerticallyScrollableProperty)
                && !double.IsNaN(verticalScrollPercent);
        }
    } // class
} // namespace
