// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
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
            Stopwatch stopwatch = Stopwatch.StartNew();
            ExecutionHistory history = new ExecutionHistory();
            string errorMessage = null;

            try
            {
                history.TypedExecutionResult = ResultExecutionWrapper.Execute(ExecutionResult.Unknown, 
                    () => Properties.Resources.UnableToCompleteInstallation,
                    () =>
                    {
                        InstallationEngine engine = new InstallationEngine(ProductName, SafelyGetAppInstalledPath(history), history);
                        return engine.PerformInstallation(DispatcherUpdateProgress);
                    });
            }
            catch (ResultBearingException e)
            {
                history.TypedExecutionResult = e.Result;
                history.AddLocalDetail(e.ToString());
                errorMessage = e.Message;
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    history.ExecutionTimeInMilliseconds = stopwatch.ElapsedMilliseconds;
                    string path = ExecutionHistory.GetDataFilePath();
                    FileHelpers.SerializeDataToJSON(history, path);
                });
            }

            Dispatcher.Invoke(() => CloseAppliction(errorMessage));
        }

        private void UpdateProgress(int percentage)
        {
            string newText = string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.ProgressIndicatorFormat, percentage);

            if (newText != statusText.Text)
            {
                progressBar.Value = percentage;
                statusText.Text = newText;
                var peer = FrameworkElementAutomationPeer.FromElement(statusText);
                peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
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

        private static string SafelyGetAppInstalledPath(ExecutionHistory history)
        {
            try
            {
                return MsiUtilities.GetAppInstalledPath();
            }
            catch (InvalidOperationException e)
            {
                history.AddLocalDetail(e.ToString());
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
