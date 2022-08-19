// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions;
using Axe.Windows.Core.Bases;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Automation;

namespace AccessibilityInsights
{
    /// <summary>
    /// this is partial class for MainWindow
    /// this portion will contain State machine logic
    /// </summary>
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    public partial class MainWindow
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        /// <summary>
        /// Current Page variable to keep track of state.
        /// - Startup
        /// - Inspect
        /// - Test
        /// </summary>
        public AppPage CurrentPage { get; set; } = AppPage.Start;

        /// <summary>
        /// Current view in current page.
        /// e.g.
        /// Inspect
        ///  . Live
        ///  . TestDetail
        ///  . Test
        /// </summary>
        public dynamic CurrentView { get; set; }

        /// <summary>
        /// Current mode control
        /// </summary>
        IModeControl ctrlCurMode;

        /// <summary>
        /// Handle Target Selection Change
        /// </summary>
        /// <param name="ec"></param>
        private void HandleTargetSelectionChanged()
        {
            if (this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live)
            {
                StartInspectMode(InspectView.Live);
                UpdateTabSelection();
                UpdateTitleString();
            }

            if (this.CurrentPage == AppPage.CCA && (CCAView)this.CurrentView == CCAView.Automatic)
            {
                StartCCAMode((CCAView)this.CurrentView);
            }
        }

        /// <summary>
        /// Handle Test request by hotkey (Shift + F8)
        /// it is due by HotKey.
        /// </summary>
        void HandleRunTestsByHotkey()
        {
            if (CurrentPage != AppPage.Start && CurrentPage != AppPage.Exit && IsInSelectingState())
            {
                HandleSnapshotRequest(TestRequestSources.HotKey);
            }
        }

        /// <summary>
        /// Handle Recording Request by Hotkey (Shift + F7)
        /// </summary>
        void HandleRequestRecordingByHotkey()
        {
            if (this.CurrentPage == AppPage.Test && (TestView)this.CurrentView == TestView.TabStop)
            {
                ctrlTestMode.ctrlTabStop.ToggleRecording();
            }
            else if (this.CurrentPage == AppPage.Events)
            {
                ctrlEventMode.ctrlEvents.ToggleRecording();
            }
            else if (this.IsInSelectingState())
            {
                var sa = SelectAction.GetDefaultInstance();

                if (sa.HasPOIElement())
                {
                    var poi = GetDataAction.GetElementContext(sa.SelectedElementContextId.Value);
                    this.StartEventsMode(poi.Element);
                    UpdateMainWindowUI();
                }
                else
                {
                    MessageDialog.Show(Properties.Resources.HandleRequestRecordingByHotkeySelectElementMessage);
                }
            }
        }

        /// <summary>
        /// Handle Mode Change request due to snapshot timer
        /// </summary>
        void HandleModeChangeRequestByTimer()
        {
            if (IsInSelectingState())
            {
                HandleSnapshotRequest(TestRequestSources.Timer);
            }
        }

