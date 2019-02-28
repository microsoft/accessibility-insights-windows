using System;
using System.Globalization;
using System.IO;


namespace AccessibilityInsights.Extensions
{
    public static class VSAHandler
    {
        public static void RemoveVSAFromTempFolder()
        {
            try
            {
                Directory.Delete(Path.GetTempPath() + "AccessibilityInsights.VersionSwitcher", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine("AccessibilityInsights - exception when removing VSA: " + ex.ToString());
            }
        }

        public static bool TryCopyVSAToTempFolder()
        {
            return TryCopyFilesRecursively(GetAppInstallationPath(), Path.GetTempPath() + "AccessibilityInsights.VersionSwitcher");
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

        /* we hard code this for now since our current thinking is that 
         * we'll have each ring in a separate folder, so we'll have Control/Insiders for Insiders,
         * Control/Production for production, etc. Part of this is to simplify the handling of release notes */
        public static string GetLocationOfInstaller()
        {
            return "https://github.com/babylonbee/pipeline_upgrade_spike/releases/download/v1.1.0968/cdburner_xp_setup.msi";
        }

        public static string GetAppInstallationPath()
        {
            return "C:\\Users\\biwu\\githome\\accessibility-insights-windows\\src\\AccessibilityInsights.VersionSwitcher";
        }

        public static string GetAppPathInTempFolder()
        {
            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath + "AccessibilityInsights.VersionSwitcher\\bin\\Debug\\", "AccessibilityInsights.VersionSwitcher.exe");
        }

        public static string GetAppArguments(string targetRing)
        {
            if (String.IsNullOrEmpty(targetRing))
            {
                return GetLocationOfInstaller();
            }
            return GetLocationOfInstaller() + " " + targetRing;
        }
    }
}
