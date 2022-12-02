// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Enums;
using Axe.Windows.Core.Misc;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation;
using Axe.Windows.Desktop.UIAutomation.EventHandlers;
using Axe.Windows.Desktop.UIAutomation.Patterns;
using Axe.Windows.Desktop.UIAutomation.TreeWalkers;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Controls.TestTabs
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Interaction logic for TabStopControl.xaml
    /// </summary>
    public partial class TabStopControl : UserControl
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        /// <summary>
        /// The time toast shows before it hides
        /// </summary>
        const int ToastDelayInSeconds = 3;
        /// <summary>
        /// Element context
        /// </summary>
        public ElementContext ElementContext { get; set; }

        /// <summary>
        /// The most recently focused element
        /// </summary>
        private A11yElement CurrentElement { get; set; }

        /// <summary>
        /// Set Highlighter button state in main UI
        /// this should be provided from Test Mode controller.
        /// </summary>
        public Action<bool> SetHighlighterButtonState { get; set; }

        /// <summary>
        /// Toast Notification
        /// </summary>
        private ToastNotification Toast;

        private readonly object _lockObject = new object();

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
        /// Keep track of tab open state
        /// </summary>
        bool IsTabOpen;

        /// <summary>
        /// Event Handler
        /// </summary>
        EventListenerFactory EventHandler;

        int tabStopCount;
        /// <summary>
        /// Counter for current tab stop number
        /// </summary>
        int TabStopCount
        {
            get
            {
                lock (_lockObject)
                {
                    tabStopCount++;
                    return tabStopCount;
                }
            }
            set
            {
                tabStopCount = value;
            }
        }

        /// <summary>
        /// Gets/sets fastpass highlighter visibility, updating as necessary
        /// </summary>
#pragma warning disable CA1822
        public bool HighlightVisibility
