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
        /// The URI to the web-hosted installer to use
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
        /// The MSI size for validation. A value of 0 means that no validation is possible
        /// </summary>
        internal int MsiSizeInBytes { get; }

        /// <summary>
        /// The SHA512 of the MSI file. A value of null means that no validation is possible
        /// </summary>
        internal string MsiSha512 { get; }

        /// <summary>
        /// The Timeout for downloads
        /// </summary>
        internal TimeSpan DownloadTimeout { get; }

        /// <summary>
        /// Whether or not UIAccess should be enabled post-install
        /// </summary>
        internal bool EnableUIAccess { get; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="msiPath">The uri to the web-hosted installer</param>
        /// <param name="msiSizeInBytes">The byte count of the MSI on disk, or 0 if unknown</param>
        /// <param name="msiSha512">The SHA512 of the MSI on disk, or null if unknown</param>
        /// <param name="newChannel">String indicating the value for NewChannel</param>
        /// <param name="enableUIAccess">True only if we need to enable UIAccess post-install"</param>
        internal InstallationOptions(Uri msiPath, int msiSizeInBytes, string msiSha512, string newChannel, bool enableUIAccess)
        {
            MsiPath = msiPath;
            LocalInstallerFile = Path.ChangeExtension(Path.GetTempFileName(), "msi");
            MsiSizeInBytes = msiSizeInBytes;
            MsiSha512 = msiSha512;
            NewChannel = newChannel;
            DownloadTimeout = TimeSpan.FromSeconds(60);
            EnableUIAccess = enableUIAccess;
        }
    }
}