        /// <summary>
        /// Bring app back to selecting State
        /// </summary>
        public void HandleBackToSelectingState()
        {
            // if coming from startup, restore left nav bar
            if (this.CurrentPage == AppPage.Start)
            {
                this.bdLeftNav.IsEnabled = true;
                this.gdModes.Visibility = Visibility.Visible;
                this.btnPause.Visibility = Visibility.Visible;
            }

            // clear selected element highlighter
            HollowHighlightDriver.GetInstance(HighlighterType.Selected).Clear();
            ImageOverlayDriver.ClearDefaultInstance();

            // this can be disabled if previous action was loading old data format.
            // bring it back to enabled state.
            this.btnHilighter.IsEnabled = true;

            // since we comes back to live mode, enable highlighter by default.
            SelectAction.GetDefaultInstance().ClearSelectedContext();

            // turn highlighter on once you get back to selection mode.
            SetHighlightBtnState(true);
            var highlightDriver = HollowHighlightDriver.GetDefaultInstance();
            highlightDriver.HighlighterMode = ConfigurationManager.GetDefaultInstance().AppConfig.HighlighterMode;
            highlightDriver.IsEnabled = true;
            highlightDriver.SetElement(null);

            /// make sure that all Mode UIs are clean since new selection will be done.
            CleanUpAllModeUIs();

#pragma warning disable CA1508 // Dead code warning doesn't apply here
            if (this.CurrentPage == AppPage.Start
                || (this.CurrentPage == AppPage.CCA)
                || (this.CurrentPage == AppPage.Test)
                || (this.CurrentPage == AppPage.Inspect && this.CurrentView != InspectView.Live)
                || (this.CurrentPage == AppPage.Events)
                || (this.CurrentPage == AppPage.Inspect && SelectAction.GetDefaultInstance().IsPaused))
            {
                SelectAction.GetDefaultInstance().Scope = ConfigurationManager.GetDefaultInstance().AppConfig.IsUnderElementScope ? Axe.Windows.Actions.Enums.SelectionScope.Element : Axe.Windows.Actions.Enums.SelectionScope.App;
                cbSelectionScope.SelectedIndex = (SelectAction.GetDefaultInstance().Scope == Axe.Windows.Actions.Enums.SelectionScope.Element) ? 0 : 1;
                StartInspectMode(InspectView.Live); // clean up data when we get back to selection mode.
                this.CurrentPage = AppPage.Inspect;
                this.CurrentView = InspectView.Live;
            }
#pragma warning restore CA1508 // Dead code warning doesn't apply here

            // garbage collection for any UI elements
            GC.Collect();

            // enable element selector
            EnableElementSelector();

            // if it was open when the switch back button is clicked.
            HideConfigurationMode();

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Make sure that Live mode UI is clean.
        /// </summary>
        private void CleanUpAllModeUIs()
        {
            this.ctrlEventMode.ctrlEvents.StopRecordEvents();
            this.ctrlEventMode.Clear();
            this.ctrlLiveMode.Clear();
            this.ctrlTestMode.Clear();
            this.ctrlSnapMode.Clear();
        }

        /// <summary>
        /// Checking whether app is still in Selecting an element or not.
        /// Old live mode
        /// </summary>
        /// <returns></returns>
        private bool IsInSelectingState()
        {
            return (this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live);
        }

        /// <summary>
        /// indicate the type of TestRequestSources
        /// </summary>
        internal enum TestRequestSources
        {
            HotKey,
            Beaker,
            HierarchyNode,
            Timer,
        }

        /// <summary>
        /// When snapshot is invoked, Request appropriate mode change.
        /// </summary>
        /// <param name="method">by which way : Beaker or Hotkey</param>
        internal void HandleSnapshotRequest(TestRequestSources method)
        {
            if (this.CurrentPage == AppPage.Inspect && this.CurrentView == InspectView.Live)
            {
                if (ctrlLiveMode.SelectedInHierarchyElement != null)
                {
                    if (ctrlLiveMode.SelectedInHierarchyElement.Item2 == 0)
                    {
                        SetDataAction.ReleaseDataContext(SelectAction.GetDefaultInstance().SelectedElementContextId.Value);
                    }
                    else
                    {
                        var sa = SelectAction.GetDefaultInstance();
                        sa.SetCandidateElement(ctrlLiveMode.SelectedInHierarchyElement.Item1, ctrlLiveMode.SelectedInHierarchyElement.Item2);
                        sa.Select();
                    }
                }

                // Based on UX model feedback from PM team, we decided to go to AutomatedTestResults as default page view for snapshot.
                StartTestMode(TestView.AutomatedTestResults);

                Logger.PublishTelemetryEvent(TelemetryEventFactory.ForTestRequested(
                    method.ToString(), SelectAction.GetDefaultInstance().Scope.ToString()));
            }
            HollowHighlightDriver.GetDefaultInstance().Clear();
            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle Inspect Tab Click
        /// </summary>
        private void HandleInspectTabClick()
        {
            // make sure that configuration page is closed.
            HideConfigurationMode();

            HandleBackToSelectingState();

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle Test Tab Click
        /// </summary>
        private void HandleTestTabClick()
        {
            // make sure that configuration page is closed.
            HideConfigurationMode();

            switch (this.CurrentPage)
            {
                case AppPage.Inspect:
                    switch ((InspectView)this.CurrentView)
                    {
                        case InspectView.CapturingData:
                            // no op while capturing data.
                            break;
                        default:
                            StartTestMode(TestView.NoSelection);
                            break;
                    }
                    break;
                case AppPage.Test:
                    if ((TestView)this.CurrentView == TestView.ElementHowToFix || (TestView)this.CurrentView == TestView.ElementDetails)
                    {
                        StartTestMode(TestView.AutomatedTestResults);
                    }
                    break;
                case AppPage.Events:
                    StartTestMode(TestView.NoSelection);
                    break;
                case AppPage.CCA:
                    StartTestMode(TestView.NoSelection);
                    break;
            }

            UpdateMainWindowUI();
        }

        private void HandleCCATabClick()
        {

            if (SelectAction.GetDefaultInstance().IsPaused)
            {
                HandlePauseButtonToggle(true);
            }

            this.CurrentPage = AppPage.CCA;
            if (ctrlCCAMode.isToggleChecked())
            {
                this.CurrentView = CCAView.Automatic;
            }
            else
            {
                this.CurrentView = CCAView.Manual;
            }

            HideConfigurationMode();
            ctrlCurMode.HideControl();
            ctrlCurMode = ctrlCCAMode;
            ctrlCurMode.ShowControl();

            StartCCAMode((CCAView)this.CurrentView);

            // if it was open when the switch back button is clicked.
            HideConfigurationMode();

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Delegate type for methods to load given file
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="selectedElementId">ElementId to select after loading. May not be applicable.</param>
        delegate void FileLoadHandler(string path, int? selectedElementId = null);

        /// <summary>
        /// Handle Load snapshot data and Request UX Change
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedEelement"></param>
        internal void HandleLoadingSnapshotData(string path, int? selectedElementId = null)
        {
            HandlePauseButtonToggle(true);

            StartLoadingSnapshot(path, selectedElementId);
            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle Load snapshot data and Request UX Change
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedEelement">Not used</param>
        private void HandleLoadingEventData(string fileName, int? selectedElementId = null)
        {
            HandlePauseButtonToggle(true);

            StartEventsWithLoadedData(fileName);
            Logger.PublishTelemetryEvent(TelemetryAction.Event_Load);

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Attempts to open a file as either a test or events file
        /// </summary>
        /// <returns>true if file opened successfully; otherwise, false.</returns>
        private bool TryOpenFile(string fileName, int? selectedElementId = null)
        {
            // array of file handlers to be attempted in order. If one throws an exception, the next will be tried
            FileLoadHandler[] fileHandlers = { HandleLoadingSnapshotData, HandleLoadingEventData };

            foreach (var fileHandler in fileHandlers)
            {
                try
                {
                    fileHandler(fileName, selectedElementId);
                    return true;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return false;
        }

        /// <summary>
        /// Select an element in Snapshot from Test Mode(Automated Check) to show "How to fix" info.
        /// </summary>
        internal void HandleSelectElementToShowHowToFix()
        {
            StartTestMode(TestView.ElementHowToFix);

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Switch to test results in UIA tree, run tests on selected element, then open file bug window
        /// </summary>
        internal void HandleFileBugLiveMode()
        {
            HandlePauseButtonToggle(true);

            if (ctrlLiveMode.SelectedInHierarchyElement != null)
            {
                if (ctrlLiveMode.SelectedInHierarchyElement.Item2 == 0)
                {
                    SetDataAction.ReleaseDataContext(SelectAction.GetDefaultInstance().SelectedElementContextId.Value);
                }
                else
                {
                    var sa = SelectAction.GetDefaultInstance();
                    sa.SetCandidateElement(ctrlLiveMode.SelectedInHierarchyElement.Item1, ctrlLiveMode.SelectedInHierarchyElement.Item2);
                    sa.Select();
                }
            }

            StartElementHowToFixView(() =>
            {
                Dispatcher.Invoke(() => ctrlSnapMode.ctrlHierarchy.FileBug());
            });

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Bring back to Inspect StartUp
        /// </summary>
        internal void HandleBackToLiveFromEventPage()
        {
            StartInspectMode(InspectView.Live);
            HollowHighlightDriver.GetInstance(HighlighterType.Selected).Clear();
            HollowHighlightDriver.GetDefaultInstance().IsEnabled = ConfigurationManager.GetDefaultInstance().AppConfig.IsHighlighterOn;

            UpdateMainWindowUI();
        }

        /// <summary>
        /// Move from Live to Events mode
        /// </summary>
        /// <param name="selectedElementId"></param>
        internal void HandleLiveToEvents(A11yElement e)
        {
            StartEventsMode(e);
            UpdateMainWindowUI();
        }

        /// <summary>
        /// Handle End of app
        /// </summary>
        private void HandleExitMode()
        {
            this.CurrentPage = AppPage.Exit;
        }

        /// <summary>
        /// check whether current mode allow selection
        /// </summary>
        /// <returns></returns>
        private bool IsCurrentModeAllowingSelection()
        {
            return (this.CurrentPage == AppPage.Inspect && (InspectView)this.CurrentView == InspectView.Live);
        }

        /// <summary>
        /// Handle File open request
        /// if it was based on double clicking.  open file.
        /// </summary>
        /// <return>true if file open was handled. otherwise, false.</return>
        private bool HandleFileAssociationOpenRequest()
        {
            try
            {
                var path = CommandLineSettings.FileToOpen;
                if (path != null)
                {
                    return HandleFileDiskOpen(path, null);
                }
                else
                {
                    return false;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Handle File open request from disk if it was based on double clicking,
        /// or the temp file saved after downloading the file from the client UI
        /// </summary>
        /// <return>true if file open was handled. otherwise, false.</return>
        private bool HandleFileDiskOpen(string path, int? selectedElementId = null)
        {
            if (path != null)
            {
                this.bdLeftNav.IsEnabled = true;
                this.gdModes.Visibility = Visibility.Visible;

                return TryOpenFile(path, selectedElementId);
            }

            return false;
        }

        /// <summary>
        /// Handle configuration changed
        /// - apply changes as needed
        /// - Send telemetry
        /// </summary>
        /// <param name="testconfig"></param>
        internal void HandleConfigurationChanged(IReadOnlyDictionary<string, object> changes)
        {
            HotkeyHandler?.ClearHotkeys();
            InitHotKeys();

            var configManager = ConfigurationManager.GetDefaultInstance();

            SelectAction.GetDefaultInstance().IntervalMouseSelector = configManager.AppConfig.MouseSelectionDelayMilliSeconds;
            this.Topmost = configManager.AppConfig.AlwaysOnTop;
            this.ctrlTestMode.ctrlTabStop.SetHotkeyText(configManager.AppConfig.HotKeyForRecord);

            HollowHighlightDriver.GetDefaultInstance().HighlighterMode = configManager.AppConfig.HighlighterMode;

            if (changes != null && changes.ContainsKey(ConfigurationModel.KeyFontSize))
            {
                SetFontSize();
            }
        }

        internal void TransitionToSelectActionMode()
        {
            InitSelectActionMode();
            HideConfigurationMode();
            UpdateMainWindowUI();
            this.btnConfig.Focus();
        }

        /// <summary>
        /// Hide(close) configuration mode
        /// </summary>
        internal void HideConfigurationMode()
        {
            if (this.gridlayerConfig.Visibility == Visibility.Visible)
            {
                this.gridlayerConfig.Visibility = Visibility.Collapsed;
                this.spBreadcrumbs.Visibility = Visibility.Visible;
                this.ctrlCurMode.ShowControl();
                this.AllowFurtherAction = true;
                EnableElementSelector();
            }
        }

        /// <summary>
        /// Start configuration mode with connection screen
        /// </summary>
        internal void HandleConnectionConfigurationStart()
        {
            HandleConfigurationModeStart(true);
        }

        /// <summary>
        /// Start Configuration mode.
        /// - if connection is true, routes to connection configuration
        /// </summary>
        private void HandleConfigurationModeStart(bool connection)
        {
            this.AllowFurtherAction = false;
            this.ctrlCurMode.HideControl();
            DisableElementSelector();
            this.ctrlConfigurationMode.ShowControl(connection);

            gridLayerModes.Children.Remove(gridlayerConfig);
            gridLayerModes.Children.Add(gridlayerConfig);
            this.spBreadcrumbs.Visibility = Visibility.Collapsed;
            this.gridlayerConfig.Visibility = Visibility.Visible;
            this.btnPause.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///  check whether we are curring capturing data or not.
        /// </summary>
        /// <returns></returns>
        internal bool IsCapturingData()
        {
            return (CurrentPage == AppPage.Inspect && ((InspectView)CurrentView) == InspectView.CapturingData) ||
                (CurrentPage == AppPage.Test && ((TestView)CurrentView) == TestView.CapturingData) ||
                (CurrentPage == AppPage.CCA && ((CCAView)CurrentView) == CCAView.CapturingData);
        }

        /// <summary>
        /// Shows a dialog with an error message and then goes back to selecting state
        /// </summary>
        public void HandleFailedSelectionReset()
        {
            MessageDialog.Show(Properties.Resources.HandleFailedSelectionResetConnectionLostMessage);
            HandleBackToSelectingState();
        }

        /// <summary>
        /// Shows a dialog with an error message and then goes back to selecting state
        /// </summary>
        internal void HandlePauseButtonToggle(bool enabled)
        {
            if (enabled)
            {
                this.vmLiveModePauseResume.State = ButtonState.On;
                SelectAction.GetDefaultInstance().ResumeUIATreeTracker();
                AutomationProperties.SetName(btnPause, Properties.Resources.btnPauseAutomationPropertiesNameOn);
            }
            else
            {
                this.vmLiveModePauseResume.State = ButtonState.Off;
                SelectAction.GetDefaultInstance().PauseUIATreeTracker();
                AutomationProperties.SetName(btnPause, Properties.Resources.btnPauseAutomationPropertiesNameOff);
            }
            UpdateMainWindowUI();
        }
    }
}
