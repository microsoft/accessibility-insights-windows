// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Settings;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Utilities
{
    public static class HelperMethods
    {
        /// <summary>
        /// Visibility for the FileBug button. This defaults to Visible if the bug filing is enabled,
        /// unless the current data context has so many items that we would refuse to load the resulting file
        /// </summary>
        public static Visibility FileBugVisibility
        {
            get
            {
                if (GeneralFileBugVisibility == Visibility.Collapsed)
                    return Visibility.Collapsed;

                Guid? ecId = SelectAction.GetDefaultInstance()?.GetSelectedElementContextId();
                if (ecId.HasValue)
                {
                    ElementDataContext dataContext = GetDataAction.GetElementDataContext(ecId.Value);

                    if (dataContext != null && dataContext.ElementCounter.UpperBoundExceeded)
                    {
                        return Visibility.Collapsed;
                    }
                }

                return Visibility.Visible;
            }
        }

        /// <summary>
        /// General visibilty status of bug-filing features. This is visible when a bug filing
        /// extension exists, otherwise it's collapsed. Used when the decision is independent
        /// of the selected ElementContext
        /// </summary>
        public static Visibility GeneralFileBugVisibility
        {
            get
            {
                return IssueReporter.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Get a "File Issue" column width that is appropriate for the currently configured font size
        /// </summary>
        public static double FileIssueColumnWidth
        {
            get
            {
                switch (ConfigurationManager.GetDefaultInstance().AppConfig.FontSize)
                {
                    case Enums.FontSize.Small: return 80.0;
                    default: return 90.0;
                }
            }
        }

        /// <summary>
        /// Provides bindable property for ProgressRingControls
        /// </summary>
        public static bool PlayScanningSound => ConfigurationManager.GetDefaultInstance().AppConfig.PlayScanningSound;

        /// <summary>
        /// Finds all controls of the given type under the given object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<T> FindChildren<T>(DependencyObject element) where T : DependencyObject
        {
            if (element != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(element, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }
                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        /// <summary>
        /// Get child element of specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static DependencyObject GetFirstChildElement<T>(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            if (element is T)
            {
                return element as DependencyObject;
            }

            for (int x = 0; x < VisualTreeHelper.GetChildrenCount(element); x++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, x);
                var b = GetFirstChildElement<T>(child);

                if (b != null)
                    return b;
            }
            return null;
        }
    }
}
