// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using AccessibilityInsights.SharedUx.Settings;
using System.Windows.Automation.Peers;
using System.Diagnostics;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using System.Globalization;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for StartUpModeControl.xaml
    /// </summary>
    public partial class StartUpModeControl : UserControl, IModeControl
    {
        /// <summary>
        /// URL to getting started video
        /// </summary>
        const string VideoUrl = "https://aka.ms/hmeji4";

        /// <summary>
        /// MainWindow to access shared methods
        /// </summary>
        static MainWindow MainWin
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
        }

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
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "page");
        }

        /// <summary>
        /// Most recent version string
        /// </summary>
        public string VersionString { get; private set; }

        public StartUpModeControl()
        {
            this.VersionString = AccessibilityInsights.Core.Misc.Utility.GetAppVersion();
            InitializeComponent();
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

        // <summary>
        // Updates Window size with stored data
        // </summary>
        public void AdjustMainWindowSize()
        {
            MainWin.SizeToContent = SizeToContent.Manual;
        }

        ///not implemented--nothing will copy
        public void CopyToClipboard()
        {
            return;
        }

        /// <summary>
        /// Hide control and hilighter
        /// </summary>
        public void HideControl()
        {
            UpdateConfigWithSize();
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Show control and hilighter
        /// </summary>
        public void ShowControl()
        {
            AdjustMainWindowSize();
            UpdateHotkeyLabels();
            this.Visibility = Visibility.Visible;

            Dispatcher.InvokeAsync(() =>
            {
                this.SetFocusOnDefaultControl();
            }
           , System.Windows.Threading.DispatcherPriority.Input);
        }

        /// <summary>
        /// not needed. 
        /// </summary>
        /// <param name="ecId"></param>
#pragma warning disable CS1998
        public async Task SetElement(Guid ecId) { }
#pragma warning restore CS1998

        /// <summary>
        /// Not needed.
        /// </summary>
        public void UpdateConfigWithSize() { }

        public void Clear()
        {
        }

        /// <summary>
        /// Event handler for Got it button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            if (ckbxDontShow.IsChecked.Value)
            {
                ConfigurationManager.GetDefaultInstance().AppConfig.ShowWelcomeScreenOnLaunch = false;
            }
            MainWin.HandleBackToSelectingState();
        }

        /// <summary>
        /// Refresh button is not needed on main command bar
        /// </summary>
        public bool IsRefreshEnabled { get { return false; } }

        /// <summary>
        /// Save button is not neeeded on main command bar
        /// </summary>
        public bool IsSaveEnabled { get { return false; } }

        /// <summary>
        /// No action
        /// </summary>
        public void Refresh()
        {
        }

        /// <summary>
        /// No action
        /// </summary>
        public void Save()
        {
        }

        /// <summary>
        /// Handle toggle highlighter request
        /// </summary>
        /// <returns></returns>
        public bool ToggleHighlighter()
        {
            return true;
        }

        /// <summary>
        /// Set focus on default control for mode
        /// </summary>
        public void SetFocusOnDefaultControl()
        {
            this.btnVideo.Focus();
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
            catch
            {
                MessageDialog.Show(string.Format(CultureInfo.InvariantCulture,
                    Properties.Resources.btnVideo_ClickException, VideoUrl));
            }
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
            catch
            {
                MessageDialog.Show(Properties.Resources.hlLink_RequestNavigateException);
            }
        }
    }
}
