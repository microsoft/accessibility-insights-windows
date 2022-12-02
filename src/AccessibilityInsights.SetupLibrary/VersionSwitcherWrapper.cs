// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Methods to provide support when switching versions (upgrading or changing channels)
    /// </summary>
    public static class VersionSwitcherWrapper
    {
        /// <summary>
        /// Installs a more recent version in response to an upgrade (retain the same channel)
        /// </summary>
        /// <param name="installerUri">The URI to the web-hosted installer</param>
        /// <param name="msiSizeInBytes">The byte count of the MSI on disk, or 0 if unknown</param>
        /// <param name="msiSha512">The SHA512 of the MSI on disk, or null if unknown</param>
        public static void InstallUpgrade(Uri installerUri, int msiSizeInBytes, string msiSha512)
        {
            DownloadAndInstall(installerUri, null, msiSizeInBytes, msiSha512);
        }

        /// <summary>
        /// Installs a different version in response to a channel change
        /// </summary>
        /// <param name="newChannel">The new channel to use</param>
        public static void ChangeChannel(ReleaseChannel newChannel)
        {
            if (ChannelInfoUtilities.TryGetChannelInfo(newChannel, out EnrichedChannelInfo enrichedChannelInfo, null)
                && enrichedChannelInfo.IsValid)
            {
                DownloadAndInstall(new Uri(enrichedChannelInfo.InstallAsset), newChannel, enrichedChannelInfo.MsiSizeInBytes, enrichedChannelInfo.MsiSha512);
                return;
            }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
            throw new ArgumentException("Unable to get channel information", nameof(newChannel));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        /// <summary>
        /// Private method that does the work shared between InstallUpgrade and ChangeChannel
        /// </summary>
        /// <param name="installerUrl">The URL to the web-hosted installer</param>
        /// <param name="newChannel">If not null, the new channel to select</param>
        /// <param name="msiSizeInBytes">The byte count of the MSI on disk, or 0 if unknown</param>
        /// <param name="msiSha512">The SHA512 of the MSI on disk, or null if unknown</param>
        private static void DownloadAndInstall(Uri installerUri, ReleaseChannel? newChannel, int msiSizeInBytes, string msiSha512)
        {
            List<FileStream> fileLocks = new List<FileStream>();
            try
            {
                string installedFolder = GetInstalledVersionSwitcherFolder();
                string temporaryFolder = GetTemporaryVersionSwitcherFolder();
                RemoveFolder(temporaryFolder);
                TreeCopy(installedFolder, temporaryFolder, fileLocks);
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = Path.Combine(temporaryFolder, "AccessibilityInsights.VersionSwitcher.exe"),
                    Arguments = GetVersionSwitcherArguments(installerUri, newChannel, msiSizeInBytes, msiSha512)
                };
                Process.Start(start);
            }
            finally
            {
                // Release all of our file locks
                foreach (FileStream fileLock in fileLocks)
                {
                    fileLock.Dispose();
                }
            }
        }

        /// <summary>
        /// Remove a previous folder, if it exists
        /// </summary>
        private static void RemoveFolder(string folderToDelete)
        {
            if (Directory.Exists(folderToDelete))
            {
                Directory.Delete(folderToDelete, true);
            }
        }

        /// <summary>
        /// TreeCopy source to dest, keeping a lock on each copied file to prevent tampering
        /// </summary>
        private static void TreeCopy(string source, string dest, List<FileStream> fileLocks)
        {
            if (!Directory.Exists(source))
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new ArgumentException("No Source folder found", nameof(source));
#pragma warning restore CA1303 // Do not pass literals as localized parameters

            RecursiveTreeCopy(source, dest, fileLocks);
        }

        /// <summary>
        /// Core function for TreeCopy. Keeps a lock on each file that gets copied to prevent tampering
        /// </summary>
        private static void RecursiveTreeCopy(string source, string dest, List<FileStream> fileLocks)
        {
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            // copy folders
            foreach (string dir in Directory.GetDirectories(source))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                RecursiveTreeCopy(dir, Path.Combine(dest, directoryInfo.Name), fileLocks);
            }

            // copy files, keeping a FileStream to each (to prevent someone from changing them on us)
            foreach (string file in Directory.GetFiles(source))
            {
                FileInfo fileInfo = new FileInfo(file);
                string destFile = Path.Combine(dest, fileInfo.Name);
                fileInfo.CopyTo(destFile, true);
                fileLocks.Add(File.OpenRead(destFile));
                EnsureFilesAreIdentical(file, destFile);
            }
        }

        private static void EnsureFilesAreIdentical(string file1, string file2)
        {
            if (!File.ReadAllBytes(file1).SequenceEqual(File.ReadAllBytes(file2)))
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new IOException("File " + file1 + " does not match " + file2);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }
        }

        /// <summary>
        /// Extract the path to the installed version switcher, based on the app path. The
        /// VersionSwitcher is a sibling folder of the installed app folder
        /// </summary>
        private static string GetInstalledVersionSwitcherFolder()
        {
            string appPath = MsiUtilities.GetAppInstalledPath();
            string root = Path.GetDirectoryName(appPath);
            string versionSwitcherFolder = Path.Combine(root, "VersionSwitcher");
            return versionSwitcherFolder;
        }

        /// <summary>
        /// Find the folder where the temporary VersionSwitcher will go
        /// </summary>
        private static string GetTemporaryVersionSwitcherFolder()
        {
            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath, "VersionSwitcher");
        }

        /// <summary>
        /// Create the arguments to pass to the Version Switcher process
        /// </summary>
        /// <param name="installerUri">The URI to the web-hosted installer</param>
        /// <param name="newChannel">If not null, the new channel to select</param>
        /// <param name="msiSizeInBytes">The byte count of the MSI on disk, or 0 if unknown</param>
        /// <param name="msiSha512">The SHA512 of the MSI on disk, or null if unknown</param>
        private static string GetVersionSwitcherArguments(Uri installerUri, ReleaseChannel? newChannel, int msiSizeInBytes, string msiSha512)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}",
                installerUri.ToString(),
                msiSizeInBytes,
                string.IsNullOrWhiteSpace(msiSha512) ? "none" : msiSha512,
                newChannel.HasValue ? newChannel.Value.ToString() : string.Empty
                );
        }
    }
}
