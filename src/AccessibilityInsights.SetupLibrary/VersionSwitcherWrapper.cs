// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

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
        /// <param name="installerUri">The uri to the web-hosted installer</param>
        /// <returns>true if the Version Switcher process started successfully, false if not</returns>
        public static bool InstallUpgrade(Uri installerUri)
        {
            return DownloadAndInstall(installerUri, null);
        }

        /// <summary>
        /// Installs a different version in response to a channel change
        /// </summary>
        /// <param name="installerUri">The uri to the web-hosted installer</param>
        /// <param name="newChannel">The new channel to use</param>
        /// <returns>true if the Version Switcher process started successfully, false if not</returns>
        public static bool ChangeChannel(Uri installerUri, string newChannel)
        {
            return DownloadAndInstall(installerUri, newChannel);
        }

        /// <summary>
        /// Private method that does the work shared between InstallUpgrade and ChangeChannel
        /// </summary>
        /// <param name="installerUrl">The uri to the web-hosted installer</param>
        /// <param name="newChannel">If not null, the new channel to select</param>
        /// <returns>true if the Version Switcher process started successfully, false if not</returns>
        private static bool DownloadAndInstall(Uri installerUri, string newChannel)
        {
            List<FileStream> fileLocks = new List<FileStream>();
            try
            {
                string installedFolder = GetInstalledVersionSwitcherFolder();
                string temporaryFolder = GetTemporaryVersionSwitcherFolder();
                RemoveFolder(temporaryFolder);
                if (TryRecursiveCopy(installedFolder, temporaryFolder, fileLocks))
                {
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        FileName = Path.Combine(temporaryFolder, "AccessibilityInsights.VersionSwitcher.exe"),
                        Arguments = GetVersionSwitcherArguments(installerUri, newChannel)
                    };
                    return Process.Start(start).Id != 0;
                }
                return false;
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
        /// Core function for xcopy. Keeps a lock on each file that gets copied to prevent tampering
        /// </summary>
        /// <returns>true if the copy succeeded</returns>
        private static bool TryRecursiveCopy(string source, string dest, List<FileStream> fileLocks)
        {
            try
            {
                if (!Directory.Exists(source))
                    return false;

                if (!Directory.Exists(dest))
                {
                    Directory.CreateDirectory(dest);
                }

                // copy files, keeping a FileStream to each (to prevent someone from changing them on us)
                foreach (string file in Directory.GetFiles(source))
                {
                    FileInfo fileInfo = new FileInfo(file);
                    fileInfo.CopyTo(Path.Combine(dest, fileInfo.Name), true);
                    fileLocks.Add(File.OpenRead(dest));
                }

                // copy folders
                foreach (string dir in Directory.GetDirectories(source))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                    if (!TryRecursiveCopy(dir, Path.Combine(dest, directoryInfo.Name), fileLocks))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AccessibilityInsights - exception when copying tree: " + ex.ToString());
                return false;
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
        /// <param name="installerUri">The uri to the web-hosted installer</param>
        /// <param name="newChannel">If not null, the new channel to select</param>
        private static string GetVersionSwitcherArguments(Uri installerUri, string newChannel)
        {
            string arguments = installerUri.ToString();

            if (newChannel != null)
            {
                arguments += " " + newChannel;
            }

            return arguments;
        }
    }
}
