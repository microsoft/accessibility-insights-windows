// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Drawing;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Misc;
using Axe.Windows.Core.Types;
using Axe.Windows.Rules.PropertyConditions;

using static System.FormattableString;

namespace Axe.Windows.Rules.Extensions
{
    static class ExtensionMethods
    {
        /// <summary>
        /// Returns true if the given element is a tab item
        /// and is in the WPF or XAML frameworks.
        /// These frameworks are very similar.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsWPFTabItem(this IA11yElement e)
        {
            if (e == null) return false;

            if (e.ControlTypeId != Axe.Windows.Core.Types.ControlType.UIA_TabItemControlTypeId)
                return false;

            var framework = e.GetUIFramework();
            return framework == Framework.WPF
                || framework == Framework.XAML;
        }

        public static T GetPropertyValueOrDefault<T>(this IA11yElement e, int propertyId)
        {
            if (e == null) return default(T);

            return e.TryGetPropertyValue(propertyId, out T value)
                ? value : default(T);
        }

        public static Condition WithTimer(this Condition c, string message)
        {
            if (c == null) return null;

            return Condition.Create(e =>
            {
                var stopwatch = Stopwatch.StartNew();
                bool retVal = c.Matches(e);
                var ms = stopwatch.ElapsedTicks / (float)10000;
                Debug.WriteLine(Invariant($"{ms} ms {message}"));
                return retVal;
            });
        }

        /// <summary>
        /// When an Element is an item like TreeItem or TabItem,
        /// the container is the TreeView or Tab.
        /// If not the tree item or tab,
        /// the function searches for a scrollable container.
        /// This function may skip direct parents to find the container ancestor.
        /// However, as long is a parent is available, this function will always return an element,
        /// even if the expected container is not found.
        /// </summary>
        public static IA11yElement FindContainerElement(this IA11yElement e)
        {
            if (e == null) return null;
            if (e.Parent == null) return null;

            // WPF tab item children consist of the entire page content
            // which most likely does not fit inside the small rectangle for the tab item itself.
            // So we consider the tab item to be the starting element when searching for the parent rectangle.
            if (e.Parent.IsWPFTabItem())
                e = e.Parent;

            var containerControlType = GetContainerType(e.ControlTypeId);
            if (containerControlType == 0)
            {
                var scrollableElement = FindScrollableAncestor(e.Parent);

                return scrollableElement ?? e.Parent;
            }

            var container = FindAncestorOfType(e.Parent, containerControlType);
            if (container == null)
                return e.Parent;

            return container;
        }

        private static int GetContainerType(int controlType)
        {
            switch (controlType)
            {
                case Axe.Windows.Core.Types.ControlType.UIA_TabItemControlTypeId:
                    return Axe.Windows.Core.Types.ControlType.UIA_TabControlTypeId;
                case Axe.Windows.Core.Types.ControlType.UIA_TreeItemControlTypeId:
                    return Axe.Windows.Core.Types.ControlType.UIA_TreeControlTypeId;
            }

            return 0;
        }

        public static IA11yElement FindAncestorOfType(this IA11yElement e, int ctrlType)
        {
            if (e == null) return null;

            return e.ControlTypeId == ctrlType
                ? e : e.Parent?.FindAncestorOfType(ctrlType);
        }

        private static IA11yElement FindScrollableAncestor(IA11yElement e)
        {
            if (e == null) return null;
            if (e.IsRootElement()) return null;

            var pattern = e.GetPattern(PatternType.UIA_ScrollPatternId);

            return pattern != null
                ? e : FindScrollableAncestor(e.Parent);
        }

        public static int Area(this Rectangle rect)
        {
            if (rect == null) throw new ArgumentNullException(nameof(rect));
            return rect.Size.Width * rect.Size.Height; 
        }

        public static bool CompletelyObscures(this Rectangle a, Rectangle b)
        {
            if (a == null) return false;
            if (b == null) return false;
            if (a.IsEmpty) return false;
            if (b.IsEmpty) return false;

            return a.Left <= (b.Left - BoundingRectangle.OverlapMargin)
                && a.Top <= (b.Top - BoundingRectangle.OverlapMargin)
                && a.Right >= (b.Right + BoundingRectangle.OverlapMargin)
                && a.Bottom >= (b.Bottom + BoundingRectangle.OverlapMargin)
                && a.Area() != (b.Area() + (BoundingRectangle.OverlapMargin * 4));
        }
    } // class
} // namespace
