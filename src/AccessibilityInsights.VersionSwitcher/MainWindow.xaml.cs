﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using System;
using System.ComponentModel;
using System.Threading;
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
        bool _allowClosing;

        /// <summary>
        /// The entry point for our code
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Show();

            // Don't block the UI thread or else our progress bar won't update
            Thread t = new Thread(new ThreadStart(SwitchVersionAndInvokeCloseApplication));
            t.Start();
        }

        private void SwitchVersionAndInvokeCloseApplication()
        {
            string errorMessage = null;

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
                errorMessage = e.Message;
            }
#pragma warning restore CA1031 // Do not catch general exception types

            Dispatcher.Invoke(() => CloseAppliction(errorMessage));
        }

        private void CloseAppliction(string errorMessage)
        {
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, Properties.Resources.InstallError);
            }
            _allowClosing = true;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e != null)
            {
                base.OnClosing(e);
                e.Cancel = !_allowClosing;
            }
        }
    }
}
