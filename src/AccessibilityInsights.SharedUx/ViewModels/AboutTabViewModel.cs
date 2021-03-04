// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Win32;
using System;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// A view model for the AboutTabControl.xaml.cs
    /// </summary>
    public class AboutTabViewModel : ViewModelBase
    {
#pragma warning disable CA1822

        /// <summary>
        /// Provides the text for the UIAccess label
        /// </summary>
        public string UIAccessStatus => NativeMethods.IsRunningWithUIAccess() ?
            Properties.Resources.LabelUIAccessAvailable :
            Properties.Resources.LabelUIAccessNotAvailable;

        /// <summary>
        /// Provides a Uri to the release notes for this version
        /// </summary>
        public Uri VersionInfoUri
        {
            get
            {
                try
                {
                    return VersionTools.AppVersionUri;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                    return null;
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        /// <summary>
        /// Provides the version-specific label
        /// </summary>
        public string VersionInfoLabel => VersionTools.AppVersionLabel;

        public string AxeVersion => VersionTools.AxeVersion;
#pragma warning restore CA1822
    }
}
