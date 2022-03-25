// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Utilities
{
    internal static class ContainedControlFinder<T> where T : DependencyObject
    {
        internal static IEnumerable<T> Find(DependencyObject currentControl)
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(currentControl);
            foreach (var child in children)
            {
                if (child is DependencyObject childControl)
                {
                    if (childControl is T typedChildControl)
                    {
                        yield return typedChildControl;
                    }
                    foreach (var descendentControl in Find(childControl))
                    {
                        yield return descendentControl;
                    }
                }
            }
        }
    }
}
