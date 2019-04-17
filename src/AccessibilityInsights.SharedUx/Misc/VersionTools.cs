// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SharedUx.Misc
{
    public static class VersionTools
    {
        /// <summary>
        /// Get version from AccessibilityInsights.SharedUx Assembly
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;
            return fileVersion;
        }
    }
}
