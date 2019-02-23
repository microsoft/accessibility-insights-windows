using System;
using System.Globalization;
using System.IO;


namespace AccessibilityInsights.Extensions
{
    class VSAHandler
    {
        private readonly string appName = "AccessibilityInsights.VersionSwitcher.exe";

        public static void RemoveVSAFrom(string vsaPath)
        {
            Directory.Delete(vsaPath, true);
        }

        public bool TryCopyFilesRecursively(string sourcePath, string targetPath)
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
        public static string GetLocationOfInstaller(string version)
        {
            return "https://github.com/Microsoft/accessibility-insights-windows/releases/download/" + version + "/AccessibilityInsights.msi";
        }

        public string GeneratorCommandLine(string version, string targetRing)
        {
            if (String.IsNullOrEmpty(targetRing))
            {
                return string.Format(CultureInfo.CurrentCulture, @"{0} {1}", appName, GetLocationOfInstaller(version));
            }
            return string.Format(CultureInfo.CurrentCulture, @"{0} {1} {2}", appName, GetLocationOfInstaller(version), targetRing);
        }
    }
}
