// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for PrivacyLearnMore.xaml
    /// </summary>
    public partial class PrivacyLearnMore : TextBlock
    {
        public PrivacyLearnMore()
        {
            InitializeComponent();
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
                MessageDialog.Show(string.Format(CultureInfo.CurrentCulture, Properties.Resources.InvalidLink, e.Uri.AbsoluteUri));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
