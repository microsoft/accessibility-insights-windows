// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// Methods to help with updates
    /// </summary>
    internal static class UpdateMethods
    {
        private const string UpdateGuid = "{0D760959-F713-46C4-9A3D-4E73619EE3B5}";

        /// <summary>
        /// Starts a process to install the MSI file at the specified location
        ///     Method returns before installation is complete
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        internal static void BeginMSIInstall(string location)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = "/K timeout /t 5 & msiexec /i " + location + " /qb-";
            p.Start();
        }

        /// <summary>
        /// Returns the product version of the currently installed
        ///     Microsoft.AccessibilityInsights application
        /// Returns null if the version could not be found
        ///     - could occur if the application has not been installed at all
        ///     - could occur if Windows Installer's cached MSI file is corrupted or deleted
        /// </summary>
        /// <returns></returns>
        internal static string GetInstalledProductVersion()
        {
            string targetUpgradeCode = UpdateGuid.ToUpperInvariant();

            // Check whether application with target upgrade code is installed on this machine
            IEnumerable<ProductInstallation> installations = ProductInstallation.GetRelatedProducts(targetUpgradeCode);
            bool existingApp;
            try
            {
                existingApp = installations.Any<ProductInstallation>();
            }
            catch (ArgumentException e)
            {
                e.ReportException();
                // occurs when the upgrade code is formatted incorrectly
                // exception text: "Parameter is incorrect"
                return null;
            }
            if (!existingApp)
            {
                // occurs when the upgrade code does not match any existing application
                return null;
            }
            ProductInstallation existingInstall = installations.FirstOrDefault<ProductInstallation>(i => i.ProductVersion != null);
            if (existingInstall == null)
            {
                return null;
            }
            string msiFilePath = existingInstall.LocalPackage;
            if (msiFilePath != null)
            {
                return GetMSIProductVersion(msiFilePath);
            }

            // Should only get here if LocalPackage not set
            return null;
        }

        /// <summary>
        /// Returns the product version of the file at the given path
        /// </summary>
        /// <param name="pathToMSI"></param>
        /// <returns></returns>
        internal static string GetMSIProductVersion(string pathToMSI)
        {
            using (Database db = new Database(pathToMSI, DatabaseOpenMode.ReadOnly))
            {
                return db.ExecutePropertyQuery("ProductVersion");
            }
        }
    }
}
