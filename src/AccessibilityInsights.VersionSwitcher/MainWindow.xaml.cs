// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Stopwatch _installerDownloadStopwatch = new Stopwatch();
        const string ProductName = "Accessibility Insights For Windows v1.1";
        private Uri _installerUri = null;

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                Hide();

                CommandLineParameters parameters = GetCommandLineParameters();
                string localFile = DownloadFromUriToLocalFile(parameters);
                ValidateLocalFile(localFile);
                InstallHelper.DeleteOldVersion(ProductName);
                InstallHelper.InstallNewVersion(parameters.MsiPath);
                UpdateConfigWithNewRing(parameters.NewRing);
                LaunchInstalledApp();
            }
            catch(Exception e)
            {
                Trace.TraceError(e.ToString());
                MessageBox.Show(e.Message, "An error occurred during install");
            }

            Close();
        }

        private static CommandLineParameters GetCommandLineParameters()
        {
            // Temporary implementation for testing--still need to finalize actual command line
            string[] args = Environment.GetCommandLineArgs();

            string msiPath = null;
            string newRing = null;

            if (args.Length > 1)
            {
                if (File.Exists(args[1]))
                {
                    msiPath = args[1];
                }
                else
                {
                    Trace.TraceError("Invalid Path: " + args[1]);
                }

                if (args.Length > 2)
                {
                    newRing = args[2];
                }

                return new CommandLineParameters(msiPath, newRing);
            }

            string input = string.Join(" | ", args);
            throw new ArgumentException("Invalid Input: " + input);
        }

        private bool TryDownloadInstaller(string targetFilePath, TimeSpan timeout)
        {
            _installerDownloadStopwatch.Reset();

            using (Stream stream = new FileStream(targetFilePath, FileMode.CreateNew))
            {
                _installerDownloadStopwatch.Start();

                try
                {
                    return GitHubHelper.TryGet(_installerUri, stream, timeout);
                }
                catch (Exception e)
                {
                    e.ReportException();
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                }
                finally
                {
                    _installerDownloadStopwatch.Stop();
                }
            } // using

            return false;
        }

        private string DownloadFromUriToLocalFile(CommandLineParameters parameters)
        {
            // TODO - copy from MSI, throw exception on error
            string tempFile = Path.ChangeExtension(Path.GetTempFileName(), "msi");
            if (!TryDownloadInstaller(tempFile, TimeSpan.FromSeconds(60)))
            {
                throw new Exception("Unable to download installer");
            }
            parameters.MsiPath = tempFile;

            // Return value is where the local file was written
            return parameters.MsiPath;
        }

        private void ValidateLocalFile(string localFile)
        {
            using (TrustVerifier verifier = new TrustVerifier(localFile))
            {
                if (!verifier.IsVerified)
                {
                    // TODO : Better error messaging
                    //System.Diagnostics.Trace.WriteLine("AccessibilityInsights upgrade - exception when converting the config data: ");
                    throw new ArgumentException("Untrusted file!", nameof(localFile));
                }
            }
        }

        private void UpdateConfigWithNewRing(string newRing)
        {
            if (newRing != null)
            {
                // TODO : Update local config file
            }
        }

        private void LaunchInstalledApp()
        {
            // TODO : Launch the app
        }
    }
}
