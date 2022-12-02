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
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Class to hold helper methods for installation
    /// </summary>
    internal class InstallationEngine
    {
        const string AppManifestFile = "AccessibilityInsights.exe.manifest";
        const string EnabledManifestFile = "UIAccess_Enabled.manifest";

        private readonly string _productName;
        private readonly string _appToLaunchAfterInstall;
        private readonly ExecutionHistory _history;
        private readonly Func<string[]> _commandLineProvider;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="productName">The localized product name</param>
        /// <param name="appToLaunchAfterInstall">The path to the app to launch after install completes</param>
        /// <param name="history">Holder for history data</param>
        /// <param name="commandLineProvider">Provider of command line arguments (production uses null)</param>
        internal InstallationEngine(string productName, string appToLaunchAfterInstall, ExecutionHistory history, Func<string[]> commandLineProvider = null)
        {
            _productName = productName;
            _appToLaunchAfterInstall = appToLaunchAfterInstall;
            _history = history;
            _commandLineProvider = commandLineProvider ?? Environment.GetCommandLineArgs;
        }

        /// <summary>
        /// Triggers the installation and launches the installed app upon completion
        /// </summary>
        internal ExecutionResult PerformInstallation(Action<int> progressCallback)
        {
            ExecutionResult result;

            _history.AddLocalDetail("Beginning Installation");
            progressCallback(0);
            InstallationOptions options = GetInstallationOptions();
            SetInitialTelemetryValues(options);
            DownloadFromUriToLocalFile(options, (p) => progressCallback((p * 4) / 5));
            using (ValidateLocalFile(options))
            using (Transaction transaction = new Transaction(_productName, TransactionAttributes.ChainEmbeddedUI))
            {
                result = InstallWithinTransaction(options, transaction);
            }
            progressCallback(80);
            UpdateConfigWithNewChannel(options.NewChannel);
            progressCallback(90);
            SetManifestForUIAccess(options.EnableUIAccess);
            progressCallback(95);
            LaunchPostInstallApp();
            progressCallback(100);
            _history.AddLocalDetail("Completed Installation");

            return result;
        }

        /// <summary>
        /// Create an InstallationOptions object based on available input
        /// </summary>
        /// <returns>The populated InstallationOptions object</returns>
        internal InstallationOptions GetInstallationOptions()
        {
            string[] args = _commandLineProvider();
            return ResultExecutionWrapper.Execute(ExecutionResult.ErrorBadCommandLine,
                () => string.Format(Properties.Resources.BadCommandLineMessageFormat, string.Join(" ", args)),
                () =>
                {
                    string newChannel = null;

                    // args[0] is the app name and gets ignored
                    Uri msiPath = new Uri(args[1]);
                    int msiSizeInBytes = int.Parse(args[2]);
                    string msiSha512 = args[3];

                    if (args.Length > 4)
                    {
                        newChannel = args[4];
                    }

                    bool enableUIAccess = IsUIAccessEnabled();

                    _history.AddLocalDetail("Option: MSI Path = {0}", msiPath);
                    _history.AddLocalDetail("Option: MSI expected size = {0} bytes", msiSizeInBytes);
                    _history.AddLocalDetail("Option: MSI expected SHA512 = {0}", msiSha512);
                    _history.AddLocalDetail("Option: Enable UIAccess = {0}", enableUIAccess);
                    if (newChannel != null)
                    {
                        _history.AddLocalDetail("Option: New channel = {0}", newChannel);
                    }
                    return new InstallationOptions(msiPath, msiSizeInBytes, (msiSha512 == "none") ? null : msiSha512, newChannel, enableUIAccess);
                });
        }

        internal void SetInitialTelemetryValues(InstallationOptions options)
        {
            _history.StartingVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            _history.RequestedMsi = options.MsiPath.ToString();
            _history.ExpectedMsiSizeInBytes = options.MsiSizeInBytes;
            _history.ExpectedMsiSha512 = options.MsiSha512;
            _history.NewChannel = options.NewChannel;
        }

        /// <summary>
        /// Infer UIAccess state from the existing manifest files. Assume false unless proven otherwise
        /// </summary>
        private bool IsUIAccessEnabled()
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
                _history.AddLocalDetail("Unable to determine UIAccess status. Assuming disabled. Detail: {0}", e.ToString());
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Update manifest files as needed for UIAccess support
        /// </summary>
        /// <param name="enableUIAccess">Desired state of UIAccess.</param>
        private void SetManifestForUIAccess(bool enableUIAccess)
        {
            if (!enableUIAccess)
                return;   // UIAccess is disabled by default

            try
            {
                string appPath = Path.GetDirectoryName(MsiUtilities.GetAppInstalledPath());
                string enabledManifestContents = File.ReadAllText(Path.Combine(appPath, EnabledManifestFile));
                File.WriteAllText(Path.Combine(appPath, AppManifestFile), enabledManifestContents, System.Text.Encoding.UTF8);
                _history.AddLocalDetail("UIAccess has been enabled");
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                // Report the error, assume no UIAccess
                _history.AddLocalDetail("Unable to enable UIAccess. Detail: {0}", e.ToString());
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
            ResultExecutionWrapper.Execute(ExecutionResult.ErrorMsiDownloadFailed,
                () => Properties.Resources.UnableToDownloadInstallerFile, 
                () =>
                {
                    using (Stream stream = new FileStream(options.LocalInstallerFile, FileMode.CreateNew))
                    {
                        StreamMetadata streamMetaData = GitHubClient.LoadUriContentsIntoStream(options.MsiPath, stream, options.DownloadTimeout, progressCallback);
                        _history.ResolvedMsi = streamMetaData.ResponseUri.ToString();
                    } // using

                    _history.AddLocalDetail("Successfully downloaded Installer from {0} to {1}",
                        options.MsiPath.ToString(), options.LocalInstallerFile);

                    // Dummy return value to make wrapper work
                    return 0;
                });
        }

        /// <summary>
        /// Validate a local file for proper signing
        /// </summary>
        /// <param name="options">The InstallationOptions object that tells us about the MSI file</param>
        /// <returns>An initialized TrustVerifier object</returns>
        private TrustVerifier ValidateLocalFile(InstallationOptions options)
        {
            using (Stream lockingStream = new FileStream(options.LocalInstallerFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ValidateFileProperties(options.LocalInstallerFile, options.MsiSizeInBytes, options.MsiSha512);

                TrustVerifier verifier = new TrustVerifier(options.LocalInstallerFile);
                if (!verifier.IsVerified)
                {
                    throw new ResultBearingException(ExecutionResult.ErrorMsiBadSignature, Properties.Resources.InstallerFileIsNotTrusted);
                }

                _history.AddLocalDetail("Successfully validated local file: {0}", options.LocalInstallerFile);

                return verifier;
            }
        }

        internal void ValidateFileProperties(string filePath, int expectedFileSize, string expectedFileSha512)
        {
            // Save values to telemetry before performing any vaidation
            _history.ActualMsiSizeInBytes = (int)new FileInfo(filePath).Length;
            _history.ActualMsiSha512 = ComputeSha512(filePath);

            if (expectedFileSize != 0 && expectedFileSize != _history.ActualMsiSizeInBytes)
            {
                throw new ResultBearingException(ExecutionResult.ErrorMsiSizeMismatch, Properties.Resources.InstallerFileIsWrongSize);
            }

            if (expectedFileSha512 != null && !expectedFileSha512.Equals(_history.ActualMsiSha512, StringComparison.OrdinalIgnoreCase))
            {
                throw new ResultBearingException(ExecutionResult.ErrorMsiSha512Mismatch, Properties.Resources.InstallerFileHasWrongHash);
            }
        }

        /// <summary>
        /// Compute the SHA512 value for the contents of the specified file
        /// </summary>
        /// <param name="filePath">The file to validate. The code assumes that the file exists</param>
        /// <returns>The SHA512 as a string of hex digits</returns>
        internal static string ComputeSha512(string filePath)
        {
            using (SHA512 sha = SHA512.Create())
            using (Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                StringBuilder sb = new StringBuilder();
                byte[] bytes = sha.ComputeHash(stream);

                foreach (byte b in bytes)
                {
                    sb.AppendFormat("{0:X2}", b);
                }

                return sb.ToString();
            }
        }

        /// <summary>
        /// Wrap the install inside a transaction
        /// </summary>
        /// <param name="options">The parameters to drive the installation</param>
        /// <param name="transaction">The transaction to wrap the operation</param>
        private ExecutionResult InstallWithinTransaction(InstallationOptions options, Transaction transaction)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            ExecutionResult result = ExecutionResult.Unknown;
            try
            {
                result = ResultExecutionWrapper.Execute<ExecutionResult>(
                    ExecutionResult.ErrorInstallingMsi,
                    () => Properties.Resources.UnableToCompleteInstallation,
                    () =>
                    {
                        RemoveOldVersion();
                        InstallNewVersion(options.LocalInstallerFile);
                        return ExecutionResult.Success;
                    });
            }
            finally
            {
                string statusMessage;
                if (result == ExecutionResult.Success)
                {
                    transaction.Commit();
                    statusMessage = "succeeded";
                }
                else
                {
                    transaction.Rollback();
                    statusMessage = "failed";
                }

                stopwatch.Stop();

                const string messageTemplate = "VersionSwitcher {0} in {1} ms";
                _history.AddLocalDetail(messageTemplate, statusMessage, stopwatch.ElapsedMilliseconds);
            }

            return result;
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
                _history.AddLocalDetail("No application to launch");
            }
            else
            {
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.GetEnvironmentVariable("windir"), "explorer.exe"),
                    Arguments = _appToLaunchAfterInstall
                };
                if (Process.Start(start) != null)
                {
                    _history.AddLocalDetail("Successfully started process: {0}", _appToLaunchAfterInstall);
                }
                else
                {
                    _history.AddLocalDetail("Unable to start process: {0}", _appToLaunchAfterInstall);
                }
            }
        }

        /// <summary>
        /// Install from a local MSI file
        /// </summary>
        /// <param name="msiPath">full name to the MSI</param>
        internal void InstallNewVersion(string msiPath)
        {
            _history.AddLocalDetail("Attempting to install from \"{0}\"", msiPath);
            Stopwatch stopwatch = Stopwatch.StartNew();
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.InstallProduct(msiPath, "");
            stopwatch.Stop();
            _history.AddLocalDetail("Installed {0} in {1} milliseconds", msiPath, stopwatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Given the product name, find it in the product database and silently remove it
        /// </summary>
        internal void RemoveOldVersion()
        {
            Exception exception = null;
            string productId = null;
            _history.AddLocalDetail("Attempting to find product: \"{0}\"", _productName);
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
                _history.AddLocalDetail("Unable to locate product {0}! Continuing without uninstall",
                    _productName);
                return;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            Installer.SetInternalUI(InstallUIOptions.Silent);
            Installer.ConfigureProduct(productId, 0, InstallState.Absent, "");
            stopwatch.Stop();
            _history.AddLocalDetail("Removed productId: {0} in {1} milliseconds",
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
