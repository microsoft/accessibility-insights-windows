// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.Actions;
using AccessibilityInsights.Desktop.Keyboard;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.Win32;
using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Shell;
using System.Globalization;
using AccessibilityInsights.Desktop.Telemetry;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Initializing AccessibilityInsights
    /// </summary>
    public partial class MainWindow
    {
        public HotkeysHandler HotkeyHandler { get; private set; }
        readonly TreeNavigator TreeNavigator = null;

        /// <summary>
        /// Start Start Mode
        /// </summary>
        /// <returns></returns>
        void StartStartMode()
        {
            this.CurrentPage = AppPage.Start;
            this.ctrlCurMode = ctrlStartUpMode;
            PageTracker.TrackPage(this.CurrentPage, null);

            /// Make sure that title and Name property are set with same value. 
            UpdateTitleString();

            this.hWnd = new WindowInteropHelper(this).Handle;

            HighlightImageAction.SetHighlightBtnState = SetHighlightBtnState;
            HighlightAction.GetDefaultInstance().HighlighterMode = ConfigurationManager.GetDefaultInstance().AppConfig.HighlighterMode;

            InitHotKeys();
            InitSelectActionMode();
            InitTimerSelector();
            InitTimerAutoSnap();
            InitAccessKeyRule();
            InitHighlighter();

            /// Set UI appropriately if showing startup screen
            if (ConfigurationManager.GetDefaultInstance().AppConfig.NeedToShowWelcomeScreen())
            {
                this.bdLeftNav.IsEnabled = false;
                this.gdModes.Visibility = Visibility.Collapsed;
                this.ctrlStartUpMode.ShowControl();
                //show telemetry dialog
                ShowTelemetryDialog();
                // make sure that we show update. 
                CheckForUpdates();
            }
            else
            {
                HandleBackToSelectingState();
                // make sure that we disable selector before showing update
                // otherwise, UI could be in weird state (Main window is gray out but still show selection and update dialog is disappearing)
                DisableElementSelector();
                //show telemetry dialog
                ShowTelemetryDialog();
                CheckForUpdates();
                EnableElementSelector();
            }

            // chrome height is set to make sure system menu is shown over title bar area. 
            var chrome = new WindowChrome();
            chrome.CaptionHeight = 35;
            WindowChrome.SetWindowChrome(this, chrome);

            UpdateTabSelection();
        }

        /// <summary>
        ///  Update main window layout. 
        /// </summary>
        private void UpdateMainWindowLayout()
        {
            var layout = ConfigurationManager.GetDefaultInstance().AppLayout;

            FixWindowPositionForStartup();

            this.Top = layout.Top;
            this.Left = layout.Left;
            this.Width = layout.Width;
            this.Height = layout.Height;
            this.WindowState = layout.WinState;
        }


        /// <summary>
        /// Populate all configurations
        /// </summary>
        private static void PopulateConfigurations()
        {
            // make sure that necessary folders are created. 
            DirectoryManagement.CreateFolders();

            // Populate the App Config and Test Config
            ConfigurationManager.GetDefaultInstance();

            // based on customer feedback, we will set default selection mode to Element
            // when AccessibilityInsights starts up. 
            ConfigurationManager.GetDefaultInstance().AppConfig.IsUnderElementScope = true;
            
            //AK - remove
            ConfigurationManager.GetDefaultInstance().AppConfig.SelectedIssueReporter = Guid.Parse("27f21dff-2fb3-4833-be55-25787fce3e17");
            Dictionary<Guid, string> configs = new Dictionary<Guid, string>() { { Guid.Parse("27f21dff-2fb3-4833-be55-25787fce3e17"), "hello world" } };
            string serializedConfig = JsonConvert.SerializeObject(configs);
            ConfigurationManager.GetDefaultInstance().AppConfig.IssueReporterSerializedConfigs = serializedConfig;
            // enable/disable telemetry
            Logger.IsTelemetryAllowed = ConfigurationManager.GetDefaultInstance().AppConfig.EnableTelemetry;
        }

        /// <summary>
        /// Initialize the highlighter
        /// </summary>
        private void InitHighlighter()
        {
            // set the beaker callback.
            HighlightAction.GetDefaultInstance().SetCallBackForSnapshot(new Action(() =>
            {
                HandleSnapshotRequest(TestRequestSources.Beaker);
            }));

        }

        /// <summary>
        /// Initialize a hotkey handler and HotKeys
        /// </summary>
        private void InitHotKeys()
        {
            this.HotkeyHandler = HotkeysHandler.GetHotkeyHandler(new WindowInteropHelper(this).Handle);

            SetHotKeyForModeSwitch();
            SetHotKeyForToggleRecord();
            SetHotKeyForTogglePause();

            SetHotKeyForActivatingMainWindow();

            SetHotKeyForMoveToParent();
            SetHotKeyForMoveToFirstChild();
            SetHotKeyForMoveToLastChild();
            SetHotKeyForMoveToNextSibbling();
            SetHotKeyForMoveToPreviousSibbling();
        }

        /// <summary>
        /// Update Select Action Mode
        /// </summary>
        private static void InitSelectActionMode()
        {
            var sa = SelectAction.GetDefaultInstance();

            sa.Stop();
            sa.IntervalMouseSelector = ConfigurationManager.GetDefaultInstance().AppConfig.MouseSelectionDelayMilliSeconds;
            sa.IsFocusSelectOn = ConfigurationManager.GetDefaultInstance().AppConfig.SelectionByFocus;
            sa.IsMouseSelectOn = ConfigurationManager.GetDefaultInstance().AppConfig.SelectionByMouse;
            sa.TreeViewMode = ConfigurationManager.GetDefaultInstance().AppConfig.TreeViewMode;

        }

        /// <summary>
        /// Set HotKey
        /// </summary>
        /// <param name="hkcombo"></param>
        /// <param name="action"></param>
        /// <param name="errmsg"></param>
        private void SetHotKey(string hkcombo, Action action, string errmsg)
        {
            var hk = Hotkey.GetInstance(hkcombo);

            if (hk != null)
            {
                if (this.HotkeyHandler.Exist(hk) == false)
                {
                    hk.Action = action;

                    this.HotkeyHandler.RegisterHotKey(hk);
                }
            }
            else
            {
                this.AllowFurtherAction = false;
                MessageDialog.Show(errmsg);
                this.AllowFurtherAction = true;
            }
        }

        /// <summary>
        /// Set Hot Key for event recording
        /// honor the value from Configuration file. 
        /// </summary>
        private void SetHotKeyForToggleRecord()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForRecord,
                      new Action(() =>
                      {
                          lock (this)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (this)
                                  {
                                      if (this.CurrentPage == AppPage.Test && (TestView)this.CurrentView == TestView.TabStop)
                                      {
                                          ctrlTestMode.ctrlTabStop.ToggleRecording();
                                      }
                                      else if (this.CurrentPage == AppPage.Events && this.CurrentView != EventsView.Load)
                                      {
                                          ctrlEventMode.ctrlEvents.ToggleRecording();
                                      }
                                      else
                                      {
                                          var sa = SelectAction.GetDefaultInstance();

                                          // make sure that POI is set. 
                                          if (this.IsInSelectingState() && sa.HasPOIElement())
                                          {
                                              this.StartEventsMode(GetDataAction.GetElementContext(sa.GetSelectedElementContextId().Value).Element);
                                              UpdateMainWindowUI();
                                          }
                                      }
                                  }
                              });
                          }
                      }),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        /// <summary>
        /// Set Hot Key for event recording
        /// honor the value from Configuration file. 
        /// </summary>
        private void SetHotKeyForTogglePause()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForPause,
                      new Action(() =>
                      {
                          lock (this)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (this)
                                  {
                                      if (this.btnPause.IsVisible)
                                      {
                                          if (!IsCapturingData())
                                          {
                                              HandlePauseButtonToggle(this.ToggleUIATreePlayerState);
                                          }
                                      }
                                  }
                              });
                          }
                      }),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        /// <summary>
        /// Set Hot Key for Mode Switch
        /// honor the value from Configuration file. 
        /// </summary>
        private void SetHotKeyForModeSwitch()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap,
                      new Action(() =>
                      {
                          lock (this)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (this)
                                  {
                                      HandleRunTestsByHotkey();
                                      BringToForeground();
                                      this.ctrlCurMode.SetFocusOnDefaultControl();
                                  }
                              });
                          }
                      }),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        /// <summary>
        /// Set Hot key to activate or minimize window
        /// </summary>
        private void SetHotKeyForActivatingMainWindow()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForActivatingMainWindow,
                      new Action(() =>
                      {
                          lock (this)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (this)
                                  {
                                      if (this.hWnd == NativeMethods.GetForegroundWindow())
                                      {
                                          this.WindowState = WindowState.Minimized;
                                      }
                                      else
                                      {
                                          this.WindowState = WindowState.Normal;
                                          BringToForeground();
                                          this.ctrlCurMode.SetFocusOnDefaultControl();
                                      }
                                  }
                              });
                          }
                      }),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        /// <summary>
        /// return formatted string to be used as an error message for invalid hotkeys
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ErrorMessage(string message)
        {
            return string.Format(CultureInfo.InvariantCulture
                , Properties.Resources.CultureInfoErrorMessage
                , message);
        }
        private void SetHotKeyForMoveToParent()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForMoveToParent,
                      InvokeAction(this.TreeNavigator.MoveToParent),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        private void SetHotKeyForMoveToFirstChild()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForMoveToFirstChild,
                      InvokeAction(this.TreeNavigator.MoveToFirstChild),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        private void SetHotKeyForMoveToLastChild()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForMoveToLastChild,
                      InvokeAction(this.TreeNavigator.MoveToLastChild),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        private void SetHotKeyForMoveToNextSibbling()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForMoveToNextSibling,
                      InvokeAction(this.TreeNavigator.MoveToNextSibbling),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        private void SetHotKeyForMoveToPreviousSibbling()
        {
            SetHotKey(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForMoveToPreviousSibling,
                      InvokeAction(this.TreeNavigator.MoveToPreviousSibbling),
                    ErrorMessage(ConfigurationManager.GetDefaultInstance().AppConfig.HotKeyForSnap));
        }

        private Action InvokeAction(Action action)
        {
            return () =>
            {
                lock (this)
                {
                    Dispatcher.Invoke(action);
                } // lock
            }; // lambda
        }

        /// <summary>
        /// Sets window position to avoid opening window offscreen. Modelled after explorer's behavior
        /// If fully offscreen, open in default position
        /// If partially offscreen, move onscreen
        /// </summary>
        /// <returns></returns>
        private void FixWindowPositionForStartup()
        {
            var layout = ConfigurationManager.GetDefaultInstance().AppLayout;

            if (layout != null)
            {
                // If the window is completely offscreen, open it in default location
                if ((layout.Left <= SystemParameters.VirtualScreenLeft - layout.Width) ||
                        (layout.Top <= SystemParameters.VirtualScreenTop - layout.Height) ||
                        (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth <= layout.Left) ||
                        (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight <= layout.Top))
                {
                    layout.Top = this.Top;
                    layout.Left = this.Left;
                }
                else // if the window is partially offscreen, move it the appropriate vertical and/or horizontal distance to become fully onscreen
                {
                    if (layout.Left < SystemParameters.VirtualScreenLeft)
                    {
                        layout.Left = SystemParameters.VirtualScreenLeft;
                    }
                    else if (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth - layout.Width < layout.Left)
                    {
                        layout.Left = SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth - layout.Width;
                    }

                    if (layout.Top < SystemParameters.VirtualScreenTop)
                    {
                        layout.Top = SystemParameters.VirtualScreenTop;
                    }
                    else if (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight - layout.Height < layout.Top)
                    {
                        layout.Top = SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight - layout.Height;
                    }
                }
            }
        }
    }
}
