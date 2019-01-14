// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Dialogs;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    /// <summary>
    /// Interaction logic for FeedbackControl1.xaml
    /// </summary>
    public partial class FeedbackControl : UserControl
    {

        public FeedbackControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Opens the default process and navigates to the provided uri.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
            catch
            {
                MessageDialog.Show(Properties.Resources.FeedbackControl_hlLink_RequestNavigate_Failed_to_navigate_to_requested_url);
            }
        }
    }
}
