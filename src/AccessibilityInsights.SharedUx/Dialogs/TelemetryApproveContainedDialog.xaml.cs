// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for TelemetryApproveContainedDialog.xaml
    /// </summary>
    public partial class TelemetryApproveContainedDialog : ContainedDialog
    {
        public TelemetryApproveContainedDialog()
        {
            InitializeComponent();
            WaitHandle.Reset();
        }

        /// <summary>
        /// Go to link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, Properties.Resources.TelemetryDialog_Hyperlink_RequestNavigate_Invalid_Link, e.Uri.AbsoluteUri));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Updates telemetry settings based on user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationManager.GetDefaultInstance().AppConfig.ShowTelemetryDialog = false;
            ConfigurationManager.GetDefaultInstance().AppConfig.EnableTelemetry = ckbxAgreeToHelp.IsChecked.Value;

            DialogResult = ckbxAgreeToHelp.IsChecked ?? false;

            if (DialogResult)
                TelemetryController.OptIntoTelemetry();
            else
                TelemetryController.OptOutOfTelemetry();

            WaitHandle.Set();
        }

        public override void SetFocusOnDefaultControl()
        {
            btnExit.Focus();
        }
    }
}
