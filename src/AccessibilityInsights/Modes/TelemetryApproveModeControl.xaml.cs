// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.DesktopUI.Controls;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for TelemetryApproveModeControl.xaml
    /// </summary>
    public partial class TelemetryApproveModeControl : UserControl
    {
        public TelemetryApproveModeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "page");
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
            ConfigurationManager.GetDefaultInstance().AppConfig.ShowTelemetryDialog = false;
            ConfigurationManager.GetDefaultInstance().AppConfig.EnableTelemetry = ckbxAgreeToHelp.IsChecked.Value;
            Logger.IsTelemetryAllowed = ckbxAgreeToHelp.IsChecked.Value;
            HideControl();
        }

        public void HideControl()
        {
            this.Visibility = Visibility.Collapsed;
        }

        public void ShowControl()
        {
            this.Visibility = Visibility.Visible;

            Dispatcher.InvokeAsync(() =>
            {
                this.SetFocusOnDefaultControl();
            }
           , System.Windows.Threading.DispatcherPriority.Input);
        }

        public void SetFocusOnDefaultControl()
        {
            this.ckbxAgreeToHelp.Focus();
        }
    }
}
