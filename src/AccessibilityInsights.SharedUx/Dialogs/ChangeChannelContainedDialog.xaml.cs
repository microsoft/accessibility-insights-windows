// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Navigation;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for TelemetryApproveContainedDialog.xaml
    /// </summary>
    public partial class ChangeChannelContainedDialog : ContainedDialog
    {
        public ChangeChannelContainedDialog()
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
            catch
            {
                MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, Properties.Resources.TelemetryDialog_Hyperlink_RequestNavigate_Invalid_Link, e.Uri.AbsoluteUri));
            }
        }

        /// <summary>
        /// Updates telemetry settings based on user input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            WaitHandle.Set();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            WaitHandle.Set();
        }
    }
}
