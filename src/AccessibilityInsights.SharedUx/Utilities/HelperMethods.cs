// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.SharedUx.FileBug;
using System;
using System.Windows;

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
                return BugReporter.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
