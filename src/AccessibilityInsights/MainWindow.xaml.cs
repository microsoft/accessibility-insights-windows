// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.Win32;
using Axe.Windows.Actions;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using static AccessibilityInsights.Misc.FrameworkNavigator;

namespace AccessibilityInsights
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public partial class MainWindow : Window, IMainWindow, IControlTreeNavigation
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        const string HelpDocLink = "https://go.microsoft.com/fwlink/?linkid=2077919";

        IntPtr hWnd;
        private bool isClosed;

        public TwoStateButtonViewModel vmHilighter { get; private set; } = new TwoStateButtonViewModel(ButtonState.On);
        public TwoStateButtonViewModel vmLiveModePauseResume { get; private set; } = new TwoStateButtonViewModel(ButtonState.On);

        public LogoViewModel vmReporterLogo { get; private set; } = new LogoViewModel();

        public bool IsEventRecording { get; private set; }

        /// <summary>
        /// Whether or not the File Bug button should be made visible
        /// </summary>
        public static Visibility BugColumnVisibility => HelperMethods.GeneralFileBugVisibility;

        /// <summary>
        /// The width of the Bug column in the data grid (0 if bug filing isn't available)
        /// </summary>
        public static double BugColumnWidth => HelperMethods.GeneralFileBugVisibility == Visibility.Visible ? HelperMethods.FileIssueColumnWidth : 0;

        private readonly object _lockObject = new object();

        /// <summary>
        /// Is title bar context menu open
        /// </summary>
        bool IsSystemMenuOpen;

        /// <summary>
        /// Handle toggle highlighter request
        /// </summary>
        internal bool ToggleUIATreePlayerState
        {
            get => this.vmLiveModePauseResume.State != ButtonState.On;
        }

        /// <summary>
        /// AutomationProperties name for Test tab button
        /// </summary>
        public string AutomationPropertiesNameTest
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Properties.Resources.MainWindow_AutomationPropertiesName_0_1_2, Properties.Resources.btnTestAutomationPropertiesName, this.CurrentPage.ToString(), Properties.Resources.ModeIsEnabled);
            }
        }

        /// <summary>
        /// AutomationProperties name for Test tab button
        /// </summary>
        public string AutomationPropertiesNameInspect
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Properties.Resources.MainWindow_AutomationPropertiesName_0_1_2, Properties.Resources.btnInspectAutomationPropertiesName, this.CurrentPage.ToString(), Properties.Resources.ModeIsEnabled);
            }
        }

        /// <summary>
        /// AutomationProperties name for Test tab button
        /// </summary>
        public string AutomationPropertiesNameCCA
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Properties.Resources.MainWindow_AutomationPropertiesName_0_1_2, Properties.Resources.btnCCAAutomationPropertiesName, this.CurrentPage.ToString(), Properties.Resources.ModeIsEnabled);
            }
        }

        /// <summary>
        /// update the navgation bar
        /// </summary>
        private void UpdateNavigationBarAutomationName()
        {
            AutomationProperties.SetName(btnInspect, AutomationPropertiesNameInspect);
            AutomationProperties.SetName(btnTest, AutomationPropertiesNameTest);
            AutomationProperties.SetName(btnCCA, AutomationPropertiesNameCCA);
        }

        /// <summary>
        /// allow/disallow Element selection or mode switch
        /// </summary>
        /// <param name="enabled"></param>
        public void SetAllowFurtherAction(bool enabled)
        {
            lock (_lockObject)
            {
                if (AllowFurtherAction != enabled && IsCurrentModeAllowingSelection())
                {
                    AllowFurtherAction = enabled;
                }
            }
        }

        public static readonly RoutedCommand F1Command = new RoutedCommand();
        public static readonly RoutedCommand F6Command = new RoutedCommand();
        public static readonly RoutedCommand CtrlOCommand = new RoutedCommand();
        public static readonly RoutedCommand CtrlSCommand = new RoutedCommand();
        public static readonly RoutedCommand ShiftF6Command = new RoutedCommand();
        public static readonly RoutedCommand ClickInspectCommand = new RoutedCommand();
        public static readonly RoutedCommand ClickTestCommand = new RoutedCommand();
        public static readonly RoutedCommand ClickColorContrastCommand = new RoutedCommand();

        /// <summary>
        /// Contains the FrameworkElements, in order, that make up the F6 and shift + F6 pane cycle.
        /// </summary>
        private List<FrameworkElement> F6Panes;
        private List<FrameworkElement> ShiftF6Panes;

        public MainWindow()
        {
            TelemetryBuffer telemetryBuffer = new TelemetryBuffer();

            SystemEvents.UserPreferenceChanging += SystemEvents_UserPreferenceChanging;

            SetColorTheme();

            // in case we need to do any debugging with elevated app
            SupportDebugging();

            // create necessary config folders & their internal config files
            PopulateConfigurations(telemetryBuffer);

            SetFontSize();

            InitCommandBindings();

            InitializeComponent();

            this.Topmost = ConfigurationManager.GetDefaultInstance().AppConfig.AlwaysOnTop;

            UpdateMainWindowLayout();

            this.TreeNavigator = new TreeNavigator(this);
            this.TreeNavigator.SelectionChanged += this.HandleTargetSelectionChanged;

            InitTelemetry(telemetryBuffer);

            InitPanes();
        }

        /// <summary>
        /// Set up some context properties like installation id, app version, session id
        /// </summary>
        private static void InitTelemetry(TelemetryBuffer telemetryBuffer)
        {
            var configFolder = ConfigurationManager.GetDefaultInstance().SettingsProvider.ConfigurationFolderPath;
            // Initialize user info from file if it exists, reset if needed, and re-serialize
            var installInfo = InstallationInfo.LoadFromPath(configFolder, DateTime.UtcNow);
            Logger.AddOrUpdateContextProperty(TelemetryProperty.InstallationID, installInfo.InstallationGuid.ToString());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.Version, VersionTools.GetAppVersion());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.AppSessionID, Guid.NewGuid().ToString());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.SessionType, "Desktop");
            Logger.AddOrUpdateContextProperty(TelemetryProperty.ReleaseChannel, ConfigurationManager.GetDefaultInstance().AppConfig.ReleaseChannel.ToString());
            Logger.PublishTelemetryEvent(Misc.TelemetryEventFactory.ForMainWindowStartup());
            telemetryBuffer.ProcessEventFactories(Logger.PublishTelemetryEvent);
        }

        /// <summary>
        /// Listen to UserPreferenceChange to switch to high contrast theme when appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_UserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            // TODO DHT: General triggers when changing dark mode--is it too expensive?
            if (e.Category == UserPreferenceCategory.Color ||
                e.Category == UserPreferenceCategory.VisualStyle ||
                e.Category == UserPreferenceCategory.General)
            {
                SetColorTheme();
            }
        }

        /// <summary>
        /// Choose color theme based on system settings
        /// </summary>
        private static void SetColorTheme()
        {
            App.Theme theme;

            if (SystemParameters.HighContrast)
            {
                theme = App.Theme.HighContrast;
            }
            else
            {
                // Due to initialization order, config will be null the first time this is called
                ConfigurationModel config = ConfigurationManager.GetDefaultInstance()?.AppConfig;

                theme = (config != null && !config.DisableDarkMode && NativeMethods.IsDarkModeEnabled())
                    ? App.Theme.Dark
                    : App.Theme.Light;
            }

            (App.Current as App).SetColorTheme(theme);
        }

        /// <summary>
        /// Choose font size - based on user preference
        /// </summary>
        private static void SetFontSize()
        {
            (App.Current as App).SetFontSize(ConfigurationManager.GetDefaultInstance().AppConfig.FontSize);
        }

        void InitCommandBindings()
        {
            var f1Binding = new CommandBinding(F1Command, OnF1);
            this.CommandBindings.Add(f1Binding);
            var f6binding = new CommandBinding(F6Command, OnF6);
            this.CommandBindings.Add(f6binding);
            var shiftF6binding = new CommandBinding(ShiftF6Command, OnShiftF6);
            this.CommandBindings.Add(shiftF6binding);
            var ClickInspectBinding = new CommandBinding(ClickInspectCommand, btnInspect_Click);
            this.CommandBindings.Add(ClickInspectBinding);
            var ClickTestBinding = new CommandBinding(ClickTestCommand, btnTest_Click);
            this.CommandBindings.Add(ClickTestBinding);
            var ClickColorContrastBinding = new CommandBinding(ClickColorContrastCommand, btnCCA_Click);
            this.CommandBindings.Add(ClickColorContrastBinding);
            var loadFile = new CommandBinding(CtrlOCommand, btnLoad_Click);
            this.CommandBindings.Add(loadFile);
            var saveFile = new CommandBinding(CtrlSCommand, btnSave_Click);
            this.CommandBindings.Add(saveFile);
        }

        void InitPanes()
        {
            this.F6Panes = new List<FrameworkElement> { this.NavigationBar, this.ctrlNamedCommandbar, this.gridLayerModes };
            this.ShiftF6Panes = new List<FrameworkElement>(this.F6Panes);
            this.ShiftF6Panes.Reverse();
        }

        /// <summary>
        /// To debug elevated app, Set "AttachAccessibilityInsights" Env Variable and run it.
        /// </summary>
        private static void SupportDebugging()
        {
            if (CommandLineSettings.AttachToDebugger)
            {
                var dlg = new MessageDialog();
                dlg.Message = Properties.Resources.SupportDebuggingDialogMessage;
                dlg.ShowDialog();
            }
        }

        /// <summary>
        /// Set state of highlighter button
        /// </summary>
        /// <param name="enabled"></param>
        internal void SetHighlightBtnState(bool enabled)
        {
            if (enabled)
            {
                this.vmHilighter.State = ButtonState.On;
                AutomationProperties.SetName(btnHilighter, Properties.Resources.btnHilighterAutomationPropertiesNameOn);
            }
            else
            {
                this.vmHilighter.State = ButtonState.Off;
                AutomationProperties.SetName(btnHilighter, Properties.Resources.btnHilighterAutomationPropertiesNameOff);
            }
            ConfigurationManager.GetDefaultInstance().AppConfig.IsHighlighterOn = enabled;
        }

        /// <summary>
        /// Handle OnLoaded Event for Main window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLoaded(object sender, RoutedEventArgs e)
        {
            if (IssueReporter.IsEnabled)
            {
                RestoreConfigurationAsync();
            }
            else
            {
                btnAccountConfig.Visibility = Visibility.Collapsed;
                btnAccountConfig.SetValue(AutomationProperties.NameProperty, Properties.Resources.btnConfigAutomationPropertiesNameNoBugFiling);
            }

            handleWindowStateChange();

            // update version string.
            UpdateVersionString();

            Initialize();

            HandleFileAssociationOpenRequest();
        }

        /// <summary>
        /// it will make Access Key is only working with Alt
        /// </summary>
        private static void InitAccessKeyRule()
        {
            EventManager.RegisterClassHandler(typeof(UIElement),
                                            AccessKeyManager.AccessKeyPressedEvent,
                                            new AccessKeyPressedEventHandler(OnAccessKeyPressed));
        }

        /// <summary>
        /// Access key press handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null && e.Target == null)
            {
                // If Alt key is not pressed - handle the event
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
                {
                    e.Target = null;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Bring Main Window to foreground
        /// </summary>
        public void BringToForeground()
        {
            NativeMethods.SetForegroundWindow(this.hWnd);
        }

        /// <summary>
        /// On close event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onClosed(object sender, EventArgs e)
        {
            lock (_lockObject)
            {
                try
                {
                    Debug.WriteLine(Properties.Resources.onClosedDebugMessage);
                    SystemEvents.UserPreferenceChanging -= SystemEvents_UserPreferenceChanging;

                    ConfigurationManager.GetDefaultInstance().Save();

                    this.ctrlEventMode.ctrlEvents.StopRecordEvents(true);
                    this.isClosed = true;
                    this.ctrlTestMode.Clear();

                    PageTracker.TrackPage(this.CurrentPage, null);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                    Debug.WriteLine(Properties.Resources.onClosedDebugMessage);

                    // close silently since it is the end of process.
                }
#pragma warning restore CA1031 // Do not catch general exception types
                finally
                {
                    Debug.WriteLine(Properties.Resources.onClosedDebugMessage);
                }
            }
        }

        /// <summary>
        /// Clean up things before closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine(Properties.Resources.Window_ClosingDebugMessage);

            try
            {
                AllowFurtherAction = false;
                this.CurrentPage = AppPage.Exit;
                DisableElementSelector();
                this.timerAutoSnap.Dispose();
                this.timerSelector.Dispose();
                this.HotkeyHandler.ClearHotkeys(); // no more hot key handling.

                var layout = ConfigurationManager.GetDefaultInstance().AppLayout;

                if (this.WindowState == WindowState.Maximized)
                {
                    layout.WinState = WindowState.Maximized;
                }
                else
                {
                    layout.Width = this.RestoreBounds.Width;
                    layout.Height = this.RestoreBounds.Height;
                    layout.Top = this.RestoreBounds.Top;
                    layout.Left = this.RestoreBounds.Left;
                    if (this.WindowState == WindowState.Normal)
                    {
                        layout.WinState = WindowState.Normal;
                    }
                }
                this.ctrlCurMode?.UpdateConfigWithSize();

                HandleExitMode();
                // remove SelectAction
                SelectAction.ClearDefaultInstance();
                HollowHighlightDriver.ClearAllHighlighters();
                ImageOverlayDriver.ClearDefaultInstance();
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                WindowsEventLogger.WriteLogEntry(EventLogEntryType.Warning, Properties.Resources.Window_ClosingException, ex.ToString());
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Debug.WriteLine(Properties.Resources.Window_ClosingDebugMessage);
            }
        }

        private void OnF1(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(HelpDocLink));
        }

        private void OnF6(object sender, ExecutedRoutedEventArgs e)
        {
            var nextPane = GetNextPane(e.Parameter as FrameworkElement);
            MoveFocusToFrameworkElement(nextPane);
        }

        private void OnShiftF6(object sender, ExecutedRoutedEventArgs e)
        {
            var nextPane = GetPriorPane(e.Parameter as FrameworkElement);
            MoveFocusToFrameworkElement(nextPane);
        }

        /// <summary>
        /// Used when getting the next pane for F6 navigation
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private FrameworkElement GetNextPane(FrameworkElement e)
        {
            var nextPane = GetSubsequentFrameworkElement(e, this.F6Panes);

            if (nextPane != this.gridLayerModes)
                return nextPane;

            // if config pane is open, let it get focus
            if (gridlayerConfig.Visibility == Visibility.Visible)
                return nextPane;

            var subPaneNavigation = this.ctrlCurMode as ISupportInnerF6Navigation;
            if (subPaneNavigation == null)
                return nextPane;

            var subPane = subPaneNavigation.GetFirstPane();
            return subPane ?? nextPane;
        }

        /// <summary>
        /// Used to get the prior pane for F6 navigation
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private FrameworkElement GetPriorPane(FrameworkElement e)
        {
            var nextPane = GetSubsequentFrameworkElement(e, this.ShiftF6Panes);

            if (nextPane != this.gridLayerModes)
                return nextPane;

            var subPaneNavigation = this.ctrlCurMode as ISupportInnerF6Navigation;
            if (subPaneNavigation == null)
                return nextPane;

            var subPane = subPaneNavigation.GetLastPane();
            return subPane ?? nextPane;
        }

        #region Title bar

        /// <summary>
        /// Help button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo(HelpDocLink));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && !(Keyboard.FocusedElement is ListViewItem lvi && !(lvi.DataContext is ScanListViewItemViewModel)))
            {
                ctrlCurMode.CopyToClipboard();
            }
            else
            {
                e.Handled = false;
            }
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        /// <summary>
        /// Update related UI when switching between maximized and normal window mode
        /// </summary>
        private void handleWindowStateChange()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.dockPanelMain.Margin = new Thickness(5);
                ctrlCurMode?.UpdateConfigWithSize();
                this.btnMaxFabric.GlyphName = FabricIcon.ChromeRestore;
            }
            else
            {
                this.dockPanelMain.Margin = new Thickness(0);
                this.btnMaxFabric.GlyphName = FabricIcon.Checkbox;
            }
        }

        /// <summary>
        /// Saves user's non-maximized window data when window is maximized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStateChanged(object sender, EventArgs e)
        {
            handleWindowStateChange();
        }

        /// <summary>
        /// Handles close button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(Properties.Resources.btnClose_ClickDebugMessage);
            this.Close();

            Debug.WriteLine(Properties.Resources.btnClose_ClickDebugMessage);

        }

        /// <summary>
        /// Handles maximize button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMax_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Handles minimize button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Replicate title bar icon click behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                IsSystemMenuOpen = true;
                SystemCommands.ShowSystemMenu(this, new Point(Left, Top + this.borderTitleBar.Height));
            }
            else
            {
                if (e.ClickCount == 1)
                {
                    if (IsSystemMenuOpen)
                    {
                        IsSystemMenuOpen = false;
                    }
                    else
                    {
                        IsSystemMenuOpen = true;
                        SystemCommands.ShowSystemMenu(this, new Point(Left, Top + this.borderTitleBar.Height));
                    }
                }
                else if (e.ClickCount == 2)
                {
                    this.Close();
                }
            }
        }

        /// <summary>
        /// Let user reopen title bar menu if they clicked elsewhere
        /// and move back into icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            IsSystemMenuOpen = false;
        }

        #endregion

        #region LeftNavButtons
        /// <summary>
        /// Event handler for Inspect Tab button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInspect_Click(object sender, RoutedEventArgs e)
        {
            HandleInspectTabClick();
        }

        /// <summary>
        /// Event handler for Test Tab button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            HandleTestTabClick();
        }

        private void btnCCA_Click(object sender, RoutedEventArgs e)
        {
            HandleCCATabClick();
        }

        /// <summary>
        /// Open config dialog. Update based on changes if user presses ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onbtnConfigClicked(object sender, RoutedEventArgs e)
        {
            HandleConfigurationModeStart(false);
        }

        /// <summary>
        /// Button handler for clicking "login"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAccountConfig_Click(object sender, RoutedEventArgs e)
        {
            HandleConfigurationModeStart(true);
        }

        /// <summary>
        /// Set saved issue reporter and try restoring configuration if it exists.
        /// </summary>
        private async void RestoreConfigurationAsync()
        {
            try
            {
                var appConfig = ConfigurationManager.GetDefaultInstance().AppConfig;
                var selectedIssueReporterGuid = appConfig.SelectedIssueReporter;
                if (selectedIssueReporterGuid != Guid.Empty)
                {
                    IssueReporterManager.GetInstance().SetIssueReporter(selectedIssueReporterGuid);
                    IssueReporter.SetConfigurationPath(ConfigurationManager.GetDefaultInstance().SettingsProvider.ConfigurationFolderPath);
                    var serializedConfigsDict = appConfig.IssueReporterSerializedConfigs;
                    if (serializedConfigsDict != null)
                    {
                        Dictionary<Guid, string> configsDictionary = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigsDict);
                        if (configsDictionary != null)
                        {
                            configsDictionary.TryGetValue(selectedIssueReporterGuid, out string serializedConfig);
                            await IssueReporter.RestoreConfigurationAsync(serializedConfig).ConfigureAwait(true);
                            Dispatcher.Invoke(UpdateMainWindowConnectionFields);
                        }
                    }
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Update sign in logo and tooltip
        /// </summary>
        internal void UpdateMainWindowConnectionFields()
        {
            bool isConfigured = IssueReporter.IssueReporting != null && IssueReporter.IsConnected;
            string fabricIconName = IssueReporter.Logo.ToString("g");
            fabricIconName = int.TryParse(fabricIconName, out int invalidLogo) ? ReporterFabricIcon.PlugConnected.ToString("g") : fabricIconName;

            // Main window UI changes
            vmReporterLogo.FabricIconLogoName = isConfigured ? fabricIconName : null;
            string tooltipResource = isConfigured ? Properties.Resources.UpdateMainWindowLoginFieldsSignedInAs : Properties.Resources.HandleLogoutRequestSignIn;
            string tooltipText = string.Format(CultureInfo.InvariantCulture, tooltipResource, IssueReporter.DisplayName);
            AutomationProperties.SetName(btnAccountConfig, tooltipText);
            btnAccountConfig.ToolTip = new ToolTip() { Content = tooltipText };
        }

        #endregion

        #region Commandbar

        /// <summary>
        /// Take user to first item inside of command bar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spCommandBar_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (e.NewFocus == sender)
            {
                spCommandBar.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles highlighter button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onHilightButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!IsCapturingData())
            {
                SetHighlightBtnState(ctrlCurMode.ToggleHighlighter());
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCapturingData())
            {
                this.ctrlCurMode?.Refresh();
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCapturingData())
            {
                HandlePauseButtonToggle(this.ToggleUIATreePlayerState);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.ctrlCurMode.IsSaveEnabled && !IsCapturingData())
            {
                this.ctrlCurMode.Save();
            }
        }

        /// <summary>
        /// From IControlTreeNavigation Interface, called by TreeNavigator to determine when it is appropriate to handle tree navigation commands.
        /// </summary>
        /// <returns></returns>
        public bool IsTreeNavigationAllowed()
        {
            if (this.CurrentPage != AppPage.Inspect)
                return false;

            return (InspectView)this.CurrentView == InspectView.Live;
        }

        /// <summary>
        /// snap to process when selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /// ignore this step if the combobox is being initialized
            if (e.RemovedItems.Count != 0)
            {
                var scope = this.cbiEntireApp.IsSelected ? Axe.Windows.Actions.Enums.SelectionScope.App : Axe.Windows.Actions.Enums.SelectionScope.Element;
                SelectAction.GetDefaultInstance().Scope = scope;
                ConfigurationManager.GetDefaultInstance().AppConfig.IsUnderElementScope = (scope == Axe.Windows.Actions.Enums.SelectionScope.Element);
                Logger.PublishTelemetryEvent(Misc.TelemetryEventFactory.ForSetScope(scope.ToString()));
                SelectAction.GetDefaultInstance().ClearSelectedContext();
                ctrlLiveMode.Clear();
                HollowHighlightDriver.GetDefaultInstance().Clear();
            }
        }

        /// <summary>
        /// Open drop down to populate UIA element for narrator announcement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void processComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Down)
            {
                this.cbSelectionScope.IsDropDownOpen = true;
            }
            else if (e.Key == Key.Left)
            {
                this.cbSelectionScope.MoveFocus(new TraversalRequest(FocusNavigationDirection.Left));
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                this.cbSelectionScope.MoveFocus(new TraversalRequest(FocusNavigationDirection.Right));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Load snapshot when button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (IsCurrentModeAllowingSelection())
            {
                this.DisableElementSelector();

                using (var dlg = new System.Windows.Forms.OpenFileDialog
                {
                    Title = Properties.Resources.btnLoad_ClickDialogTitle,
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    Filter = FileFilters.A11yFileFilter,
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                    InitialDirectory = ConfigurationManager.GetDefaultInstance().AppConfig.TestReportPath,
                    AutoUpgradeEnabled = !SystemParameters.HighContrast,
                })
                {
                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (!TryOpenFile(dlg.FileName))
                        {
                            MessageDialog.Show(Properties.Resources.StartLoadingSnapshotLoadFileException);
                            this.EnableElementSelector();
                        }
                    }
                    else
                    {
                        this.EnableElementSelector();
                    }
                }
            }
        }

        /// <summary>
        /// Start snapshot timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTimer_Click(object sender, RoutedEventArgs e)
        {
            int sec;
            if (int.TryParse(tbxTimer.Text, out sec))
            {
                sec = Math.Max(sec, 1); // make sure that delay is bigger than 1 seconds.
                this.tbxTimer.Text = sec.ToString(CultureInfo.InvariantCulture); // set the new value back.
                this.StartTimerAutoSnap(sec);
            }
            else
            {
                MessageDialog.Show(Properties.Resources.btnTimer_ClickMessage);
            }
        }

        /// <summary>
        /// Only let user enter numbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxTimer_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
        /// Ensure menu item always knows current seconds input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxTimer_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.miTimer.SetValue(AutomationProperties.NameProperty, Properties.Resources.tbxTimer_TextChangedText1 + " " + tbxTimer.Text + " " + Properties.Resources.tbxTimer_TextChangedText2);
        }

        #endregion

        #region Breadcrumbs
        /// <summary>
        /// Move to first level breadcrumb
        /// Currently, this is always back to selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCrumbOne_Click(object sender, RoutedEventArgs e)
        {
            /// make sure that we are not capturing data.
            if (IsCapturingData() == false)
            {
                switch (this.CurrentPage)
                {
                    case AppPage.Events:
                        if (this.CurrentView == EventsView.Recording)
                        {
                            MessageDialog.Show(Properties.Resources.PleaseMoveBackToLiveDialogMessage);
                        }
                        else
                        {
                            HandlePauseButtonToggle(true);
                            HandleBackToSelectingState();
                        }
                        break;
                    case AppPage.CCA:
                    case AppPage.Test:
                    default:
                        HandlePauseButtonToggle(true);
                        HandleBackToSelectingState();
                        break;
                }
            }
        }

        /// <summary>
        /// Move to second level breadcrumb
        /// Currently, this is always back to test
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCrumbTwo_Click(object sender, RoutedEventArgs e)
        {
            switch (this.CurrentPage)
            {
                case AppPage.Test:
                    switch ((TestView)this.CurrentView)
                    {
                        case TestView.CapturingData:
                        case TestView.AutomatedTestResults:
                        case TestView.TabStop:
                            break;
                        case TestView.ElementDetails:
                        case TestView.ElementHowToFix:
                            HandleTestTabClick();
                            break;
                        default:
                            HandleBackToSelectingState();
                            break;
                    }
                    break;
                case AppPage.Events:
                    if (this.CurrentView == EventsView.Recording)
                    {
                        MessageDialog.Show(Properties.Resources.PleaseMoveBackToLiveDialogMessage);
                    }
                    else
                    {
                        HandleBackToSelectingState();
                    }
                    break;
                default:
                    HandleBackToSelectingState();
                    break;
            }
        }

        #endregion
    }
}
