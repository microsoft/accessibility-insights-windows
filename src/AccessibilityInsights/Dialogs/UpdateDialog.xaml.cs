// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Windows;

namespace AccessibilityInsights.Dialogs
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateDialog : Window
    {
        public Uri ReleaseNotesUri { get; }

        /// <summary>
        /// Initializes update dialog 
        /// </summary>
        /// <param name="releaseNotesUri">Uri to release notes</param>
        public UpdateDialog(Uri releaseNotesUri)
        {
            InitializeComponent();
            this.ReleaseNotesUri = releaseNotesUri;
            Topmost = App.Current.MainWindow.Topmost;
        }

        private void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void UpdateLater_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Navigates to the url for the release notes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReleaseNotes_Click(object sender, RoutedEventArgs e)
        {
            string error = string.Empty;
            string releaseNotesString = string.Empty;
            try
            {
                releaseNotesString = ReleaseNotesUri.ToString();

                if (ReleaseNotesUri.Scheme == Uri.UriSchemeHttp || ReleaseNotesUri.Scheme == Uri.UriSchemeHttps)
                {
                    System.Diagnostics.Process.Start(releaseNotesString);
                }
                else
                {
                    error = Properties.Resources.ReleaseNotes_ClickURLErrorMessage + " " + releaseNotesString;
                    MessageDialog.Show(error);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                error = Properties.Resources.ReleaseNotes_ClickLoadErrorMessage + " " + releaseNotesString;
                MessageDialog.Show(error);
            }
#pragma warning restore CA1031 // Do not catch general exception types
            Logger.PublishTelemetryEvent(TelemetryEventFactory.ForReleaseNotesClick(error));
        }
    }
}
