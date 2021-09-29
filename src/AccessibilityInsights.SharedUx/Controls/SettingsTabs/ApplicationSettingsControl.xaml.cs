// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    /// <summary>
    /// Interaction logic for ApplicationSettingsControl.xaml
    /// </summary>
    public partial class ApplicationSettingsControl : UserControl
    {
        /// <summary>
        /// Dialog to select hotkey
        /// </summary>
        HotkeyGrabDialog HkDialog;

        public ReleaseChannel SelectedReleaseChannel => ctrlChannelConfig.VM.CurrentChannel;

        /// <summary>
        /// Updates save button state based on data changes
        /// </summary>
        public Action UpdateSaveButton
        {
            get
            {
                return DataContextVM.UpdateSaveButton;
            }
            set
            {
                DataContextVM.UpdateSaveButton = value;
                ctrlChannelConfig.SetUpdateSaveButton(value);
            }
        }

        private ApplicationSettingsViewModel DataContextVM = new ApplicationSettingsViewModel();

        /// <summary>
        /// Keep the list of Hotkey buttons for checking duplication later.
        /// </summary>
        private List<Button> HotKeyButtons;

        /// <summary>
        /// Constructor
        /// </summary>
        public ApplicationSettingsControl()
        {
            InitializeComponent();
            this.DataContext = DataContextVM;
            this.HotKeyButtons = new List<Button>
            {
                this.btnHotekyToNext,
                this.btnHotkeyActivateWindow,
                this.btnHotkeyRecord,
                this.btnHotkeyPause,
                this.btnHotkeyToBefore,
                this.btnHotkeyToFirstChild,
                this.btnHotkeyToggle,
                this.btnHotkeyToParent
            };
            if (!TelemetryController.DoesGroupPolicyAllowTelemetry)
            {
                this.telemetryDescription.Text = Properties.Resources.ApplicationSettingsControl_TelemetryDisabledByAdministrator;
                this.privacyLearnMore.Visibility = Visibility.Collapsed;
                this.lblEnableTelemetryLabel.Visibility = Visibility.Collapsed;
                this.tgbtnEnableTelemetry.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Opens hotkey recording dialog. Does not allow more than one
        /// to be open at a time.
        /// </summary>
        /// <param name="button"></param>
        private void OpenHotkeyDialog(Button button)
        {
            if (HkDialog == null || !HkDialog.IsLoaded)
            {
                HkDialog = new HotkeyGrabDialog(button);
                HkDialog.Owner = Window.GetWindow(this);
                HkDialog.ShowDialog();
            }
        }

        /// <summary>
        /// Don't let user type letters into mouse timer delay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textboxMouseDelay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0)
            {
                if (!char.IsDigit(e.Text, e.Text.Length - 1))
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Update save button if delay text changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbMouseDelay_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.UpdateSaveButton != null)
            {
                UpdateSaveButton();
            }
        }

        /// <summary>
        /// Check if user has changed any settings
        /// </summary>
        /// <returns></returns>
        public bool IsConfigurationChanged(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.HotKeyForActivatingMainWindow != (string)this.btnHotkeyActivateWindow.Content)
            {
                return true;
            }
            if (config.HotKeyForRecord != (string)this.btnHotkeyRecord.Content)
            {
                return true;
            }
            if (config.HotKeyForPause != (string)this.btnHotkeyPause.Content)
            {
                return true;
            }
            if (config.HotKeyForSnap != (string)this.btnHotkeyToggle.Content)
            {
                return true;
            }
            if (config.HotKeyForMoveToParent != (string)this.btnHotkeyToParent.Content)
            {
                return true;
            }
            if (config.HotKeyForMoveToPreviousSibling != (string)this.btnHotkeyToBefore.Content)
            {
                return true;
            }
            if (config.HotKeyForMoveToNextSibling != (string)this.btnHotekyToNext.Content)
            {
                return true;
            }
            if (config.HotKeyForMoveToFirstChild != (string)this.btnHotkeyToFirstChild.Content)
            {
                return true;
            }
            if (config.HotKeyForMoveToLastChild != (string)this.btnTHotkeyoLastChild.Content)
            {
                return true;
            }
            if (config.SelectionByFocus != this.DataContextVM.SelectionByFocus)
            {
                return true;
            }
            if (config.SelectionByMouse != this.DataContextVM.SelectionByMouse)
            {
                return true;
            }

            // make sure that text is parsable.
            int milsec;
            var conv = int.TryParse(this.tbMouseDelay.Text, out milsec);
            if (conv == true && config.MouseSelectionDelayMilliSeconds != milsec)
            {
                return true;
            }
            if (config.AlwaysOnTop != this.DataContextVM.AlwaysOnTop)
            {
                return true;
            }
            if (config.EnableTelemetry != this.DataContextVM.EnableTelemetry)
            {
                return true;
            }
            if (config.PlayScanningSound != (this.DataContextVM.PlayScanningSound))
            {
                return true;
            }
            if (config.HighlighterMode != (HighlighterMode)(this.DataContextVM.HighlighterMode))
            {
                return true;
            }
            if (config.FontSize != (FontSize)(this.DataContextVM.FontSize))
            {
                return true;
            }

            if (ctrlChannelConfig.GetConfigChanged(config))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Updates display elements with config settings
        /// </summary>
        public void UpdateFromConfig(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            this.btnHotkeyToggle.Content = config.HotKeyForSnap;
            this.btnHotkeyActivateWindow.Content = config.HotKeyForActivatingMainWindow;
            this.btnHotkeyRecord.Content = config.HotKeyForRecord;
            this.btnHotkeyPause.Content = config.HotKeyForPause;
            this.btnHotkeyToParent.Content = config.HotKeyForMoveToParent;
            this.btnHotkeyToBefore.Content = config.HotKeyForMoveToPreviousSibling;
            this.btnHotekyToNext.Content = config.HotKeyForMoveToNextSibling;
            this.btnHotkeyToFirstChild.Content = config.HotKeyForMoveToFirstChild;
            this.btnTHotkeyoLastChild.Content = config.HotKeyForMoveToLastChild;
            this.tbMouseDelay.Text = config.MouseSelectionDelayMilliSeconds.ToString(CultureInfo.InvariantCulture);
            (this.DataContext as ApplicationSettingsViewModel).UpdateFromConfig(config);
            ctrlChannelConfig.UpdateFromConfig(config);
        }

        /// <summary>
        /// Updates configuration with user's selections
        /// </summary>
        public void UpdateConfigFromSelections(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (config.HotKeyForSnap != (string) this.btnHotkeyToggle.Content)
            {
                var dic = new Dictionary<string, string>();
                dic.Add("HotkeyToggleMode", (string) this.btnHotkeyToggle.Content);
            }

            if (config.HotKeyForActivatingMainWindow != (string)this.btnHotkeyActivateWindow.Content)
            {
                var dic = new Dictionary<string, string>();
                dic.Add("HotkeyActivateWIndow", (string)this.btnHotkeyActivateWindow.Content);
            }

            if (config.HotKeyForRecord != (string)this.btnHotkeyRecord.Content)
            {
                var dic = new Dictionary<string, string>();
                dic.Add("HotkeyRecord", (string)this.btnHotkeyRecord.Content);
            }

            if (config.HotKeyForPause != (string)this.btnHotkeyPause.Content)
            {
                var dic = new Dictionary<string, string>();
                dic.Add("HotkeyPause", (string)this.btnHotkeyPause.Content);
            }

            config.HotKeyForActivatingMainWindow = (string)this.btnHotkeyActivateWindow.Content;
            config.HotKeyForSnap = (string)this.btnHotkeyToggle.Content;
            config.HotKeyForRecord = (string)this.btnHotkeyRecord.Content;
            config.HotKeyForPause = (string)this.btnHotkeyPause.Content;
            config.HotKeyForMoveToParent = (string)this.btnHotkeyToParent.Content;
            config.HotKeyForMoveToPreviousSibling = (string)this.btnHotkeyToBefore.Content;
            config.HotKeyForMoveToNextSibling = (string)this.btnHotekyToNext.Content;
            config.HotKeyForMoveToFirstChild = (string)this.btnHotkeyToFirstChild.Content;
            config.HotKeyForMoveToLastChild = (string)this.btnTHotkeyoLastChild.Content;
            config.MouseSelectionDelayMilliSeconds = Math.Max(int.Parse(this.tbMouseDelay.Text,CultureInfo.InvariantCulture), ConfigurationModel.MinimumSelectionDelayMilliseconds); // make sure that we allow only bigger than minimum value.
            DataContextVM.SaveToConfig(config);
        }

        /// <summary>
        /// Handles click on hotkey pause button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnToggleHk_Click(object sender, RoutedEventArgs e)
        {
            OpenHotkeyDialog(sender as Button);

            if (HadDuplicatedHotkeys())
            {
                MessageDialog.Show(Properties.Resources.ApplicationSettingsControl_btnToggleHk_Click_The_hotkey_you_assigned_is_duplicated_with_the_existing_one_please_set_a_new_hotkey);
            }
            else
            {
                UpdateSaveButton();
            }
        }

        /// <summary>
        /// Check whether all hot keys are duplicated or not.
        /// </summary>
        /// <returns></returns>
        private bool HadDuplicatedHotkeys()
        {
            return (from hkb in HotKeyButtons
                    let n = (string)hkb.Content
                    group n by n into g
                    where g.Count() > 1
                    select g).Any();
        }

        /// <summary>
        /// Open drop down to populate UIA element for narrator announcement
        /// </summary>
        /// <param name="sender">sender of event</param>
        /// <param name="e">event arguments</param>
        private void cbComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null)
                return;

            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                combo.IsDropDownOpen = true;
            }
            else if (e.Key == Key.Left)
            {
                combo.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                combo.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                e.Handled = true;
            }
        }
    }
}

