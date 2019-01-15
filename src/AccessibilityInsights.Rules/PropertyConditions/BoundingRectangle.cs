// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Types;
using AccessibilityInsights.Rules.Extensions;
using AccessibilityInsights.Rules.Resources;

namespace AccessibilityInsights.Rules.PropertyConditions
{
    /// <summary>
    /// Contains commonly used conditions for testing against the BoundingRectangle property of an IA11yElement.
    /// </summary>
    static class BoundingRectangle
    {
        private const int MinimumArea = 25;

        /// <summary>
        /// margin of error for comparing rectangles.
        /// Using a margin allows small boundary cases not to be marked as errors.
        /// increased margin from 30 to 35 since there is HDPI impact. 
        /// </summary>
        public const int OverlapMargin = 35;

        public static Condition Null = Condition.Create(IsBoundingRectangleNull);
        public static Condition NotNull = ~Null;
        public static Condition Empty = Condition.Create(IsBoundingRectangleEmpty);
        public static Condition NotEmpty = ~Empty;
        public static Condition Valid = Condition.Create(HasValidArea);
        public static Condition NotValid = ~Valid;
        public static Condition CorrectDataFormat = Condition.Create(HasCorrectDataFormat);
        public static Condition NotCorrectDataFormat = ~CorrectDataFormat;
        public static Condition CompletelyObscuresContainer = Condition.Create(ElementCompletelyObscuresContainer, ConditionDescriptions.BoundingRectangleCompletelyObscuresContainer);

        private static bool IsBoundingRectangleNull(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            return !e.TryGetPropertyValue(PropertyType.UIA_BoundingRectanglePropertyId, out double[] array);
        }

        private static bool HasCorrectDataFormat(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            if (!e.TryGetPropertyValue(PropertyType.UIA_BoundingRectanglePropertyId, out double[] array)) return false;
            if (array == null) return false;

            return array.Length == 4;
        }

        private static bool IsBoundingRectangleEmpty(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));
            if (e.BoundingRectangle == null) throw new InvalidOperationException(nameof(e.BoundingRectangle));

            // Because IA11yElement.BoundingRectangle will return an empty rectangle when the property value is null,
            // we are expecting that this function is not called unless the nullity of the property has already been checked.

            return e.BoundingRectangle.IsEmpty;
        }

        private static bool HasValidArea(IA11yElement e)
        {
            if (e == null) throw new ArgumentException(nameof(e));

            return (e.BoundingRectangle.Width * e.BoundingRectangle.Height) >= MinimumArea;
        }

        private static bool ElementCompletelyObscuresContainer(IA11yElement e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));

            var container = e.FindContainerElement();
            if (container == null) throw new Exception("Expected to find a valid ancestor element.");

            return e.BoundingRectangle.CompletelyObscures(container.BoundingRectangle);
        }
    } // class
} // namespace
