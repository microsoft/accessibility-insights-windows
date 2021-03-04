// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Misc;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace AccessibilityInsights.SharedUx.Misc
{
    public static class VersionTools
    {
        /// <summary>
        /// Get version from AccessibilityInsights.SharedUx Assembly
        /// </summary>
        /// <returns>The file version string (as formatted in the file resources)</returns>
        public static string GetAppVersion()
        {
            string fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            return fileVersion;
        }

        /// <summary>
        /// Return the label for use in UI
        /// </summary>
        public static string AppVersionLabel => string.Format(CultureInfo.InvariantCulture,
            Properties.Resources.VersionLinkContentFormat, GetAppVersion());

        /// <summary>
        /// Provides a Uri to the release notes for this version
        /// </summary>
        public static Uri AppVersionUri
        {
            get
            {
                string compressedVersion = GetAppVersion();
                Version version = Version.Parse(compressedVersion);
                return new Uri(string.Format(CultureInfo.InvariantCulture,
                    Properties.Resources.VersionLinkFormat,
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision));
            }
        }

        public static string AxeVersion => PackageInfo.InformationalVersion;
    }
}
