using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;


namespace AccessibilityInsights.Extensions
{
    public static class VSAHandler
    {
        private static void RemoveVSAFromTempFolder()
        {
            try
            {
                Directory.Delete(GetAppFolderInTempFolder(), true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("AccessibilityInsights - exception when removing VSA: " + ex.ToString());
            }
        }

        private static bool TryCopyVSAToTempFolder()
        {
            return TryCopyFilesRecursively(GetAppInstallationPath(), Path.GetTempPath() + "VersionSwitcher");
        }

        private static bool TryCopyFilesRecursively(string sourcePath, string targetPath)
        {
            try
            {
                if (Directory.Exists(sourcePath))
                {
                    if (!Directory.Exists(targetPath))
                    {
                        Directory.CreateDirectory(targetPath);
                    }
                    // copy files
                    foreach (string file in Directory.GetFiles(sourcePath))
                    {
                        FileInfo fileInfo = new FileInfo(file);
                        fileInfo.CopyTo(string.Format(CultureInfo.CurrentCulture, @"{0}\{1}", targetPath, fileInfo.Name), true);
                    }
                    // copy folders
                    foreach (string dir in Directory.GetDirectories(sourcePath))
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(dir);
                        if (!TryCopyFilesRecursively(dir, string.Format(CultureInfo.CurrentCulture, @"{0}\{1}", targetPath, directoryInfo.Name)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("AccessibilityInsights - exception when copying VSA: " + ex.ToString());
                return false;
            }
        }

        private static string GetAppInstallationPath()
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

        private static string GetAppFolderInTempFolder()
        {
            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath, "VersionSwitcher");
        }

        private static string GetAppPathInTempFolder()
        {
            return Path.Combine(GetAppFolderInTempFolder(), "AccessibilityInsights.VersionSwitcher.exe");
        }

        private static string GetAppArguments(Uri installerUrl, string targetRing)
        {
            if (String.IsNullOrEmpty(targetRing))
            {
                return installerUrl.ToString();
            }
            return installerUrl.ToString() + " " + targetRing;
        }

        public static UpdateResult Run(Uri installerUrl)
        {
            VSAHandler.RemoveVSAFromTempFolder();
            if (!VSAHandler.TryCopyVSAToTempFolder())
            {
                return UpdateResult.Unknown;
            }
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = VSAHandler.GetAppPathInTempFolder();
            start.Arguments = VSAHandler.GetAppArguments(installerUrl, "default");
            System.Diagnostics.Process.Start(start);
            return UpdateResult.Success;
        }
    }
}
