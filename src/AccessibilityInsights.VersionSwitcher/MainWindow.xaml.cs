// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using System;
using System.Windows;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly static IExceptionReporter ExceptionReporter = new ExceptionReporter();
        const string ProductName = "Accessibility Insights For Windows v1.1";

        /// <summary>
        /// The entry point for our code
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Hide();

            try
            {
                InstallationEngine engine = new InstallationEngine(ProductName, SafelyGetAppInstalledPath());
                engine.PerformInstallation();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                EventLogger.WriteErrorMessage(e.ToString());
                ExceptionReporter.ReportException(e);
                MessageBox.Show(e.Message, Properties.Resources.InstallError);
            }
#pragma warning restore CA1031 // Do not catch general exception types

            Close();
        }

        private static string SafelyGetAppInstalledPath()
        {
            try
            {
                return MsiUtilities.GetAppInstalledPath();
            }
            catch (InvalidOperationException e)
            {
                ExceptionReporter.ReportException(e);
                return null;
            }
        }
    }
}
