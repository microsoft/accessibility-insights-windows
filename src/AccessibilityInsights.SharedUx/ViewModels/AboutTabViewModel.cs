// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Win32;
using System;
using System.Globalization;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// A view model for the AboutTabControl.xaml.cs
    /// </summary>
    public class AboutTabViewModel : ViewModelBase
    {
#pragma warning disable CA1822

        public string UIAccessStatus => NativeMethods.IsRunningWithUIAccess() ?
            Properties.Resources.LabelUIAccessAvailable :
            Properties.Resources.LabelUIAccessNotAvailable;

        public Uri VersionInfoUri
        {
            get
            {
                try
                {
                    string compressedVersion = VersionTools.GetAppVersion();
                    Version version = Version.Parse(compressedVersion);
                    return new Uri(string.Format(CultureInfo.InvariantCulture,
                        Properties.Resources.VersionLinkFormat,
                        version.Major,
                        version.Minor,
                        version.Build,
                        version.Revision));
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


        public string VersionInfoLabel => string.Format(CultureInfo.InvariantCulture,
            Properties.Resources.VersionLinkContentFormat, VersionTools.GetAppVersion());

#pragma warning restore CA1822
    }
}