#pragma warning restore CA1822
        {
            get
            {
                return Configuration.IsHighlighterOn;
            }
            set
            {
                Configuration.IsHighlighterOn = value;
                var ha = ClearOverlayDriver.GetDefaultInstance();

                if (value)
                {
                    ha.Show();
                    foreach (TabStopItemViewModel svm in lvElements.SelectedItems)
                    {
                        ha.AddElement(svm.Element, svm.Number);
                    }
                    Application.Current.MainWindow.Activate();
                }
                else
                {
                    ha.Hide();
                }
            }
        }

        private bool _isRecordingActive;
        private bool IsRecordingActive
        {
            get => _isRecordingActive;
            set
            {
                _isRecordingActive = value;
                tgbtnRecord.IsChecked = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TabStopControl()
        {
            InitializeComponent();
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
        /// Sets element context and updates UI
        /// </summary>
        /// <param name="ec"></param>
        public void SetElement(ElementContext ec)
        {
            this.ElementContext = ec ?? throw new ArgumentNullException(nameof(ec));
            this.runElement.Text = ec.Element.Glimpse;
            this.EventHandler?.Dispose();
            this.EventHandler = new EventListenerFactory(ec.Element);
            var brush = Application.Current.Resources["HLbrushPurple"] as SolidColorBrush;
            var ha = ClearOverlayDriver.GetDefaultInstance();
            ha.SetElement(ElementContext.Element, brush, null, 6);
            if (this.IsVisible && this.HighlightVisibility)
            {
                ha.Show();
            }
        }

        /// <summary>
        /// Add horizontal scroll bars when necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void scrollview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var view = sender as ScrollViewer;
            if (e.NewSize.Width <= this.gridTabText.MinWidth)
            {
                view.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                this.gridTabText.Width = this.gridTabText.MinWidth;
            }
            else
            {
                view.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.gridTabText.Width = Double.NaN;
            }

            if (e.NewSize.Height <= this.gridTabText.MinHeight)
            {
                view.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                this.gridTabText.Height = this.gridTabText.MinHeight;
            }
            else
            {
                view.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                this.gridTabText.Height = Double.NaN;
            }
        }

        /// <summary>
        /// Turn event recording on/off
        /// </summary>
        public void ToggleRecording()
        {
            if (this.IsTabOpen)
            {
                lock (_lockObject)
                {
                    if (IsRecordingActive)
                    {
                        StopRecordEvents();
                        this.Toast.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        // turn highlighter on when recording is starting
                        TurnOnHighlighter();
                        StartRecordingEvent();
                        var ha = ClearOverlayDriver.GetDefaultInstance();
                        ha.MarkSelectedElement();
                        this.Toast = new ToastNotification();
                        ha.ShowToast(Toast);
                        this.DelayCollapse();
                    }
                }
            }
            else
            {
                TurnOffHighlighter();
                this.Toast.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Collapse the toast notification
        /// </summary>
        private Task DelayCollapse()
        {
            return Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(ToastDelayInSeconds));
                this.Dispatcher.Invoke(() =>
                {
                    this.Toast.Visibility = Visibility.Collapsed;
                });
            });
        }

        /// <summary>
        /// Implicitly turn on highlighter when recording start.
        /// otherwise, users will be confused since there is no indication of recording.
        /// </summary>
        private void TurnOnHighlighter()
        {
            this.SetHighlighterButtonState(true);
            this.HighlightVisibility = true;
        }

        /// <summary>
        /// Implicitly turn off highlighter when recording start.
        /// otherwise, users will be confused since there is no indication of recording.
        /// </summary>
        private void TurnOffHighlighter()
        {
            this.SetHighlighterButtonState(false);
            this.HighlightVisibility = false;
        }

        /// <summary>
        /// Toggle event recording
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            ToggleRecording();
        }

        /// <summary>
        /// Activate highlighter based on list view selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var ha = ClearOverlayDriver.GetDefaultInstance();

            foreach (TabStopItemViewModel mes in e.RemovedItems)
            {
                ha.RemoveElement(mes.Element);
            }
            foreach (TabStopItemViewModel mes in e.AddedItems)
            {
                if (mes.Element.Patterns != null)
                    foreach (var p in mes.Element.Patterns)
                    {
                        if (p is ScrollItemPattern sip)
                        {
                            try
                            {
                                sip.ScrollIntoView();
                            }
#pragma warning disable CA1031 // Do not catch general exception types
                            catch (Exception ex)
                            {
                                ex.ReportException();
                                /// object dismissed
                            }
#pragma warning restore CA1031 // Do not catch general exception types
                        }
                    }
                mes.Element.PopulateAllPropertiesWithLiveData();
                ha.AddElement(mes.Element, mes.Number);
            }

            // record the number of items highlighted.
            Logger.PublishTelemetryEvent(TelemetryAction.TabStop_Select_Records,
                TelemetryProperty.By, ha.ElementCount.ToString(CultureInfo.InvariantCulture));

            var list = from TabStopItemViewModel l in lvElements.SelectedItems where !e.AddedItems.Contains(l) select l;
            foreach (TabStopItemViewModel mes in list)
            {
                mes.Element.PopulateAllPropertiesWithLiveData();
                ha.UpdateElement(mes.Element);
            }
        }

        /// <summary>
        /// Start record event logs
        /// </summary>
        private void StartRecordingEvent()
        {
            if (!IsRecordingActive && this.ElementContext != null)
            {
                TabStopCount = 0;

                CurrentElement = null;
                EventHandler?.RegisterAutomationEventListener(EventType.UIA_AutomationFocusChangedEventId, this.EventMessageReceived);
                lvElements.Items.Clear();
                IsRecordingActive = true;
                Logger.PublishTelemetryEvent(TelemetryAction.TabStop_Record_On,
                    TelemetryProperty.Scope, SelectAction.GetDefaultInstance().Scope.ToString());
            }
            else
            {
                MessageDialog.Show(Properties.Resources.TabStopControl_StartRecordingEvent_NoSelection);
            }
        }

        /// <summary>
        /// Checks if el is child of root
        /// </summary>
        /// <param name="root"></param>
        /// <param name="el"></param>
        /// <returns></returns>
        private static bool IsChildOf(A11yElement root, A11yElement el)
        {
            while (el != null)
            {
                if (el.IsSameUIElement(root))
                {
                    return true;
                }
                el = el.Parent;
            }
            return false;
        }

        private void EventMessageReceived(EventMessage message)
        {
            switch (message.EventId)
            {
                case EventType.UIA_EventRecorderNotificationEventId:
                    // this is an message about the event listener, not an actual event
                    ProcessEventRecorderNotification(message);
                    break;
                case EventType.UIA_AutomationFocusChangedEventId:
                    ProcessFocusedChangedEvent(message);
                    break;
                default:
                    // we don't care about other events here
                    break;
            }
        }

        private static void ProcessEventRecorderNotification(EventMessage message)
        {
            if (!IsListenerRegisteredNotification(message))
                return;

            ClearOverlayDriver.BringMainWindowOfPOIElementToFront();
        }

        private void ProcessFocusedChangedEvent(EventMessage message)
        {
            // exclude tooltip since it is transient UI.
            if (message.Element.ControlTypeId != Axe.Windows.Core.Types.ControlType.UIA_ToolTipControlTypeId && !message.Element.IsSameUIElement(CurrentElement))
            {
                // ancestry variable is not used directly. however, it populates ancestry of message.Element and the ancestry is used in IsChildOf(...) method.
                // Don't remove it.
                var ancestry = new DesktopElementAncestry(TreeViewMode.Control, message.Element, true);

                /// Don't do anything if focused element isn't in target scope
                if (!IsChildOf(this.ElementContext.Element, message.Element))
                {
                    //StopRecordEvents();
                    return;
                }
                this.CurrentElement = message.Element;

                /// if element has already been added to list, just select it
                foreach (TabStopItemViewModel ev in this.lvElements.Items)
                {
                    if (ev.Element != null && ev.Element.IsSameUIElement(message.Element))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            this.lvElements.ScrollIntoView(ev);
                        });

                        return;
                    }
                }

                Dispatcher.Invoke(() =>
                {
                    var count = TabStopCount;
                    var item = new TabStopItemViewModel(message.Element, count);
                    this.lvElements.Items.Add(item);
                    this.lvElements.ScrollIntoView(item);
                    this.lvElements.SelectedItems.Add(item);
                });
            }
            else
            {
                message.Dispose();
            }
        }

        /// <summary>
        /// Determines if a message is a notification that event listening has begun
        /// </summary>
        /// <param name="message">The message to evaluate</param>
        /// <returns>Is the message is a notification that event listening has begun</returns>
        private static bool IsListenerRegisteredNotification(EventMessage message)
        {
            // sadly, these strings are hardcoded into axe-windows at the moment they are populated...
            return message.Properties.Any(p => p.Key == "Message" && p.Value == "Succeeded to register an event listener");
        }

        /// <summary>
        /// Stop recording event logs
        /// </summary>
        internal void StopRecordEvents()
        {
            if (IsRecordingActive && this.EventHandler != null)
            {
                this.EventHandler.UnregisterAutomationEventListener(EventType.UIA_AutomationFocusChangedEventId);
                IsRecordingActive = false;
                this.Toast.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Automated Checks mode is left
        /// </summary>
        internal void Hide()
        {
            this.IsTabOpen = false;
            this.StopRecordEvents();

            // if we don't run this on dispatcher like this, we get rendering issues in the parent
            // tab control in TestModeControl on hide.
            Dispatcher.BeginInvoke(new Action(ClearOverlayDriver.GetDefaultInstance().Hide));
        }

        /// Automated Checks mode is shown
        internal async void Show()
        {
            ImageOverlayDriver.GetDefaultInstance().Hide();
            this.IsTabOpen = true;
            if (HighlightVisibility)
            {
                await System.Threading.Tasks.Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        this.HighlightVisibility = true;
                    });
                }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Clears listview
        /// </summary>
        public void ClearUI()
        {
            this.StopRecordEvents();
            this.EventHandler?.Dispose();
            this.EventHandler = null;
            this.ElementContext = null;
            this.lvElements.Items.Clear();
            ClearOverlayDriver.ClearDefaultInstance();
        }

        /// <summary>
        /// Call show/hide when tab changes
        /// </summary>
        private void UserControl_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void SetHotkeyText(string hk)
        {
            rtbInstructions.Document.Blocks.Clear();
            rtbInstructions.AppendText(string.Format(CultureInfo.InvariantCulture, Properties.Resources.TabStopControl_tbxInstructions, hk, hk));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Uri.OriginalString))
            {
                Process.Start(e.Uri.ToString());
            }
        }
    }
}

