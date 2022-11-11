// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Enums;
using AccessibilityInsights.Misc;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Interfaces;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation.EventHandlers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Threading;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for EventModeControl.xaml
    /// </summary>
    public partial class EventModeControl : UserControlWithPanes, IModeControl
    {
#pragma warning disable IDE0052 // TODO: Is this really unused?
        private ElementContext ElementContext;
#pragma warning restore IDE0052

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
        /// App configuration
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
        /// Indicate the Current View of Events Mode
        /// </summary>
        public EventsView CurrentView { get; set; }

        /// <summary>
        /// Express whether Refresh button is needed.
        /// </summary>
        public bool IsRefreshEnabled { get { return false; } }

        /// <summary>
        /// Express whether Save button is needed.
        /// </summary>
        public bool IsSaveEnabled { get { return MainWin.CurrentView != EventsView.Load; } }

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
        public EventModeControl()
        {
            InitializeComponent();
            InitF6Panes(this.ctrlEvents, this.ctrlTabs);

            this.ctrlEvents.UpdateElementInfoWithSelectedEventRecord = UpdateElementInfoWithSelectedEventRecord;
            this.ctrlEvents.NotifyRecordingChange = HandleRecordingChange;
            this.ctrlEvents.UpdateGlobalFocusEventCheckbox = this.ctrlTabs.CtrlEventConfig.UpdateGlobalFocusEventCheckbox;
        }

        /// <summary>
        /// Handle Stop Recording notification from event recording control
        /// </summary>
        private void HandleRecordingChange(bool isStarted)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (MainWin.CurrentPage == AppPage.Events)
                {
                    if (isStarted)
                    {
                        MainWin.SetCurrentViewAndUpdateUI(EventsView.Recording);
                        this.ctrlTabs.Clear();
                    }
                    else
                    {
                        MainWin.SetCurrentViewAndUpdateUI(EventsView.Stopped);
                    }
                    this.ctrlTabs.IsRecordingChanged(isStarted);
                }
            });
        }

        /// <summary>
        /// Handles selection change in events list
        /// </summary>
        /// <param name="msg"></param>
        private void UpdateElementInfoWithSelectedEventRecord(EventMessage msg)
        {
            if (msg != null)
            {
                if (msg.EventId != EventType.UIA_EventRecorderNotificationEventId)
                {
                    var e = msg.Element;
                    UpdateUI(e);
                }
                else
                {
                    UpdateUI(null);
                }

                this.ctrlTabs.CtrlEventMessage.SetEventMessage(msg);
            }
            else
            {
                // clear data in Element Info
                this.ctrlTabs.Clear();
            }
        }

        /// <summary>
        /// Update Element Info UI(Properties/Patterns/Tests)
        /// </summary>
        /// <param name="ec"></param>
#pragma warning disable CS1998
        public async Task SetElement(Guid ecId)
        {
            var ec = GetDataAction.GetElementContext(ecId);

            if (ec != null)
            {
                this.ctrlTabs.CurrentMode = InspectTabMode.Events;
                try
                {
                    // the element context is a special one for Event, so it doesn't have any data yet.
                    // need to populate data with selected element.
                    var e = GetDataAction.GetA11yElementWithLiveData(ecId, 0);
                    this.ElementContext = ec;
                    this.ctrlEvents.SetElement(ec);
                    UpdateUI(e);
                    this.ctrlTabs.CtrlEventConfig.SetElement(e);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                    MessageDialog.Show(Properties.Resources.SetElementException);
                    this.Clear();
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
#pragma warning restore CS1998

        /// <summary>
        /// Update UI with element data
        /// </summary>
        /// <param name="element"></param>
        private void UpdateUI(A11yElement element)
        {
            this.ctrlTabs.SetElement(element, false);
            HollowHighlightDriver.GetDefaultInstance().SetElement(element);
        }

        /// <summary>
        /// Hide control and highlighter
        /// </summary>
        public void HideControl()
        {
            this.ctrlEvents.HideControl();
            UpdateConfigWithSize();
            this.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Show control and highlighter
        /// </summary>
        public void ShowControl()
        {
            AdjustMainWindowSize();
            this.Visibility = Visibility.Visible;
            this.ctrlEvents.ShowControl();
            // set focus on default element.
            Dispatcher.InvokeAsync(() =>
            {
                this.SetFocusOnDefaultControl();
            }
            , System.Windows.Threading.DispatcherPriority.Input);

            // Inspect tab control current mode to Events.
            this.ctrlTabs.CurrentMode = InspectTabMode.Events;
        }

        /// <summary>
        /// Clear UI
        /// </summary>
        public void Clear()
        {
            this.ElementContext = null;
            this.ctrlTabs.Clear();
            this.ctrlTabs.CtrlEventConfig.Clear();
            this.ctrlEvents.Clear();
        }

        /// <summary>
        /// Load event records
        /// </summary>
        /// <param name="el"></param>
        internal void LoadEventRecords(List<EventMessage> el)
        {
            this.ctrlTabs.CurrentMode = InspectTabMode.LoadedEvents;

            this.ctrlEvents.LoadEventRecords(el);
            HollowHighlightDriver highlightDriver = HollowHighlightDriver.GetDefaultInstance();
            if (highlightDriver.IsEnabled)
            {
                highlightDriver.IsEnabled = false;
                highlightDriver.HighlighterMode = HighlighterMode.Highlighter;
                MainWin.SetHighlightBtnState(false);
            }
        }

        // <summary>
        // Store Window data for event Mode
        // </summary>
        public void UpdateConfigWithSize()
        {
            CurrentLayout.LayoutEvent.ColumnSnapWidth = this.columnSnap.Width.Value;
        }

        // <summary>
        // Updates Window size with stored data and adjusts layout for event Mode
        // </summary>
        public void AdjustMainWindowSize()
        {
            MainWin.SizeToContent = SizeToContent.Manual;
            this.columnSnap.Width = new GridLength(CurrentLayout.LayoutEvent.ColumnSnapWidth);

            this.gsMid.Visibility = Visibility.Visible;
        }

        ///not implemented--nothing will copy
        public void CopyToClipboard()
        {
            return;
        }

        /// <summary>
        /// Handle Refresh request
        /// </summary>
        public void Refresh() { }

        /// <summary>
        /// Handle Save request
        /// </summary>
        public void Save()
        {
            this.ctrlEvents.SaveEvents();
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

#pragma warning disable IDE0051 // This is referenced in the XAML file
        /// <summary>
        /// Set focus when visible
        /// </summary>
        private void UserControl_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.Dispatcher.BeginInvoke((Action)delegate
                {
                    this.ctrlTabs.CtrlEventConfig.Focus();
                }, DispatcherPriority.Render);
            }
        }
#pragma warning restore IDE0051 // This is referenced in the XAML file

        /// <summary>
        /// Set focus on default control for mode
        /// </summary>
        public void SetFocusOnDefaultControl()
        {
        }
    }
}
