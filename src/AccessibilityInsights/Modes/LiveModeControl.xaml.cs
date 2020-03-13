// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for LiveModeControl.xaml
    /// </summary>
    public partial class LiveModeControl : UserControlWithPanes, IModeControl, IHierarchyAction
    {
        private ElementContext ElementContext;

        /// <summary>
        /// Selected element from hierarchy
        /// </summary>
        public Tuple<Guid,int> SelectedInHierarchyElement { get; private set; }

        /// <summary>
        /// Inidate whether need to EnableElementSelector at the hierarchy node Selection change. 
        /// </summary>
        private bool EnableSelectorWhenPOISelectedInHierarchy = false;

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
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, Properties.Resources.LocalizedControlType_Page);
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
        /// Constructor
        /// </summary>
        public LiveModeControl()
        {
            InitializeComponent();
            InitF6Panes(this.HierarchyGrid, this.ctrlTabs);

            this.ctrlHierarchy.HierarchyActions = this;
            this.ctrlHierarchy.SetbtnTestElementHandler(this.btnTest_Click);
            this.ctrlHierarchy.IsLiveMode = true;
        }

        /// <summary>
        /// Refresh hierarchy by clearing data context
        /// </summary>
        /// <param name="newData"></param>
        public void RefreshHierarchy(bool newData)
        {
            if (newData)
            {
                // keep the selected element since it is for refresh. 
                SetDataAction.ReleaseDataContext(ElementContext.Id);
            }

#pragma warning disable CS4014
            // NOTE: We aren't awaiting this async call, so if you
            // touch it, consider if you need to add the await
            this.SetElement(this.ElementContext.Id);
#pragma warning restore CS4014
        }

        /// <summary>
        /// Event handler when node selection is changed in treeview. 
        /// </summary>
        public void SelectedElementChanged()
        {
            if (this.ElementContext != null && GetDataAction.ExistElementContext(this.ElementContext.Id))
            {
                this.SelectedInHierarchyElement = this.ctrlHierarchy.SelectedInHierarchyElement != null ? new Tuple<Guid, int>(this.ElementContext.Id, this.ctrlHierarchy.SelectedInHierarchyElement.UniqueId) : null;

                // selection only when UI snapshot is done. 
                if (this.SelectedInHierarchyElement != null && MainWin.CurrentPage == AppPage.Inspect && (InspectView)MainWin.CurrentView == InspectView.Live)
                {
                    var e = GetDataAction.GetA11yElementWithLiveData(this.SelectedInHierarchyElement.Item1, this.SelectedInHierarchyElement.Item2);
                    HollowHighlightDriver.GetDefaultInstance().SetElement(this.SelectedInHierarchyElement.Item1, this.SelectedInHierarchyElement.Item2);
                    UpdateElementInfoUI(e);
                }
            }
            else
            {
                ClearSelectedItem();
            }

            // NeedToEnableSelector is set True in SetElement
            // it is to make sure that ElementSelector is continued after populating UI
            if (EnableSelectorWhenPOISelectedInHierarchy)
            {
                // enable selector once UI update is finished. 
                UpdateStateMachineForLiveMode();
                EnableSelectorWhenPOISelectedInHierarchy = false;
            }
        }

        /// <summary>
        /// Clear the selected item, without showing any warning
        /// </summary>
        private void ClearSelectedItem()
        {
            Clear();
            SelectAction.GetDefaultInstance().ClearSelectedContext();
            HollowHighlightDriver.GetDefaultInstance().Clear();
        }

        /// <summary>
        /// Update selected element
        /// </summary>
        /// <param name="ecId">Element Context Id</param>
        public async Task SetElement(Guid ecId)
        {
            this.ElementContext = null;
            this.SelectedInHierarchyElement = null;
            EnableSelectorWhenPOISelectedInHierarchy = false;

            var ec = GetDataAction.GetElementContext(ecId);

            if (ec != null)
            {
                var prevFocus = Keyboard.FocusedElement;

                try
                {

                    this.ctrlProgressRing.Activate();

                    this.ctrlHierarchy.IsEnabled = false;
                    ctrlHierarchy.Visibility = Visibility.Visible;
                    svInstructions.Visibility = Visibility.Collapsed;
                    await Task.Run(() =>
                    {
                        CaptureAction.SetLiveModeDataContext(ecId, Configuration.TreeViewMode);
                    }).ConfigureAwait(false);
                    Application.Current.Dispatcher.Invoke(() => {
                        this.ctrlHierarchy.DataContext = ec.DataContext;
                        this.ElementContext = ec;

                        // make sure that  when selected node is changed in hiearchy tree, enable selector. 
                        EnableSelectorWhenPOISelectedInHierarchy = true;

                        this.ctrlHierarchy.SetElement(ec);

                        AutomationProperties.SetName(this, string.Format(CultureInfo.InvariantCulture, "Live inspect with {0}", ec.Element.Glimpse));
                    });
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                    // if there was any exception, make sure that we enable selector later. 
                    EnableSelectorWhenPOISelectedInHierarchy = false;
                }
#pragma warning restore CA1031 // Do not catch general exception types
                finally
                {
                    this.ctrlProgressRing.Deactivate();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        this.ctrlHierarchy.IsEnabled = true;

                        // if the previously focused element is no longer visible and focus has gone to the window,
                        // we will set focus back to the hierarchy control. We do this because disabling the hierarchy control
                        // will throw keyboard focus to mainwindow, where it is not very useful.
                        if (prevFocus is FrameworkElement fe && !fe.IsVisible && Keyboard.FocusedElement is Window)
                        {
                            this.ctrlHierarchy.Focus();
                        }
                    });
                }
            }
            else
            {
                Clear();
            }

            if (EnableSelectorWhenPOISelectedInHierarchy == false)
            {
                Application.Current.Dispatcher.Invoke(() => UpdateStateMachineForLiveMode() );
            }
        }

        /// <summary>
        /// Make sure that statemachine and UI are updated for Live mode. 
        /// </summary>
        private static void UpdateStateMachineForLiveMode()
        {
            // enable selector once UI update is finished. 
            MainWin?.SetCurrentViewAndUpdateUI(InspectView.Live);
            MainWin?.EnableElementSelector();
        }

        /// <summary>
        /// Fire Async Content Loaded Event to notify Live mode data load completion. 
        /// it is not used for now based on customer feedback. we may revisit it. 
        /// </summary>
        private void FireAsyncContentLoadedEvent()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.AsyncContentLoaded))
            {
                UserControlAutomationPeer peer = UIElementAutomationPeer.FromElement(this) as UserControlAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(AsyncContentLoadedState.Completed, 100));
                }
            }
        }

        /// <summary>
        /// Update Element Info UI(Properties/Patterns/Tests)
        /// </summary>
        /// <param name="e"></param>
        private void UpdateElementInfoUI(A11yElement e)
        {
            this.ctrlTabs.SetElement(e, e != null && e.PlatformObject != null);
        }

        /// <summary>
        /// Hide control and hilighter
        /// </summary>
        public void HideControl()
        {
            UpdateConfigWithSize();
            // make sure that nothing is marked as selected.
            this.SelectedInHierarchyElement = null; 
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Show control and hilighter
        /// </summary>
        public void ShowControl()
        {
            AdjustMainWindowSize();
            this.Visibility = Visibility.Visible;
            this.runHkTest.Text = Configuration.HotKeyForSnap;
            this.runHkActivate.Text = Configuration.HotKeyForActivatingMainWindow;
            ClearSelectedItem();
            Dispatcher.Invoke(() =>
            {
                this.SetFocusOnDefaultControl();
            }
            , System.Windows.Threading.DispatcherPriority.Input);

            this.ctrlTabs.CurrentMode = AccessibilityInsights.SharedUx.Enums.InspectTabMode.Live;
        }

        /// <summary>
        /// Clear UI controls
        /// </summary>
        public void Clear()
        {
            ctrlHierarchy.Visibility = Visibility.Collapsed;
            svInstructions.Visibility = Visibility.Visible;

            this.SelectedInHierarchyElement = null;
            this.ElementContext = null;
            this.ctrlHierarchy.Clear();
            this.ctrlTabs.Clear();
        }

        // <summary>
        // Store Window data for Live Mode
        // </summary>
        public void UpdateConfigWithSize()
        {
            CurrentLayout.LayoutLive.ColumnSnapWidth = this.columnSnap.Width.Value;
        }

        // <summary>
        // Updates Window size with stored data and adjusts layout for Live Mode 
        // </summary>
        public void AdjustMainWindowSize()
        {
            this.columnSnap.Width = new GridLength(CurrentLayout.LayoutLive.ColumnSnapWidth);
            this.ctrlHierarchy.IsLiveMode = true;

            this.gsMid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Copy either selected properties or element data
        /// </summary>
        public void CopyToClipboard()
        {
            if (this.ElementContext != null)
            {
                var elementToCopy = this.ctrlHierarchy.GetSelectedElement() ?? this.ElementContext.Element;

                if (elementToCopy != null)
                {
                    StringBuilder sb = new StringBuilder();
                    // glimpse
                    sb.AppendFormat(CultureInfo.InvariantCulture, Properties.Resources.LiveModeControl_CopyToClipboard_Glimpse_0, elementToCopy.Glimpse);
                    sb.AppendLine();
                    sb.AppendLine();

                    sb.AppendLine(Properties.Resources.LiveModeControl_CopyToClipboard_Available_properties);
                    // properties
                    foreach (var p in elementToCopy.Properties)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}", p.Value.Name, p.Value.TextValue);
                        sb.AppendLine();
                    }

                    sb.AppendLine();

                    sb.AppendLine("Available patterns:");
                    // patterns   
                    foreach (var pt in elementToCopy.Patterns)
                    {
                        sb.Append(pt.Name);
                        sb.AppendLine();
                    }
                    sb.CopyStringToClipboard();
                    sb.Clear();
                }
            }
        }

        /// <summary>
        /// Request Test with selected element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            if (this.SelectedInHierarchyElement != null)
            {
                var sa = SelectAction.GetDefaultInstance();
                sa.SetCandidateElement(this.SelectedInHierarchyElement.Item1, this.SelectedInHierarchyElement.Item2);
                sa.Select();
                MainWin.HandleSnapshotRequest(MainWindow.TestRequestSources.HierarchyNode);
            }
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
            var enabled = !HollowHighlightDriver.GetDefaultInstance().IsEnabled;
            HollowHighlightDriver.GetDefaultInstance().IsEnabled = enabled;
            return enabled;
        }

        /// <summary>
        /// Set focus on default control for mode
        /// </summary>
        public void SetFocusOnDefaultControl()
        {
            if (this.ctrlHierarchy.Visibility == Visibility.Visible)
            {
                this.ctrlHierarchy.Focus();
            } 
            else
            {
                this.tbInstructions.Focus();
            }
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

        /// <summary>
        /// Manual gridsplitter drag behavior
        /// </summary>
        private void gsMid_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            if (e.HorizontalChange != 0) columnSnap.ResizeColumn(e.HorizontalChange);
        }

        /// <summary>
        /// Manual gridsplitter keyboard behavior
        /// </summary>
        private void gsMid_KeyDown(object sender, KeyEventArgs e)
        {
            const int increment = 5;

            if (e.Key == Key.Left)
            {
                columnSnap.ResizeColumn(-increment);
                e.Handled = true;
            }
            else if (e.Key == Key.Right)
            {
                columnSnap.ResizeColumn(increment);
                e.Handled = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
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
    }
}
