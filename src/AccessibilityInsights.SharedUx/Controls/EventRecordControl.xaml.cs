// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Desktop.Types;
using Axe.Windows.Desktop.UIAutomation.EventHandlers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for EventRecordControl.xaml
    /// </summary>
    public partial class EventRecordControl : UserControl
    {
        private bool isClosed;

        public TwoStateButtonViewModel vmEventRecorder { get; private set; } = new TwoStateButtonViewModel(ButtonState.Off);

        /// <summary>
        /// Event handler to main window for recording start
        /// </summary>
        public Action<bool> NotifyRecordingChange{ get; set; }

        /// Updates the focus changed checkbox based on current config setting
        public Action UpdateGlobalFocusEventCheckbox { get; set; }

        public Action<EventMessage> UpdateElementInfoWithSelectedEventRecord { get; set; }

        private ElementContext ElementContext;

        private readonly object _lockObject = new object();

        public static RecorderSetting RecorderSetting
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance().EventConfig;
            }
        }

        Guid? _eventRecorderId;
        /// <summary>
        /// EventRecorder Id
        /// </summary>
        Guid? EventRecorderId
        {
            get
            {
                return _eventRecorderId;
            }
            set
            {
                // if we already have it, release it from ListenAction first.
                if(_eventRecorderId != null)
                {
                    ListenAction.ReleaseInstance(_eventRecorderId.Value);
                }

                _eventRecorderId = value;
            }
        }

        public EventMessage SelectedItem { get; internal set; }
        public static ConfigurationModel AppConfiguration
        {
            get
            {
                return ConfigurationManager.GetDefaultInstance()?.AppConfig;
            }
        }

        public static readonly RoutedCommand RecordCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToEventsGridCommand = new RoutedCommand();

        public EventRecordControl()
        {
            InitializeComponent();

            InitCommandBindings();
        }

        void InitCommandBindings()
        {
            var bindings = CreateCommandBindings();
            this.CommandBindings.AddRange(bindings);
        }

        private CommandBinding[] CreateCommandBindings()
        {
            return new CommandBinding[]
            {
                new CommandBinding(RecordCommand, this.onbuttonEventRecorderClicked),
                new CommandBinding(MoveFocusToEventsGridCommand, this.OnMoveFocusToEventsGrid, this.CanExecuteMoveFocusToEventsGridCommand)
            };
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        ///  clean up UI and properties.
        /// </summary>
        public void Clear()
        {
            this.ElementContext = null;

            ClearEventRecordGrid();

            this.btnRecord.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Clear Event record grid.
        /// </summary>
        private void ClearEventRecordGrid()
        {
            if (this.dgEvents.ItemsSource != null)
            {
                this.dgEvents.ItemsSource = null;
            }
            else
            {
                this.dgEvents.Items.Clear();
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
        /// Load saved Event Records
        /// </summary>
        /// <param name="el"></param>
        public void LoadEventRecords(IList<EventMessage> el)
        {
            this.btnRecord.Visibility = Visibility.Collapsed;
            this.tbIntro.Visibility = Visibility.Collapsed;
            this.svData.Visibility = Visibility.Visible;

            this.dgEvents.ItemsSource = el;

            // set focus on grid.
            this.dgEvents.Focus();
        }

        /// <summary>
        /// Handle start/stop recording button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onbuttonEventRecorderClicked(object sender, RoutedEventArgs e)
        {
            ToggleRecording();
            HollowHighlightDriver.GetDefaultInstance().Clear();
        }

        /// <summary>
        /// Toggle event recording
        /// </summary>
        public void ToggleRecording()
        {
            lock (_lockObject)
            {
                if (vmEventRecorder.State == ButtonState.Off)
                {
                    StartRecordingEvent();
                }
                else
                {
                    StopRecordEvents();
                }
            }
        }

        /// <summary>
        /// Set
        /// </summary>
        /// <param name="e"></param>
        public void SetElement(ElementContext ec)
        {
            this.ElementContext = ec;
        }

        public void ShowControl()
        {
            this.runHkRecord.Text = Configuration.HotKeyForRecord;
        }

        /// <summary>
        /// Reset control when hidden
        /// </summary>
        public void HideControl()
        {
            StopRecordEvents();
            this.svData.Visibility = Visibility.Collapsed;
            this.tbIntro.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Start record event logs
        /// </summary>
        private void StartRecordingEvent()
        {
            if (this.vmEventRecorder.State == ButtonState.Off && this.ElementContext != null)
            {
                this.tbIntro.Visibility = Visibility.Collapsed;
                this.svData.Visibility = Visibility.Visible;

                this.NotifyRecordingChange(true);

                Logger.PublishTelemetryEvent(TelemetryAction.Event_Start_Record);
                this.dgEvents.Items.Clear();
                this.vmEventRecorder.State = ButtonState.On;

                var config = ConfigurationManager.GetDefaultInstance().EventConfig;
                this.EventRecorderId = ListenAction.CreateInstance(config.ListenScope, this.ElementContext.Id, onRecordEvent);
                var propIds = GeneratePropertyIds(config);
                var eventIds = GenerateEventIds(config);
                ListenAction.GetInstance(this.EventRecorderId.Value).Start(eventIds, propIds);
            }
            else
            {
                MessageDialog.Show(Properties.Resources.EventRecordControl_StartRecordingEvent_No_element_is_selected_yet__please_select_first);
            }
        }

        /// <summary>
        /// Generate list of event ids to listen to from recorder setting
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static IEnumerable<int> GenerateEventIds(RecorderSetting config)
        {
            var eventIds = from c in config.Events
                           where config.IsListeningAllEvents || c.CheckedCount > 0
                           select c.Id;
            if (config.IsListeningFocusChangedEvent)
            {
                eventIds = eventIds.Append(EventType.UIA_AutomationFocusChangedEventId);
            }
            return eventIds;
        }

        /// <summary>
        /// Generate list of property ids to listen to from recorder setting
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static IEnumerable<int> GeneratePropertyIds(RecorderSetting config)
        {
            return from c in config.Properties
                where config.IsListeningAllEvents || c.CheckedCount > 0
                select c.Id;
        }

        /// <summary>
        /// Stop recording event logs
        /// </summary>
        public async void StopRecordEvents(bool onclose = false)
        {
            if(onclose)
            {
                this.isClosed = true;
                if (this.EventRecorderId != null)
                {
                    var la = ListenAction.GetInstance(this.EventRecorderId.Value);
                    la.Stop();
                    this.EventRecorderId = null;
                }
            }
            else if (this.vmEventRecorder.State == ButtonState.On && this.EventRecorderId != null)
            {
                this.ctrlProgressRing.Activate();

                await Task.Run(() => {
                    var la = ListenAction.GetInstance(this.EventRecorderId.Value);
                    la.Stop();
                }).ConfigureAwait(false);

                this.ctrlProgressRing.Deactivate();

                this.vmEventRecorder.State = ButtonState.Off;
                this.NotifyRecordingChange(false);
            }
        }

        /// <summary>
        /// Event Handler to update event log list view
        /// </summary>
        /// <param name="message"></param>
        private void onRecordEvent(EventMessage message)
        {
            if (this.vmEventRecorder.State == ButtonState.On && this.isClosed == false)
            {
                Dispatcher.Invoke(delegate ()
                {
                    lock (this.dgEvents)
                    {
                        this.dgEvents.Items.Add(message);
                        this.dgEvents.ScrollIntoView(message);
                        this.dgEvents.SelectedIndex = this.dgEvents.Items.Count - 1;

                        if (message.Element != null)
                        {
                            HollowHighlightDriver.GetDefaultInstance().SetElement(message.Element);
                        }

                        FireAsyncContentLoadedEventAtNewRecordAdded();
                    }
                });
            }
        }

        private void FireAsyncContentLoadedEventAtNewRecordAdded()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.AsyncContentLoaded))
            {
                ListBoxAutomationPeer peer = UIElementAutomationPeer.FromElement(this.dgEvents) as ListBoxAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseAsyncContentLoadedEvent(new AsyncContentLoadedEventArgs(AsyncContentLoadedState.Completed, 100));
                }
            }
        }

        private void listEvents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItem = (EventMessage)dgEvents.SelectedItem;
            UpdateElementInfoWithSelectedEventRecord(this.SelectedItem);
        }

        /// <summary>
        /// Save events file
        /// </summary>
        public void SaveEvents()
        {
            if (this.EventRecorderId != null)
            {
                var la = ListenAction.GetInstance(this.EventRecorderId.Value);
                if (HasRecordedEvents())
                {
                    using (var dlg = new System.Windows.Forms.SaveFileDialog
                    {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                        Filter = FileFilters.EventsFileFilter,
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                        InitialDirectory = AppConfiguration.EventRecordPath,
                        AutoUpgradeEnabled = !SystemParameters.HighContrast,
                    })
                    {
                        dlg.FileName = dlg.InitialDirectory.GetSuggestedFileName(FileType.EventRecord);

                        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Logger.PublishTelemetryEvent(TelemetryAction.Event_Save);

                            try
                            {
                                SetupLibrary.FileHelpers.SerializeDataToJSON(this.dgEvents.Items, dlg.FileName);
                            }
#pragma warning disable CA1031 // Do not catch general exception types
                            catch (Exception ex)
                            {
                                ex.ReportException();
                                MessageDialog.Show(string.Format(CultureInfo.InvariantCulture, "Couldn't save the event record file: {0}", ex.Message));
                            }
#pragma warning restore CA1031 // Do not catch general exception types
                        }

                        return;
                    }
                }
            }

            MessageDialog.Show(Properties.Resources.NoEventToSave);
        }

        /// <summary>
        /// Check whether there is any event recorded
        /// </summary>
        /// <returns></returns>
        private bool HasRecordedEvents()
        {
            return this.dgEvents.Items != null && this.dgEvents.Items.Count != 0;
        }

        private void OnMoveFocusToEventsGrid(object sender, RoutedEventArgs e)
        {
            var traversalRequest = new TraversalRequest(FocusNavigationDirection.First);
            this.dgEvents.MoveFocus(traversalRequest);
        }

        private void CanExecuteMoveFocusToEventsGridCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.dgEvents.Items.Count > 0;
        }

        /// <summary>
        /// Fix datagrid keyboard nav behavior
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgEvents_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg.Items != null && !dg.Items.IsEmpty && !(e.OldFocus is DataGridCell || e.OldFocus is DataGridColumnHeader))
            {
                if (dg.SelectedIndex == -1)
                {
                    dg.SelectedIndex = 0;
                    dg.CurrentCell = new DataGridCellInfo(dg.Items[0], dg.Columns[0]);
                }
                else
                {
                    dg.CurrentCell = new DataGridCellInfo(dg.Items[dg.SelectedIndex], dg.Columns[0]);
                }
            }
            else if (e.NewFocus is DataGridColumnHeader dgch && dgch.Column == null)
            {
                // this sets focus on the first column header instead of on a nonexistent header
                var header = dg.GetDGColumnHeader(dg.Columns[0]);
                if (!header.Focus()) //sometimes it takes two tries to focus on the header...
                    header.Focus();
            }
        }

        /// <summary>
        /// Update global event config menu with user's existing selections
        /// Scope information is from here
        /// https://msdn.microsoft.com/en-us/library/system.windows.automation.treescope(v=vs.110).aspx
        /// </summary>
        private void UpdateGlobalEventsMenu()
        {
            this.mniFocusChanged.IsChecked = RecorderSetting.IsListeningFocusChangedEvent;

            switch (RecorderSetting.ListenScope)
            {
                case ListenScope.Element:
                    this.radiobuttonScopeSelf.IsChecked = true;
                    break;
                case ListenScope.Descendants:
                    this.radiobuttonScopeDescendents.IsChecked = true;
                    break;
                default:
                    this.radiobuttonScopeSubtree.IsChecked = true;
                    break;
            }
        }

        /// <summary>
        /// Update event recorder settings based on user's selections
        /// </summary>
        private void UpdateRecorderSetting()
        {
            RecorderSetting.IsListeningFocusChangedEvent = this.mniFocusChanged.IsChecked;

            if (this.radiobuttonScopeSelf.IsChecked == true)
            {
                RecorderSetting.ListenScope = ListenScope.Element;
            }
            else if (this.radiobuttonScopeSubtree.IsChecked == true)
            {
                RecorderSetting.ListenScope = ListenScope.Subtree;
            }
            else if (this.radiobuttonScopeDescendents.IsChecked == true)
            {
                RecorderSetting.ListenScope = ListenScope.Descendants;
            }

            UpdateGlobalFocusEventCheckbox?.Invoke();
        }

        private void ContextMenu_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateGlobalEventsMenu();
        }

        private void ContextMenu_Unloaded(object sender, RoutedEventArgs e)
        {
            UpdateRecorderSetting();
        }

        /// <summary>
        /// Toggle menu item radio button when menu item is clicked
        /// </summary>
        private void mniRB_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem mi && mi.Header is RadioButton rb)
                rb.IsChecked = !rb.IsChecked;
        }
    } // class
} // namespace
