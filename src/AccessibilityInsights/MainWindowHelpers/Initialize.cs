// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.KeyboardHelpers;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Win32;
using Axe.Windows.Actions;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace AccessibilityInsights
{
    /// <summary>
    /// MainWindow partial class for Initializing AccessibilityInsights
    /// </summary>
    public partial class MainWindow
    {
        public HotKeyHandler HotkeyHandler { get; private set; }
        readonly TreeNavigator TreeNavigator;

        void InitStartMode()
        {
            this.CurrentPage = AppPage.Start;
            PageTracker.TrackPage(this.CurrentPage, null);
            this.gdModes.Visibility = Visibility.Visible;
            this.btnPause.Visibility = Visibility.Visible;
            ctrlDialogContainer.ShowDialog(new StartUpModeControl()).ContinueWith(BackToSelectingOnDispatcher, TaskScheduler.Current);
            ShowTelemetryDialog();
            CheckForUpdates();
        }

        private void Initialize()
        {
            this.ctrlCurMode = ctrlLiveMode;
            this.ctrlCurMode.ShowControl();

            UpdateTitleString();

            this.hWnd = new WindowInteropHelper(this).Handle;

            ImageOverlayDriver.SetHighlightBtnState = SetHighlightBtnState;
            HollowHighlightDriver.GetDefaultInstance().HighlighterMode = ConfigurationManager.GetDefaultInstance().AppConfig.HighlighterMode;

            InitHotKeys();
            InitSelectActionMode();
            InitTimerSelector();
            InitTimerAutoSnap();
            InitAccessKeyRule();
            InitHighlighter();

            if (ConfigurationManager.GetDefaultInstance().AppConfig.NeedToShowWelcomeScreen() && CommandLineSettings.FileToOpen == null)
            {
                InitStartMode();
            }
            else
            {
                HandleBackToSelectingState();
                ShowTelemetryDialog();
                EnableElementSelector();
                CheckForUpdates();
            }

            // chrome height is set to make sure system menu is shown over title bar area.
            var chrome = new WindowChrome();
            chrome.CaptionHeight = 35;
            WindowChrome.SetWindowChrome(this, chrome);

            UpdateTabSelection();
        }

        private void BackToSelectingOnDispatcher(Task<bool> _) => Dispatcher.Invoke(HandleBackToSelectingState);

        /// <summary>
        ///  Update main window layout.
        /// </summary>
        private void UpdateMainWindowLayout()
        {
            AppLayout layout = ConfigurationManager.GetDefaultInstance().AppLayout;
            EnsureWindowIsInVirtualScreen(layout);

            this.Top = layout.Top;
            this.Left = layout.Left;
            this.Width = layout.Width;
            this.Height = layout.Height;
            this.WindowState = layout.WinState;
        }

        /// <summary>Load and register for custom UI Automation properties if a configuration file exists.</summary>
        private static void InitCustomUIA(string ConfigurationFolderPath, TelemetryBuffer telemetryBuffer)
        {
            const string ConfigurationFileName = "CustomUIA.json";
            string path = Path.Combine(ConfigurationFolderPath, ConfigurationFileName);
            if (File.Exists(path))
            {
                Axe.Windows.Core.CustomObjects.Config config = CustomUIAAction.ReadConfigFromFile(path);
                if (config?.Properties != null)
                {
                    CustomUIAAction.RegisterCustomProperties(config.Properties);
                    telemetryBuffer.AddEventFactory(() => TelemetryEventFactory.ForCustomUIAPropertyCount(config.Properties.Length));
                }
            }
        }

        /// <summary>
        /// Populate all configurations
        /// </summary>
        private static void PopulateConfigurations(TelemetryBuffer telemetryBuffer)
        {
            var defaultPaths = FixedConfigSettingsProvider.CreateDefaultSettingsProvider();
            var configPathProvider = new FixedConfigSettingsProvider(
                CommandLineSettings.ConfigFolder ?? defaultPaths.ConfigurationFolderPath,
                CommandLineSettings.UserDataFolder ?? defaultPaths.UserDataFolderPath
            );

            FileHelpers.CreateFolder(configPathProvider.UserDataFolderPath);
            FileHelpers.CreateFolder(configPathProvider.ConfigurationFolderPath);

            // Populate the App Config and Test Config
            ConfigurationManager.GetDefaultInstance(configPathProvider);

            InitCustomUIA(configPathProvider.ConfigurationFolderPath, telemetryBuffer);

            // based on customer feedback, we will set default selection mode to Element
            // when AccessibilityInsights starts up.
            ConfigurationManager.GetDefaultInstance().AppConfig.IsUnderElementScope = true;

            // Configure the correct ReleaseChannel for autoupdate
            Container.SetAutoUpdateReleaseChannel(ConfigurationManager.GetDefaultInstance().AppConfig.ReleaseChannel.ToString());

            // Opt into telemetry if allowed and user has chosen to do so
            if (TelemetryController.DoesGroupPolicyAllowTelemetry &&
                ConfigurationManager.GetDefaultInstance().AppConfig.EnableTelemetry)
            {
                TelemetryController.OptIntoTelemetry();
            }

            // Update theming since it depends on config options
            SetColorTheme();
        }

        /// <summary>
        /// Initialize the highlighter
        /// </summary>
        private void InitHighlighter()
        {
            // set the beaker callback.
            HollowHighlightDriver.GetDefaultInstance().SetCallBackForSnapshot(new Action(() =>
            {
                HandleSnapshotRequest(TestRequestSources.Beaker);
            }));

        }

        /// <summary>
        /// Initialize a hotkey handler and HotKeys
        /// </summary>
        private void InitHotKeys()
        {
            this.HotkeyHandler = HotKeyHandler.GetHotkeyHandler(new WindowInteropHelper(this).Handle);

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
            var hk = HotKey.GetInstance(hkcombo);

            if (hk != null)
            {
                if (this.HotkeyHandler.Exist(hk) == false)
                {
                    hk.SetConditionalAction(action, () => !ctrlConfigurationMode.IsVisible);

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
                          lock (_lockObject)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (_lockObject)
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
                                              this.StartEventsMode(GetDataAction.GetElementContext(sa.SelectedElementContextId.Value).Element);
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
                          lock (_lockObject)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (_lockObject)
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
                          lock (_lockObject)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (_lockObject)
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
                          lock (_lockObject)
                          {
                              Dispatcher.Invoke(() =>
                              {
                                  lock (_lockObject)
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
                lock (_lockObject)
                {
                    Dispatcher.Invoke(action);
                } // lock
            }; // lambda
        }

        /// <summary>
        /// Make sure that the app layout is fully within the virtual screen, including resizing it
        /// if necessary.
        /// </summary>
        /// <param name="layout">The layout. Its position will be modified only if necessary</param>
        private void EnsureWindowIsInVirtualScreen(AppLayout layout)
        {
            EnsureWindowIsInVirtualScreenWithInjection(layout, this.Top, this.Left,
                SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth, SystemParameters.VirtualScreenHeight);
        }

        /// <summary>
        /// Return the first defined number from an array of potentialNumbers.
        /// Will return 0.0 if they're all NaN.
        /// </summary>
        private static double GetFirstDefinedNumber(double[] potentialNumbers)
        {
            foreach(double potentialNumber in potentialNumbers)
            {
                if (!double.IsNaN(potentialNumber))
                {
                    return potentialNumber;
                }
            }

            return 0.0; // Our ultimate fallback value
        }

        internal static void EnsureWindowIsInVirtualScreenWithInjection(AppLayout layout,
            double windowTop, double windowLeft, double virtualLeft,
            double virtualTop, double virtualWidth, double virtualHeight)
        {
            double virtualRight = virtualLeft + virtualWidth;
            double virtualBottom = virtualTop + virtualHeight;

            if (layout != null)
            {
                const double fallbackWindowPosition = 200.0;
                layout.Top = GetFirstDefinedNumber(new double[] { layout.Top, windowTop, fallbackWindowPosition });
                layout.Left = GetFirstDefinedNumber(new double[] { layout.Left, windowLeft, fallbackWindowPosition });

                double top = layout.Top;
                double left = layout.Left;
                double right = left + layout.Width;
                double bottom = top + layout.Height;

                // If the window is completely offscreen, open it in default location
                if ((right <= virtualLeft) ||
                    (bottom <= virtualTop) ||
                    (left >= virtualRight) ||
                    (top >= virtualBottom))
                {
                    top = layout.Top = windowTop;
                    left = layout.Left = windowLeft;
                }

                // Adjust the window extents to keep it fully within the virtual screen
                layout.Width = Math.Min(layout.Width, virtualWidth);
                layout.Height = Math.Min(layout.Height, virtualHeight);
                right = left + layout.Width;
                bottom = top + layout.Height;

                if (right > virtualRight)
                {
                    layout.Left = virtualRight - layout.Width;
                }
                else if (left < virtualLeft)
                {
                    layout.Left = virtualLeft;
                }

                if (bottom > virtualBottom)
                {
                    layout.Top = virtualBottom - layout.Height;
                }
                else if (top < virtualTop)
                {
                    layout.Top = virtualTop;
                }
            }
        }
    }
}
