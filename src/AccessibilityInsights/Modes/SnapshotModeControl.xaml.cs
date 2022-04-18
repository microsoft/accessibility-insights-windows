// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Actions.Enums;
using Axe.Windows.Actions.Misc;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.Settings;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for SnapshotModeControl.xaml
    /// </summary>
    public partial class SnapshotModeControl : UserControl, IModeControl, IHierarchyAction
    {
        /// <summary>
        /// Element Context
        /// </summary>
        public ElementContext ElementContext { get; private set; }

        ///// <summary>
        ///// Current mode of control
        ///// </summary>
        //public SnapshotModes Mode { get; set; }

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
        /// Indicate how to do the data context population.
        /// Live/Snapshot/Load
        /// </summary>
        public DataContextMode DataContextMode { get; set; } = DataContextMode.Test;

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SnapshotModeControl()
        {
            InitializeComponent();
            this.ctrlHierarchy.IsLiveMode = false;
            this.ctrlHierarchy.HierarchyActions = this;
            this.ctrlTabs.SwitchToServerLogin = MainWin.HandleConnectionConfigurationStart;
        }

        /// <summary>
        /// Event handler when node selection is changed in treeview.
        /// </summary>
        public void SelectedElementChanged()
        {
            A11yElement element = this.ctrlHierarchy.SelectedInHierarchyElement;

            // selection only when UI snapshot is done.
            if (element != null && this.IsVisible)
            {
                UpdateElementInfoUI(element);
            }
        }

        /// <summary>
        /// Update Element Info UI(Properties/Patterns/Tests)
        /// </summary>
        /// <param name="ecId"></param>
        public async Task SetElement(Guid ecId)
        {
            this.ctrlProgressRing.Activate();

            EnableMenuButtons(this.DataContextMode != DataContextMode.Load);

            bool selectionFailure = false;

            try
            {
                this.ctrlHierarchy.IsEnabled = false;
                ElementContext ec = null;
                await Task.Run(() =>
                {
                    Stopwatch stopwatch = Stopwatch.StartNew();
                    bool contextChanged = CaptureAction.SetTestModeDataContext(ecId,
                        this.DataContextMode, Configuration.TreeViewMode);
                    ec = GetDataAction.GetElementContext(ecId);

                    if (contextChanged && this.DataContextMode != DataContextMode.Load)
                    {
                        // send telemetry of scan results.
                        var dc = GetDataAction.GetElementDataContext(ecId);
                        dc.PublishScanResults(stopwatch.ElapsedMilliseconds);
                    }
                }).ConfigureAwait(false);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ec != null && ec.DataContext != null)
                    {
                        this.ElementContext = ec;
                        this.ctrlTabs.Clear();
                        if (!SelectAction.GetDefaultInstance().IsPaused)
                        {
                            this.ctrlHierarchy.CleanUpTreeView();
                        }
                        this.ctrlHierarchy.SetElement(ec);
                        this.ctrlTabs.SetElement(ec.Element, false, ec.Id);

                        if (ec.DataContext.Screenshot == null && ec.DataContext.Mode != DataContextMode.Load)
                        {
                            Application.Current.MainWindow.WindowStyle = WindowStyle.ToolWindow;
                            Application.Current.MainWindow.Visibility = Visibility.Hidden;
                            HollowHighlightDriver.GetDefaultInstance().IsEnabled = false;

                            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
                            {
                                ScreenShotAction.CaptureScreenShot(ecId);
                                Application.Current.MainWindow.Visibility = Visibility.Visible;
                                Application.Current.MainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                            })).Wait();
                        }
                        var imageOverlay = ImageOverlayDriver.GetDefaultInstance();

                        imageOverlay.SetImageElement(ecId);
                        //disable button on highlighter.
                        imageOverlay.SetHighlighterButtonClickHandler(null);

                        if (Configuration.IsHighlighterOn)
                        {
                            imageOverlay.Show();
                        }
                    }

                    // if it is enforced to select a specific element(like fastpass click case)
                    if (ec.DataContext.FocusedElementUniqueId.HasValue)
                    {
                        this.ctrlHierarchy.SelectElement(ec.DataContext.FocusedElementUniqueId.Value);
                    }

                    if (!ec.DataContext.Elements.TryGetValue(ec.DataContext.FocusedElementUniqueId ?? 0, out A11yElement selectedElement))
                    {
                        selectedElement = ec.DataContext.Elements[0];
                    }
                    AutomationProperties.SetName(this, string.Format(CultureInfo.InvariantCulture, Properties.Resources.SetElementInspectTestDetail, selectedElement.Glimpse));

                    FireAsyncContentLoadedEvent(AsyncContentLoadedState.Completed);

                    // Set focus on hierarchy tree
                    Dispatcher.InvokeAsync(() => this.ctrlHierarchy.SetFocusOnHierarchyTree(), DispatcherPriority.Input).Wait();

                    ec.DataContext.FocusedElementUniqueId = null;
                });
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Application.Current.MainWindow.Visibility = Visibility.Visible;
                    EnableMenuButtons(false);
                    this.ElementContext = null;
                    selectionFailure = true;
                });
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.ctrlProgressRing.Deactivate();
                    this.ctrlHierarchy.IsEnabled = true;

                    // if focus has gone to the window, we set focus to the hierarchy control. We do this because disabling the hierarchy control
                    // will throw keyboard focus to mainwindow, where it is not very useful.
                    if (Keyboard.FocusedElement is Window)
                    {
                        this.ctrlHierarchy.Focus();
                    }

                    if (selectionFailure)
                    {
                        MainWin.HandleFailedSelectionReset();
                    }
                    else
                    {
                        // Set the right view state :
                        if (MainWin.CurrentView is TestView iv && iv == TestView.ElementHowToFix)
                        {
                            MainWin.SetCurrentViewAndUpdateUI(TestView.ElementHowToFix);
                        }
                        else
                        {
                            MainWin.SetCurrentViewAndUpdateUI(TestView.ElementDetails);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Enable menu buttons
        /// - Save
        /// - Refresh
        /// </summary>
        private void EnableMenuButtons(bool enable)
        {
            this.IsSaveEnabled = enable;
            this.IsRefreshEnabled = enable;
        }

        /// <summary>
        /// Fire Async Content Loaded Event to notify Snapshot mode data load completion.
        /// </summary>
        /// <param name="state"></param>
        private void FireAsyncContentLoadedEvent(AsyncContentLoadedState state)
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.AsyncContentLoaded))
            {
                UserControlAutomationPeer peer = UIElementAutomationPeer.FromElement(this) as UserControlAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(state, state == AsyncContentLoadedState.Beginning ? 0 : 100));
                }
            }
        }

        /// <summary>
        /// Update Element Info UI.
        /// </summary>
        /// <param name="e"></param>
        private void UpdateElementInfoUI(A11yElement e)
        {
            this.ctrlTabs.SetElement(e, false, this.ElementContext.Id);

            ImageOverlayDriver.GetDefaultInstance().SetSingleElement(this.ElementContext.Id, e.UniqueId);
        }

        /// <summary>
        /// Hide control and hilighter
        /// </summary>
        public void HideControl()
        {
            UpdateConfigWithSize();
            ImageOverlayDriver.GetDefaultInstance().ClearElements();
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Show control and hilighter
        /// </summary>
        public void ShowControl()
        {
            AdjustMainWindowSize();
            if (Configuration.IsHighlighterOn)
            {
                ImageOverlayDriver.GetDefaultInstance().Show();
            }
            this.Visibility = Visibility.Visible;
            Dispatcher.InvokeAsync(() =>
            {
                this.SetFocusOnDefaultControl();
            }
            , System.Windows.Threading.DispatcherPriority.Input);

            this.ctrlTabs.CurrentMode = (TestView)(MainWin.CurrentView) == TestView.ElementHowToFix ? InspectTabMode.TestHowToFix : InspectTabMode.TestProperties;
        }

        /// <summary>
        /// Update current view based on tab selection
        /// </summary>
        /// <param name="mode"></param>
        private static void UpdateMainWinView(InspectTabType type)
        {
            if (type == InspectTabType.HowToFix)
            {
                MainWin.SetCurrentViewAndUpdateUI(TestView.ElementHowToFix);
            }
            else if (type == InspectTabType.Details)
            {
                MainWin.SetCurrentViewAndUpdateUI(TestView.ElementDetails);
            }
        }

        /// <summary>
        /// Clear UI
        /// </summary>
        public void Clear()
        {
            this.ElementContext = null;
            ImageOverlayDriver.GetDefaultInstance().Clear();
            this.ctrlHierarchy.Clear();
            this.ctrlTabs.Clear();
        }

        // <summary>
        // Store Window data for Snapshot Mode
        // </summary>
        public void UpdateConfigWithSize()
        {
            CurrentLayout.LayoutSnapshot.ColumnSnapWidth = this.columnSnap.Width.Value;
        }

        // <summary>
        // Updates Window size with stored data and adjusts layout for Snap Mode
        // </summary>
        public void AdjustMainWindowSize()
        {
            MainWin.SizeToContent = SizeToContent.Manual;
            this.columnSnap.Width = new GridLength(CurrentLayout.LayoutSnapshot.ColumnSnapWidth);

            this.ctrlHierarchy.IsLiveMode = false;
            this.gsMid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Copy either selected properties or element data
        /// </summary>
        public void CopyToClipboard()
        {
            StringBuilder sb = new StringBuilder();

            if (Keyboard.FocusedElement is ListViewItem lvi && lvi.DataContext is ScanListViewItemViewModel stvi)
            {
                ListView listView = ItemsControl.ItemsControlFromItemContainer(lvi) as ListView;
                foreach (var item in listView.SelectedItems)
                {
                    var vm = item as ScanListViewItemViewModel;
                    sb.AppendLine(vm.Header);
                    sb.AppendLine(vm.HowToFixText);
                }
            }
            else if (Keyboard.FocusedElement is TextBox tb)
            {
                sb.Append(tb.SelectedText);
            }
            else if (this.ElementContext != null)
            {
                var se = this.ctrlHierarchy.SelectedElement ?? this.ElementContext.Element;

                // glimpse
                sb.AppendFormat(CultureInfo.CurrentCulture, Properties.Resources.SnapshotModeControl_GlimpseFormat, se.Glimpse);
                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine(Properties.Resources.SnapshotModeControl_AvailableProperties);
                // properties
                foreach (var p in se.Properties)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}", p.Value.Name, p.Value.TextValue);
                    sb.AppendLine();
                }

                sb.AppendLine();

                sb.AppendLine(Properties.Resources.SnapshotModeControl_AvailablePatterns);
                // patterns
                foreach (var pt in se.Patterns)
                {
                    sb.Append(pt.Name);
                    sb.AppendLine();
                }
            }
            sb.CopyStringToClipboard();
            sb.Clear();
        }

        /// <summary>
        /// Refresh button is needed on main command bar
        /// if it is load mode(PlatformObject is null), disable it.
        /// </summary>
        public bool IsRefreshEnabled { get; private set; }

        /// <summary>
        /// Save button is neeeded on main command bar
        /// if it is load mode(PlatformObject is null), disable it.
        /// </summary>
        public bool IsSaveEnabled { get; private set; }

        /// <summary>
        /// Refresh Data
        /// it is called from state machine to handle Refresh button under title bar.
        /// </summary>
        public void Refresh()
        {
            RefreshHierarchy(true);
        }

        /// <summary>
        /// Refresh Hierarchy tree
        /// </summary>
        /// <param name="newData">true, get new datacontext</param>
        public void RefreshHierarchy(bool newData)
        {
            // data will be asked to be captured again. so set the current view right.
            MainWin.SetCurrentViewAndUpdateUI(InspectView.CapturingData);

            if (newData)
            {
                ImageOverlayDriver.ClearDefaultInstance();
                var ecId = SelectAction.GetDefaultInstance().SelectedElementContextId.Value;
                SetDataAction.ReleaseDataContext(ecId);
#pragma warning disable CS4014
                // NOTE: We aren't awaiting this async call, so if you
                // touch it, consider if you need to add the await
                this.SetElement(ecId);
#pragma warning restore CS4014
            }
            else
            {
#pragma warning disable CS4014
                // NOTE: We aren't awaiting this async call, so if you
                // touch it, consider if you need to add the await
                this.SetElement(this.ElementContext.Id);
#pragma warning restore CS4014
            }
        }

        /// <summary>
        /// Save snapshot
        /// </summary>
        public void Save()
        {
            using (var dlg = new System.Windows.Forms.SaveFileDialog
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                Filter = FileFilters.TestFileFilter,
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                InitialDirectory = Configuration.TestReportPath,
                AutoUpgradeEnabled = !SystemParameters.HighContrast,
            })
            {
                dlg.FileName = dlg.InitialDirectory.GetSuggestedFileName(FileType.TestResults);

                Logger.PublishTelemetryEvent(TelemetryAction.Hierarchy_Save);

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    try
                    {
                        SaveAction.SaveSnapshotZip(dlg.FileName, this.ElementContext.Id, this.ctrlHierarchy.SelectedElement.UniqueId, A11yFileMode.Inspect);
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (Exception ex)
                    {
                        ex.ReportException();
                        MessageDialog.Show(string.Format(CultureInfo.InvariantCulture, Properties.Resources.SaveException, ex.Message));
                    }
#pragma warning restore CA1031 // Do not catch general exception types
                }
            }
        }

        /// <summary>
        /// Toggle highlight visibility
        /// </summary>
        /// <returns>Updated highlighter visibility</returns>
        public bool ToggleHighlighter()
        {
            var visible = !ImageOverlayDriver.GetDefaultInstance().IsVisible();
            if (visible)
            {
                ImageOverlayDriver.GetDefaultInstance().Show();
                ImageOverlayDriver.GetDefaultInstance().SetSingleElement(this.ElementContext.Id, this.ctrlHierarchy.SelectedInHierarchyElement.UniqueId);
            }
            else
            {
                ImageOverlayDriver.GetDefaultInstance().Hide();
            }
            return visible;
        }

        /// <summary>
        /// Set focus on default control for mode
        /// </summary>
        public void SetFocusOnDefaultControl()
        {
            this.ctrlHierarchy.Focus();
        }

        /// <summary>
        /// Wrapper to switch to server login for IHierarchyAction
        /// </summary>
        public void SwitchToServerLogin()
        {
            MainWin.HandleConnectionConfigurationStart();
        }

        /// <summary>
        /// Wrapper to switch to events mode for IHierarchyAction
        /// </summary>
        public void HandleLiveToEvents(A11yElement el)
        {
            MainWin.HandleLiveToEvents(el);
        }

        /// <summary>
        /// Wrapper to file a bug for IHierarchyAction
        /// </summary>
        public void HandleFileBugLiveMode()
        {
            MainWin.HandleFileBugLiveMode();
        }
    }
}

