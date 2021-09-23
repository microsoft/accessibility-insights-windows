// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for StartUpModeControl.xaml
    /// </summary>
    public partial class StartUpModeControl : ContainedDialog
    {
        /// <summary>
        /// URL to getting started video
        /// </summary>
        const string VideoUrl = "https://go.microsoft.com/fwlink/?linkid=2077681";

        /// <summary>
        /// App configation
        /// </summary>
        public static ConfigurationModel Configuration
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppConfig;
            }
        }

        /// <summary>
        /// Layout configuration
        /// </summary>
        public static AppLayout CurrentLayout
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppLayout;
            }
        }

        /// <summary>
        /// Label for AppVersion
        /// </summary>
        public static string VersionInfoLabel => VersionTools.AppVersionLabel;

        public static Uri AppVersionUri => VersionTools.AppVersionUri;

        public StartUpModeControl()
        {
            InitializeComponent();
            WaitHandle.Reset();
        }

        /// <summary>
        /// Update the labels for all hotkeys
        /// </summary>
        public void UpdateHotkeyLabels()
        {
            this.lblEventHk.Content = Configuration.HotKeyForRecord;
            this.lblTestHk.Content = Configuration.HotKeyForSnap;
            this.lblActivateHk.Content = Configuration.HotKeyForActivatingMainWindow;
        }

        /// <summary>
        /// Event handler for Got it button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (!ckbxShow.IsChecked.Value)
            {
                ConfigurationManager.GetDefaultInstance().AppConfig.ShowWelcomeScreenOnLaunch = false;
            }
            DismissDialog();
        }

        /// <summary>
        /// Open getting started video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(VideoUrl));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(string.Format(CultureInfo.InvariantCulture,
                    Properties.Resources.btnVideo_ClickException, VideoUrl));
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Allow user to move window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Application.Current.MainWindow.DragMove();
            }
        }

        /// <summary>
        /// Go to link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(Properties.Resources.hlLink_RequestNavigateException);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public override void SetFocusOnDefaultControl() => hlVersion.Focus();
    }
}
