// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Class to hold the installation options
    /// </summary>
    internal class InstallationOptions
    {
        /// <summary>
        /// The Uri to the iweb-hosted nstaller to use
        /// </summary>
        internal Uri MsiPath { get; }

        /// <summary>
        /// The path to the local installer
        /// </summary>
        internal string LocalInstallerFile { get; }

        /// <summary>
        /// The new Channel (null if channel is not changing)
        /// </summary>
        internal string NewChannel { get; }

        /// <summary>
        /// The Timeout for downloads
        /// </summary>
        internal TimeSpan DownloadTimeout { get; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="msiPath">String identifying the installer location</param>
        /// <param name="newChannel">String indicating the value for NewChannel</param>
        internal InstallationOptions(string msiPath, string newChannel = null)
        {
            MsiPath = new Uri(msiPath);
            NewChannel = newChannel;
            LocalInstallerFile = Path.ChangeExtension(Path.GetTempFileName(), "msi");
            DownloadTimeout = TimeSpan.FromSeconds(60);
        }
    }
}
