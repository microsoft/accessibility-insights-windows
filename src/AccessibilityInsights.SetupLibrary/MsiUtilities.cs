// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Methods to help with getting MSI information
    /// </summary>
    public static class MsiUtilities
    {
        private const string UpdateGuid = "{0D760959-F713-46C4-9A3D-4E73619EE3B5}";

        /// <summary>
        /// Returns the product version of the currently installed
        ///     Microsoft.AccessibilityInsights application
        /// Returns null if the version could not be found
        ///     - could occur if the application has not been installed at all
        ///     - could occur if Windows Installer's cached MSI file is corrupted or deleted
        /// </summary>
        /// <returns></returns>
        public static string GetInstalledProductVersion(IExceptionReporter exceptionReporter)
        {
            if (exceptionReporter == null)
                throw new ArgumentNullException(nameof(exceptionReporter));

            string targetUpgradeCode = UpdateGuid.ToUpperInvariant();

            // Check whether application with target upgrade code is installed on this machine
            IEnumerable<ProductInstallation> installations = ProductInstallation.GetRelatedProducts(targetUpgradeCode);
            bool existingApp;
            try
            {
                existingApp = installations.Any();
            }
            catch (ArgumentException e)
            {
                exceptionReporter.ReportException(e);
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
        /// Get the installed app path, based on the registry open file verb
        ///
        /// This verb is a string in the following format:
        /// "C:\Program Files (x86)\AccessibilityInsights\1.1\AccessibilityInsights.exe" "%1"
        ///
        /// We want the following as our output:
        /// C:\Program Files (x86)\AccessibilityInsights\1.1\AccessibilityInsights.exe
        /// </summary>
        /// <returns>The installed app path</returns>
        public static string GetAppInstalledPath()
        {
            try
            {
                RegistryKey commandKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Classes\A11y.Test\shell\open\command");
                {
                    string command = (string)commandKey.GetValue("");
                    return command.Substring(0, command.Length - 5).Replace("\"", "");
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new InvalidOperationException("Unable to locate Accessibility Insights for Windows", e);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Get the Uri to the release notes, based on the installed version
        /// </summary>
        /// <param name="exceptionReporter">Allows exceptions to get tracked</param>
        /// <returns>The Uri if available, null if not</returns>
        public static Uri GetReleaseNotesUri(IExceptionReporter exceptionReporter)
        {
            string version = GetInstalledProductVersion(exceptionReporter);
            if (!string.IsNullOrEmpty(version))
            {
                return new Uri(string.Format(CultureInfo.InvariantCulture,
                    "https://github.com/Microsoft/accessibility-insights-windows/releases/tag/v{0}",
                    version));
            }

            return null;
        }

        /// <summary>
        /// Returns the product version of the file at the given path
        /// </summary>
        /// <param name="pathToMSI">The path to the MSI being queried</param>
        /// <returns>The product version in string format</returns>
        internal static string GetMSIProductVersion(string pathToMSI)
        {
            using (Database db = new Database(pathToMSI, DatabaseOpenMode.ReadOnly))
            {
                return db.ExecutePropertyQuery("ProductVersion");
            }
        }
    }
}
