// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.FileBug;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using AccessibilityInsights.Win32;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using static AccessibilityInsights.Misc.FrameworkNavigator;
using AccessibilityInsights.Properties;
using System.Web;
using Newtonsoft.Json;

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
        const string ArgAttach = "AttachAccessibilityInsights";

        IntPtr hWnd;
        private bool isClosed = false;

        public TwoStateButtonViewModel vmHilighter { get; private set; } = new TwoStateButtonViewModel(ButtonState.On);
        public TwoStateButtonViewModel vmLiveModePauseResume { get; private set; } = new TwoStateButtonViewModel(ButtonState.On);

        public ByteArrayViewModel vmAvatar { get; private set; } = new ByteArrayViewModel();

        public bool IsEventRecording { get; private set; }

        /// <summary>
        /// Whether or not the File Bug button should be made visible
        /// </summary>
        public static Visibility BugColumnVisibility => HelperMethods.GeneralFileBugVisibility;

        /// <summary>
        /// The width of the Bug column in the data grid (0 if bug filing isn't available)
        /// </summary>
        public static double BugColumnWidth => HelperMethods.GeneralFileBugVisibility == Visibility.Visible ? 80 : 0;

        /// <summary>
        /// Internal bool to ignore or allow updating logic
        /// </summary>
        private bool IgnoreUpdates = false;


        /// <summary>
        /// Is title bar context menu open
        /// </summary>
        bool IsSystemMenuOpen = false;

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
                return string.Format(CultureInfo.InvariantCulture, Properties.Resources.MainWindow_AutomationPropertiesName_0_1_2, Properties.Resources.btnCCAAutomationPropertiesName , this.CurrentPage.ToString(), Properties.Resources.ModeIsEnabled);
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
        /// <param name="allow"></param>
        public void SetAllowFurtherAction(bool allow)
        {
            lock (this)
            {
                if (AllowFurtherAction != allow && IsCurrentModeAllowingSelection())
                {
                    AllowFurtherAction = allow;
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
        private List<FrameworkElement> F6Panes = null;
        private List<FrameworkElement> ShiftF6Panes = null;

        public MainWindow()
        {
            SystemEvents.UserPreferenceChanging += SystemEvents_UserPreferenceChanging;
            Logger.AttachReportExceptionHandler();

            SetHighContrastTheme();

            // create necessary config folders & their internal config files
            PopulateConfigurations();

            SetFontSize();

            InitCommandBindings();

            InitializeComponent();

            this.Topmost = ConfigurationManager.GetDefaultInstance().AppConfig.AlwaysOnTop;
            ///in case we need to do any debugging with elevated app
            SupportDebugging();

            UpdateMainWindowLayout();

            this.TreeNavigator = new TreeNavigator(this);
            this.TreeNavigator.SelectionChanged += this.HandleTargetSelectionChanged;

            InitTelemetry();

            InitPanes();
        }

        /// <summary>
        /// Set up some context properties like installation id, app version, session id
        /// </summary>
        private static void InitTelemetry()
        {
            // Initialize user info from file if it exists, reset if needed, and re-serialize
            var installInfo = InstallationInfo.LoadFromPath(DirectoryManagement.sConfigurationFolderPath);
            Logger.AddOrUpdateContextProperty(TelemetryProperty.InstallationID, installInfo.InstallationGuid.ToString());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.Version, AccessibilityInsights.Core.Misc.Utility.GetAppVersion());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.AppSessionID, Guid.NewGuid().ToString());
            Logger.AddOrUpdateContextProperty(TelemetryProperty.SessionType, "Desktop");
            Logger.PublishTelemetryEvent(TelemetryAction.Mainwindow_Startup, null);
        }

        /// <summary>
        /// Listen to UserPreferenceChange to switch to high contrast theme when appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SystemEvents_UserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Color || e.Category == UserPreferenceCategory.VisualStyle)
            {
                SetHighContrastTheme();
            }
        }

        /// <summary>
        /// Choose theme based on system theme
        /// </summary>
        private static void SetHighContrastTheme()
        {
            (App.Current as App).SetColorTheme(SystemParameters.HighContrast ? App.Theme.HighContrast : App.Theme.Light);
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
            var args = Environment.GetCommandLineArgs();

            if (args.Contains(ArgAttach))
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
            if (BugReporter.IsEnabled)
            {
                //ConnectToSavedServerConnection();
                RestoreConfigurationAsync();
            }
            else
            {
                btnAccountConfig.Visibility = Visibility.Collapsed;
                btnAccountConfig.SetValue(AutomationProperties.NameProperty, Properties.Resources.btnConfigAutomationPropertiesNameNoBugFiling);
            }

            // update version string. 
            UpdateVersionString();

            StartStartMode();

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
            lock (this)
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
                catch
                {
                    Debug.WriteLine(Properties.Resources.onClosedDebugMessage);

                    // close silently since it is the end of process. 
                }
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
                this.HotkeyHandler.Dispose(); // no more hot key handling. 

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
                HighlightAction.ClearAllHighlighters();
                HighlightImageAction.ClearDefaultInstance();
            }
            catch (Exception ex)
            {
                WindowsEventLogger.WriteLogEntry(EventLogEntryType.Warning, Properties.Resources.Window_ClosingException, ex.ToString());
            }
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
            if (gridlayerConfig.Visibility == Visibility.Visible) return nextPane;

            var subPaneNavigation = this.ctrlCurMode as ISupportInnerF6Navigation;
            if (subPaneNavigation == null) return nextPane;

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
            if (subPaneNavigation == null) return nextPane;

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

        private void Window_KeyUp(object sender, KeyEventArgs e)
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
        /// Saves user's non-maximized window data when window is maximized
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onStateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.dockPanelMain.Margin = new Thickness(5);
                ctrlCurMode?.UpdateConfigWithSize();
            }
            else
            {
                this.dockPanelMain.Margin = new Thickness(0);
            }
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
            if (this.gridlayerConfig.Visibility == Visibility.Visible)
            {
                HideConfigurationMode();
                UpdateMainWindowUI();
                if (this.CurrentPage == AppPage.Inspect)
                {
                    HandleBackToSelectingState();
                }
            }
            else
            {
                HandleConfigurationModeStart(false);
            }
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

        ///// <summary>
        ///// Initialize server integration and try logging in implicitly 
        ///// to the saved connection in the configuration if it exists.
        ///// </summary>
        //private void ConnectToSavedServerConnection(Action callback = null)
        //{
        //    var oldConnection = ConfigurationManager.GetDefaultInstance().AppConfig.SavedConnection;
        //    if (oldConnection?.ServerUri != null)
        //    {
        //        HandleLoginRequest(oldConnection.ServerUri, false, callback);
        //    }
        //}

        /// <summary>
        /// Initialize server integration and try logging in implicitly 
        /// to the saved connection in the configuration if it exists.
        /// </summary>
        private static void RestoreConfigurationAsync(/*Action callback = null*/)
        {
            var appConfig = ConfigurationManager.GetDefaultInstance().AppConfig;
            var selectedIssueReporterGuid = appConfig.SelectedIssueReporter;
            if (selectedIssueReporterGuid != null)
            {
                IssueReporterManager.GetInstance().SetIssueReporter(selectedIssueReporterGuid);
                var serializedConfigsDict = appConfig.IssueReporterSerializedConfigs;
                Dictionary<Guid, string> configsDictionary = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigsDict);
                configsDictionary.TryGetValue(selectedIssueReporterGuid, out string serializedConfig);
                BugReporter.RestoreConfigurationAsync(serializedConfig);
                //HandleLoginRequest(null, false, null);
            }
            //Dictionary<string, string> configs = new Dictionary<string, string>() { { "Ashwin", "hello world"} };
            //string serializedConfig = JsonConvert.SerializeObject(configs);
            //callback?.Invoke();
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
            if (this.CurrentPage != AppPage.Inspect) return false;

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
                var scope = this.cbiEntireApp.IsSelected ? AccessibilityInsights.Actions.Enums.SelectionScope.App : AccessibilityInsights.Actions.Enums.SelectionScope.Element;
                SelectAction.GetDefaultInstance().Scope = scope;
                ConfigurationManager.GetDefaultInstance().AppConfig.IsUnderElementScope = (scope == AccessibilityInsights.Actions.Enums.SelectionScope.Element);
                Logger.PublishTelemetryEvent(TelemetryAction.TestSelection_Set_Scope, TelemetryProperty.Scope, scope.ToString());
                SelectAction.GetDefaultInstance().ClearSelectedContext();
                HighlightAction.GetDefaultInstance().Clear();
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

                var dlg = new System.Windows.Forms.OpenFileDialog
                {
                    Title = Properties.Resources.btnLoad_ClickDialogTitle,
                    Filter = FileFilters.A11yFileFilter,
                    InitialDirectory = ConfigurationManager.GetDefaultInstance().AppConfig.TestReportPath,
                    AutoUpgradeEnabled = !SystemParameters.HighContrast,
                };

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
