// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for UpdateDialog.xaml
    /// </summary>
    public partial class UpdateContainedDialog : ContainedDialog
    {
        public Uri ReleaseNotesUri { get; }

        /// <summary>
        /// Initializes update dialog
        /// </summary>
        /// <param name="releaseNotesUri">Uri to release notes</param>
        public UpdateContainedDialog(Uri releaseNotesUri)
        {
            InitializeComponent();
            this.ReleaseNotesUri = releaseNotesUri;
            WaitHandle.Reset();
        }

        private void UpdateNow_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            WaitHandle.Set();
        }

        private void UpdateLater_Dismiss(object sender, RoutedEventArgs e)
        {
            DismissDialog();
        }

        private void UpdateLater_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Close();
        }

        public void SetModeToRequired()
        {
            lblUpdate.Content = Properties.Resources.UpdateContainedDialog_An_update_is_required;
            btnUpdateLater.Content = Properties.Resources.closeDialogText;
            btnUpdateLater.Click -= UpdateLater_Dismiss;
            btnUpdateLater.Click += UpdateLater_Close;
        }

        public override void SetFocusOnDefaultControl()
        {
            btnUpdateNow.Focus();
        }

        private void hlReleaseNotes_Click(object sender, RoutedEventArgs e)
        {
            string error = string.Empty;
            string releaseNotesString = string.Empty;
            try
            {
                releaseNotesString = ReleaseNotesUri.ToString();

                if (ReleaseNotesUri.Scheme == Uri.UriSchemeHttp || ReleaseNotesUri.Scheme == Uri.UriSchemeHttps)
                {
                    Process.Start(releaseNotesString);
                }
                else
                {
                    error = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ReleaseNotes_ClickURLErrorMessage, releaseNotesString);
                    MessageDialog.Show(error);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                error = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ReleaseNotes_ClickLoadErrorMessage, releaseNotesString);
                MessageDialog.Show(error);
            }
#pragma warning restore CA1031 // Do not catch general exception types
            Logger.PublishTelemetryEvent(TelemetryEventFactory.ForReleaseNotesClick(error));
        }
    }
}
