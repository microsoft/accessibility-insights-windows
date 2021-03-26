// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.Misc
{
    /// <summary>
    /// Used to facilitate moving between panes with F6
    /// </summary>
    static class FrameworkNavigator
    {
        /// <summary>
        /// Based on a given FrameworkElement, gets the next pane in the given list.
        /// </summary>
        /// <param name="e">
        /// Object from which the F6 command was issued.
        /// This is expected to be a FrameworkElement from the list of FrameworkElements given in parameter 2.
        /// </param>
        /// <param name="panes"></param>
        public static FrameworkElement GetSubsequentFrameworkElement(FrameworkElement e, IReadOnlyList<FrameworkElement> panes)
        {
            if (e == null)
                return null;
            if (panes == null)
                return null;

            var count = panes.Count;
            for (int i = 0; i < count; ++i)
            {
                if (panes[i] != e)
                    continue;

                var next = ++i < count ? i : 0;
                return panes[next];
            } // for

            return null;
        }

        /// <summary>
        /// Finds the first focusable element inside the given element and puts focus there.
        /// This is used for F6 navigation.
        /// </summary>
        /// <param name="e"></param>
        public static void MoveFocusToFrameworkElement(FrameworkElement e)
        {
            var traversalRequest = new TraversalRequest(FocusNavigationDirection.First);
            e?.MoveFocus(traversalRequest);
        }
    } // class
} // namespace
