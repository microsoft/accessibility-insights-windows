// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Methods to help with execution of AccessibilityInsights.VersionSwitcher.exe
    /// </summary>
    public static class VersionSwitcherWrapper
    {
        /// <summary>
        /// Download and run the specified installer
        /// </summary>
        /// <param name="installerUrl">The uri to the web-hosted installer</param>
        /// <param name="newChannel">The new channel to set (leave null to retain the current value)</param>
        /// <returns>true if the process started successfully, false if not</returns>
        public static bool DownloadAndRun(Uri installerUrl, string newChannel)
        {
            List<FileStream> fileLocks = new List<FileStream>();
            try
            {
                string temporaryFolder = GetTemporaryVersionSwitcherFolder();
                string installedFolder = GetInstalledVersionSwitcherFolder();
                RemoveFolder(temporaryFolder);
                if (TryRecursiveCopy(installedFolder, temporaryFolder, fileLocks))
                {
                    ProcessStartInfo start = new ProcessStartInfo
                    {
                        FileName = Path.Combine(temporaryFolder, "AccessibilityInsights.VersionSwitcher.exe"),
                        Arguments = GetAppArguments(installerUrl, newChannel)
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
            try
            {
                if (Directory.Exists(folderToDelete))
                {
                    Directory.Delete(folderToDelete, true);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("AccessibilityInsights - exception when removing folder: " + ex.ToString());
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
                    fileInfo.CopyTo(Path.Combine(dest, file), true);
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
        /// Find the folder where the Version Switcher is installed (based on the File.Open from the registry)
        /// </summary>
        private static string GetInstalledVersionSwitcherFolder()
        {
            RegistryKey commandKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\A11y.Test\shell\open\command");
            {
                string command = ((string)commandKey.GetValue(""));
                string appPath = command.Substring(0, command.Length - 5).Replace("\"", "");
                string root = Path.GetDirectoryName(appPath);
                string versionSwitcherFolder = Path.Combine(root, "VersionSwitcher");
                Console.WriteLine(versionSwitcherFolder);
                return versionSwitcherFolder;
            }
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
        /// Create the command line to pass to the Version Switcher
        /// </summary>
        /// <param name="installerUrl">Where the installer lives</param>
        /// <param name="newChannel">The new channel (null if not changing)</param>
        private static string GetAppArguments(Uri installerUrl, string newChannel)
        {
            if (String.IsNullOrEmpty(newChannel))
            {
                return installerUrl.ToString();
            }
            return installerUrl.ToString() + " " + newChannel;
        }
    }
}
