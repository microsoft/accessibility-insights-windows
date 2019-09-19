// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Core.Misc;
using System.Diagnostics;
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

        public static string GetAxeVersion() => PackageInfo.InformationalVersion;
    }
}
