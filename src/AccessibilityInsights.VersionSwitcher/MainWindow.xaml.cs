// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Automation.Peers;

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
            UpdateProgress(0);
            Show();

            // Don't block the UI thread or else our progress bar won't update
            Thread t = new Thread(SwitchVersionAndInvokeCloseApplication);
            t.Start();
        }

        public void SwitchVersionAndInvokeCloseApplication()
        {
            string errorMessage = null;

            try
            {
                InstallationEngine engine = new InstallationEngine(ProductName, SafelyGetAppInstalledPath());
                engine.PerformInstallation(DispatcherUpdateProgress);
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

        private void UpdateProgress(int percentage)
        {
            string newText = $"Update {percentage}% complete";

            if (newText != statusText.Text)
            {
                progressBar.Value = percentage;
                statusText.Text = newText;
                var peer = FrameworkElementAutomationPeer.FromElement(statusText);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
                }
            }
        }

        private void DispatcherUpdateProgress(int percentage)
        {
            Dispatcher.Invoke(() => UpdateProgress(percentage));
            Thread.Sleep(TimeSpan.FromTicks(1)); // Yield momentarily to allow UI to update
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
