// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;
using System.Linq;

namespace AccessibilityInsights.Extensions.AutoUpdate
{
    /// <summary>
    /// Meta info class
    /// </summary>
    public class MetaMSIInfo
    {
        /// <summary>
        /// if LatestMSI is set with this value, we picks latest folder automatically.
        /// </summary>
        public const string PickLatestBuild = "@LATEST";

        /// <summary>
        /// Microsoft.AccessibilityInsights MSI name
        /// </summary>
        public const string MSIPackageName = "AccessibilityInsights.msi";

        /// <summary>
        /// LatestMSI is a path to the update msi file
        /// Version refers to the MSI productversion field
        /// Release notes should link to the web
        /// </summary>
        public string LatestMSI { get; set; }
        public string ReleaseNotes { get; set; }
        public string Version { get; set; }
        public string MinimumVersion { get; set; }

        /// <summary>
        /// Get MSI Path
        /// it understands macro for latest build and translate it to right path. 
        /// </summary>
        /// <param name="root">root path</param>
        public string GetMSIPathSafely(string root)
        {
            string path = this.LatestMSI;

            if (path.ToUpperInvariant() == PickLatestBuild)
            {
                var rdi = new DirectoryInfo(root);

                path = (from di in rdi.GetDirectories()
                        let fis = di.GetFiles(MSIPackageName)
                        where fis.Length == 1
                        orderby di.LastWriteTime descending
                        select fis.First().FullName).First();
            }

            return Path.Combine(root, path);
        }
    }
}
