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
        const string AppManifestFile = "AccessibilityInsights.exe.manifest";
        const string EnabledManifestFile = "UIAccess_Enabled.manifest";

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
        /// Triggers the installation and launches the installed app upon completion
        /// </summary>
        internal void PerformInstallation(Action<int> progressCallback)
        {
            EventLogger.WriteInformationalMessage("Beginning Installation");
            progressCallback(0);
            InstallationOptions options = GetInstallationOptions();
            DownloadFromUriToLocalFile(options, (p) => progressCallback((p * 4) / 5));
            using (ValidateLocalFile(options.LocalInstallerFile))
            {
                using (Transaction transaction = new Transaction(_productName, TransactionAttributes.ChainEmbeddedUI))
                {
                    InstallWithinTransaction(options, transaction);
                }
            }
            progressCallback(80);
            UpdateConfigWithNewChannel(options.NewChannel);
            progressCallback(90);
            SetManifestForUIAccess(options.EnableUIAccess);
            progressCallback(95);
            LaunchPostInstallApp();
            progressCallback(100);
            EventLogger.WriteInformationalMessage("Completed Installation");
        }

        /// <summary>
        /// Create an InstallationOptions object based on available input
        /// </summary>
        /// <returns>The populated InstallationOptions object</returns>
        private static InstallationOptions GetInstallationOptions()
        {
            string[] args = Environment.GetCommandLineArgs();
            string newChannel = null;

            if (args.Length > 1)
            {
                string msiPath = args[1];

                if (args.Length > 2)
                {
                    newChannel = args[2];
                }

                bool enableUIAccess = IsUIAccessEnabled();

                EventLogger.WriteInformationalMessage("Options:\nMSI Path = {0}\nNew Channel = {1},\nEnable UIAccess = {2}",
                    msiPath, newChannel, enableUIAccess);
                return new InstallationOptions(msiPath, newChannel, enableUIAccess);
            }

            string input = string.Join(" | ", args);
            throw new ArgumentException("Invalid Input: " + input);
        }

        /// <summary>
        /// Infer UIAccess state from the existing manifest files. Assume false unless proven otherwise
        /// </summary>
        private static bool IsUIAccessEnabled()
        {
            try
            {
                string appPath = Path.GetDirectoryName(MsiUtilities.GetAppInstalledPath());
                string appManifestContents = File.ReadAllText(Path.Combine(appPath, AppManifestFile));
                string enabledManifestContents = File.ReadAllText(Path.Combine(appPath, EnabledManifestFile));
                return appManifestContents == enabledManifestContents;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                // Report the error, assume no UIAccess
                EventLogger.WriteWarningMessage("Unable to determine UIAccess status. Assuming disabled. Detail: {0}", e.ToString());
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Update manifest files as needed for UIAccess support
        /// </summary>
        /// <param name="enableUIAccess">Desired state of UIAccess.</param>
        private static void SetManifestForUIAccess(bool enableUIAccess)
        {
            if (!enableUIAccess)
                return;   // UIAccess is disabled by default

            try
            {
                string appPath = Path.GetDirectoryName(MsiUtilities.GetAppInstalledPath());
                string enabledManifestContents = File.ReadAllText(Path.Combine(appPath, EnabledManifestFile));
                File.WriteAllText(Path.Combine(appPath, AppManifestFile), enabledManifestContents, System.Text.Encoding.UTF8);
                EventLogger.WriteInformationalMessage("UIAccess has been enabled");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                // Report the error, assume no UIAccess
                EventLogger.WriteWarningMessage("Unable to enable UIAccess. Detail: {0}", e.ToString());
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Download the installer to a computed local file
        /// </summary>
        /// <param name="options">The parameters to drive the install</param>
        /// <param name="progressCallback">Called to update download progress</param>
        private void DownloadFromUriToLocalFile(InstallationOptions options, Action<int> progressCallback)
        {
            _installerDownloadStopwatch.Reset();

            using (Stream stream = new FileStream(options.LocalInstallerFile, FileMode.CreateNew))
            {
                _installerDownloadStopwatch.Start();

                try
                {
                    GitHubClient.LoadUriContentsIntoStream(options.MsiPath, stream, options.DownloadTimeout, progressCallback);
                }
                finally
                {
                    _installerDownloadStopwatch.Stop();
                }
            } // using

            EventLogger.WriteInformationalMessage("Successfully downloaded Installer from {0} to {1}",
                options.MsiPath.ToString(), options.LocalInstallerFile);
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
                throw new ArgumentException(Properties.Resources.UntrustedFile, nameof(localFile));
            }

            EventLogger.WriteInformationalMessage("Successfully validated local file: {0}", localFile);

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
                RemoveOldVersion();
                InstallNewVersion(options.LocalInstallerFile);
                success = true;
            }
            finally
            {
                if (success)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }

                stopwatch.Stop();

                const string messageTemplate = "VersionSwitcher {0} in {1} ms";

                if (success)
                {
                    EventLogger.WriteInformationalMessage(messageTemplate, "succeeded", stopwatch.ElapsedMilliseconds);
                }
                else
                {
                    EventLogger.WriteErrorMessage(messageTemplate, "failed", stopwatch.ElapsedMilliseconds);
                }
            }
        }

        /// <summary>
        /// Update the config file to reflect the new channel setting
        /// </summary>
        /// <param name="newChannel">The new channel to use</param>
        private static void UpdateConfigWithNewChannel(string newChannel)
        {
            if (newChannel == null)
                return;

            var defaultConfigPaths = FixedConfigSettingsProvider.CreateDefaultSettingsProvider();
            var configFile = Path.Combine(defaultConfigPaths.ConfigurationFolderPath, Constants.AppConfigFileName);

            SettingsDictionary settings = FileHelpers.LoadDataFromJSON<SettingsDictionary>(configFile);
            settings[Constants.ReleaseChannelKey] = newChannel;
            FileHelpers.SerializeDataToJSON(settings, configFile);
        }

        /// <summary>
        /// Locate the MSI-installed app and launch it without elevated privileges
        /// </summary>
        private void LaunchPostInstallApp()
        {
            if (_appToLaunchAfterInstall == null)
            {
                EventLogger.WriteWarningMessage("No application to launch");
            }
            else
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe");
                start.Arguments = _appToLaunchAfterInstall;
                if (Process.Start(start) != null)
                {
                    EventLogger.WriteInformationalMessage("Successfully started process: {0}", _appToLaunchAfterInstall);
                }
                else
                {
                    EventLogger.WriteWarningMessage("Unable to start process: {0}", _appToLaunchAfterInstall);
                }
            }
        }

        /// <summary>
        /// Install from a local MSI file
        /// </summary>
        /// <param name="msiPath">full name to the MSI</param>
        internal static void InstallNewVersion(string msiPath)
        {
            EventLogger.WriteInformationalMessage("Attempting to install from \"{0}\"", msiPath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.InstallProduct(msiPath, "");
            stopwatch.Stop();
            EventLogger.WriteInformationalMessage("Installed {0} in {1} milliseconds", msiPath, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Given the product name, find it in the product database and silently remove it
        /// </summary>
        internal void RemoveOldVersion()
        {
            Exception exception = null;
            string productId = null;
            EventLogger.WriteInformationalMessage("Attempting to find product: \"{0}\"", _productName);
            try
            {
                productId = FindInstalledProductKey(_productName).ToString("B", CultureInfo.InvariantCulture);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                exception = e;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            if (exception != null)
            {
                EventLogger.WriteWarningMessage("Unable to locate product {0}! Continuing without uninstall",
                    _productName);
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.ConfigureProduct(productId, 0, InstallState.Absent, "");
            stopwatch.Stop();
            EventLogger.WriteInformationalMessage("Removed productId: {0} in {1} milliseconds",
                productId, stopwatch.ElapsedMilliseconds);
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
