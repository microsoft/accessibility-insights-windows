// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SetupLibrary.REST;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Class to hold helper methods for installation
    /// </summary>
    internal class InstallationEngine
    {
        private readonly Stopwatch _installerDownloadStopwatch = new Stopwatch();
        private readonly string _productName;
        private readonly string _appToLaunchAfterInstall;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="productName">The localized product name</param>
        /// <param name="appToLaunchAfterInstall">The path to the app to launch after install completes</param>
        internal InstallationEngine(string productName, string appToLaunchAfterInstall)
        {
            _productName = productName;
            _appToLaunchAfterInstall = appToLaunchAfterInstall;
        }

        /// <summary>
        /// Triggers the installation & launches the installed app upon completion
        /// </summary>
        internal void PerformInstallation()
        {
            InstallationOptions options = GetInstallationOptions();
            DownloadFromUriToLocalFile(options);
            using (ValidateLocalFile(options.LocalInstallerFile))
            {
                using (Transaction transaction = new Transaction("Accessibility Insights for Windows", TransactionAttributes.ChainEmbeddedUI))
                {
                    InstallWithinTransaction(options, transaction);
                }
            }
            UpdateConfigWithNewChannel(options.NewChannel);
            LaunchPostInstallApp();
        }

        /// <summary>
        /// Create a CommandLineParameters object based on available input
        /// </summary>
        /// <returns>The populated InstallationOptions object</returns>
        private static InstallationOptions GetInstallationOptions()
        {
            // Temporary implementation for testing--still need to finalize actual command line
            string[] args = Environment.GetCommandLineArgs();

            string msiPath = null;
            string newChannel = null;

            if (args.Length > 1)
            {
                msiPath = args[1];
                if (args.Length > 2)
                {
                    newChannel = args[2];
                }

                return new InstallationOptions(msiPath, newChannel);
            }

            string input = string.Join(" | ", args);
            throw new ArgumentException("Invalid Input: " + input);
        }

        /// <summary>
        /// Download the installer to a computed local file
        /// </summary>
        /// <param name="options">The parameters to drive the install</param>
        private void DownloadFromUriToLocalFile(InstallationOptions options)
        {
            _installerDownloadStopwatch.Reset();

            using (Stream stream = new FileStream(options.LocalInstallerFile, FileMode.CreateNew))
            {
                _installerDownloadStopwatch.Start();

                try
                {
                    GitHubClient.LoadUriContentsIntoStream(options.MsiPath, stream, options.DownloadTimeout);
                }
                finally
                {
                    _installerDownloadStopwatch.Stop();
                }
            } // using
        }

        /// <summary>
        /// Validate a local file for proper signing
        /// </summary>
        /// <param name="localFile">The full path to the local file</param>
        /// <returns>An initialized TrustVerifier object</returns>
        private static TrustVerifier ValidateLocalFile(string localFile)
        {
            TrustVerifier verifier = new TrustVerifier(localFile);
            if (!verifier.IsVerified)
            {
                // TODO : Better error messaging
                throw new ArgumentException("Untrusted file!", nameof(localFile));
            }

            return verifier;
        }

        /// <summary>
        /// Wrap the install inside a transaction
        /// </summary>
        /// <param name="options">The parameters to drive the installation</param>
        /// <param name="transaction">The transaction to wrap the operation</param>
        private void InstallWithinTransaction(InstallationOptions options, Transaction transaction)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            bool success = false;
            try
            {
                DeleteOldVersion();
                InstallNewVersion(options.LocalInstallerFile);
                success = true;
            }
            finally
            {
                if (success)
                    transaction.Commit();
                else
                    transaction.Rollback();

                stopwatch.Stop();
                string message = string.Format(CultureInfo.InvariantCulture, "VersionSwitcher {0} in {1} ms",
                    success ? "succeeded" : "failed", stopwatch.ElapsedMilliseconds);
                Trace.WriteLine(message);
            }
        }

        /// <summary>
        /// Update the config file to reflect the new channel setting
        /// </summary>
        /// <param name="newRing"></param>
        private static void UpdateConfigWithNewChannel(string newRing)
        {
            if (newRing != null)
            {
                // TODO : Update local config file
            }
        }

        /// <summary>
        /// Locate the MSI-installed app and launch it without elevated privileges
        /// </summary>
        private void LaunchPostInstallApp()
        {
            if (_appToLaunchAfterInstall != null)
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe");
                start.Arguments = _appToLaunchAfterInstall;
                Process.Start(start);
            }
        }

        /// <summary>
        /// Install from a local MSI file
        /// </summary>
        /// <param name="msiPath">full name to the MSI</param>
        internal static void InstallNewVersion(string msiPath)
        {
            Trace.TraceInformation("Attempting to install from \"{0}\"", msiPath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.InstallProduct(msiPath, "");
            stopwatch.Stop();
            Trace.TraceInformation("Installed {0} in {1} milliseconds", msiPath, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Given the product name, find it in the product database and silently remove it
        /// </summary>
        internal void DeleteOldVersion()
        {
            Trace.TraceInformation("Attempting to find product: \"{0}\"", _productName);
            Stopwatch stopwatch = Stopwatch.StartNew();
            string productId = FindInstalledProductKey(_productName).ToString("B", CultureInfo.InvariantCulture);
            stopwatch.Stop();

            Trace.TraceInformation("Found productId: {0} in {1} milliseconds", productId, stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.ConfigureProduct(productId, 0, InstallState.Absent, "");
            stopwatch.Start();
            Trace.TraceInformation("Deleted productId: {0} in {1} milliseconds", productId, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Convert a product name to the install GUID
        /// </summary>
        /// <param name="productName">The localized display string</param>
        /// <returns>The Guid associated with this product</returns>
        private static Guid FindInstalledProductKey(string productName)
        {
            RegistryKey productsKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            {
                foreach (string keyName in productsKey.GetSubKeyNames())
                {
                    RegistryKey subKey = productsKey.OpenSubKey(keyName);
                    string registryProductName = (string)subKey.GetValue("DisplayName", string.Empty);

                    if (registryProductName.Equals(productName, StringComparison.OrdinalIgnoreCase))
                    {
                        return new Guid(keyName);
                    }
                }
            }

            throw new ArgumentException("Unable to locate key for product: " + productName, nameof(productName));
        }
    }
}
