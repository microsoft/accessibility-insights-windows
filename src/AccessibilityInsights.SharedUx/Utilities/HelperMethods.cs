﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.Win32;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using System;
using System.Drawing;
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

                Guid? ecId = SelectAction.GetDefaultInstance()?.SelectedElementContextId;
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
        /// Get DPI
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        internal static double GetDPI(this Rectangle rc)
        {
            return GetDPI(rc.Left, rc.Top);
        }

        /// <summary>
        /// Get DPI with left/top
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <returns></returns>
        internal static double GetDPI(int left, int top)
        {
            NativeMethods.GetDpi(new System.Drawing.Point(left, top), DpiType.Effective, out uint dpiX, out _);

            return GetDPIRate(dpiX);
        }

        /// <summary>
        /// turn raw DPI value to DPI rate
        /// </summary>
        /// <param name="dpiX"></param>
        /// <returns></returns>
        private static double GetDPIRate(uint dpi)
        {
            return dpi / 96.0; // 96 is 100% scale.
        }

        /// <summary>
        /// Gets the DPI that WPF seems to use internally
        /// when positioning windows; scale by this DPI
        /// when setting Window.Left / Window.Top before calling show()
        /// </summary>
        /// <returns></returns>
        internal static double GetWPFWindowPositioningDPI()
        {
            return new Rectangle().GetDPI();
        }
    }
}
