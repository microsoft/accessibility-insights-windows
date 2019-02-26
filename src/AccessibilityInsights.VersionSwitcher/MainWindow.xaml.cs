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
        const string ProductName = "Accessibility Insights For Windows v1.1";

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

        private string DownloadFromUriToLocalFile(CommandLineParameters parameters)
        {
            // TODO - copy from MSI, throw exception on error
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
