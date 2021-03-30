// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;

namespace AccessibilityInsights.Extensions.Telemetry
{
    /// <summary>
    /// Helpers to extract data from the OS
    /// </summary>
    internal static class OSHelpers
    {
        /// <summary>
        /// Get the current Windows version (intended to be used for telemetry). If trying to
        /// determine a minimum build for features, please use the IsWindowsXXXOrLater methods
        /// (or create a new one if you can't find an appropriate one).
        /// </summary>
        /// <param name="registryProvider">Allows the default Registry.GetValue call to get
        /// replaced with a test version. Leave null for production code</param>
        /// <returns>A string in the format X.Y[.Z] where X.Y is the registry-based CurrentVersion
        /// and Z is the registry-based CurrentBuild. Returns "unknown" on error</returns>
        internal static string GetVersion(Func<string, string, string, string> registryProvider = null)
        {
            Func<string, string, string, string> registryGetStringValue = registryProvider ?? RegistryGetStringValue;
            const string WindowsVersionRegKey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion";

            string osVersion = registryGetStringValue(WindowsVersionRegKey, "CurrentVersion", string.Empty);
            if (string.IsNullOrEmpty(osVersion))
                return "unknown";

            string osBuild = registryGetStringValue(WindowsVersionRegKey, "CurrentBuild", string.Empty);

            if (string.IsNullOrEmpty(osBuild))
                return osVersion;

            return osVersion + "." + osBuild;
        }

        private static string RegistryGetStringValue(string keyName, string valueName, string defaultValue)
        {
            return (string)Registry.GetValue(keyName, valueName, defaultValue);
        }
    }
}
