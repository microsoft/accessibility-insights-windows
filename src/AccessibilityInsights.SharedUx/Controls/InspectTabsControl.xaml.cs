// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Enums;
using Axe.Windows.Core.Bases;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for InspectTabsControl.xaml
    /// </summary>
    public partial class InspectTabsControl : UserControl
    {
        /// <summary>
        /// Expose event config
        /// </summary>
        public EventConfigTabControl CtrlEventConfig
        {
            get
            {
                return this.ctrlEventConfig;
            }
        }

        /// <summary>
        /// Expose event message
        /// </summary>
        public EventDetailControl CtrlEventMessage
        {
            get
            {
                return this.ctrlEventMessage;
            }
        }

        /// <summary>
        /// Action to perform when user needs to log into the server
        /// </summary>
        public Action SwitchToServerLogin
        {
            get
            {
                return this.ctrlTests.SwitchToServerLogin;
            }

            set
            {
                this.ctrlTests.SwitchToServerLogin = value;
            }
        }

        /// <summary>
        /// Dependency property to show how to fix tab
        /// </summary>
        public static readonly DependencyProperty CurrentModeProperty =
        DependencyProperty.Register("CurrentMode", typeof(InspectTabMode), typeof(InspectTabsControl),
            new PropertyMetadata(new PropertyChangedCallback(CurrentModePropertyChanged)));

        /// <summary>
        /// CurrentMode Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CurrentModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as InspectTabsControl;
            if (control != null)
            {
                control.CurrentMode = (InspectTabMode)e.NewValue;
            }
        }

        /// <summary>
        /// flat to show whether record is changed.
        /// </summary>
        /// <param name="isStarted"></param>
        public void IsRecordingChanged(bool isStarted)
        {
            if (this.CurrentMode == InspectTabMode.Events || this.CurrentMode == InspectTabMode.EventsRecording)
            {
                this.ctrlEventConfig.SetEditEnabled(isStarted);
                if (isStarted)
                {
                    // Switch to details tab when recording starts
                    this.CurrentMode = InspectTabMode.EventsRecording;
                }
            }
        }

        /// <summary>
        /// Update main window view selection based on current tab selection
        /// </summary>
        public Action<InspectTabType> UpdateMainWinView { get; set; }

        InspectTabMode _currentMode;
        public InspectTabMode CurrentMode
        {
            get
            {
                return _currentMode;
            }
            set
            {
                this._currentMode = value;
                tabControlInspect.Items.Clear();
                tabControlInspect.Items.Add(tabDetails);

                switch (_currentMode)
                {
                    case InspectTabMode.Live:
                        DisableEventsPanel();
                        this.tabDetails.IsSelected = true;
                        break;
                    case InspectTabMode.TestHowToFix:
                        tabControlInspect.Items.Add(tabHowToFix);
                        DisableEventsPanel();
                        this.tabHowToFix.IsSelected = true;
                        break;
                    case InspectTabMode.TestProperties:
                        tabControlInspect.Items.Add(tabHowToFix);
                        DisableEventsPanel();
                        this.tabDetails.IsSelected = true;
                        break;
                    case InspectTabMode.Events:
                        tabControlInspect.Items.Add(tabEvents);
                        EnableEventsPanel();
                        this.tabEvents.IsSelected = true;
                        break;
                    case InspectTabMode.EventsRecording:
                        tabControlInspect.Items.Add(tabEvents);
                        EnableEventsPanel();
                        this.tabDetails.IsSelected = true;
                        break;
                    case InspectTabMode.LoadedEvents:
                        EnableEventsPanel();
                        this.tabDetails.IsSelected = true;
                        break;
                }
            }
        }

        public static readonly RoutedCommand PropertySettingsAcceleratorCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToPropertiesGridCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToPropertiesSearchCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToPatternsTreeCommand = new RoutedCommand();
        public static readonly RoutedCommand MoveFocusToEventsMessagesCommand = new RoutedCommand();

        /// <summary>
        /// Constructor
        /// </summary>
        public InspectTabsControl()
        {
            InitializeComponent();

            InitCommandBindings();
            InitAcceleratorPropertiesOnDescendants();

            tabDetails.Tag = InspectTabType.Details;
            tabHowToFix.Tag = InspectTabType.HowToFix;
            tabEvents.Tag = InspectTabType.EventConfig;
        }

        void InitCommandBindings()
        {
            var bindings = CreateCommandBindings();
            this.CommandBindings.AddRange(bindings);
        }

        private CommandBinding[] CreateCommandBindings()
        {
            return new CommandBinding[]{
                    new CommandBinding(PropertySettingsAcceleratorCommand, this.OnPropertySettingsAccelerator),
            new CommandBinding(MoveFocusToPropertiesGridCommand, this.OnMoveFocusToPropertiesGrid),
            new CommandBinding(MoveFocusToPropertiesSearchCommand, this.OnMoveFocusToPropertiesSearch),
            new CommandBinding(MoveFocusToPatternsTreeCommand, this.OnMoveFocusToPatternsTree),
            new CommandBinding(MoveFocusToEventsMessagesCommand, this.OnMoveFocusToEventsMessages) };
        }

        /// <summary>
        /// This function is necessary because we need the accelerator keys to be
        /// available from everywhere inside the InspectTabsControl.
        /// However, the AutomationProperties for accelerator keys belong to a different class.
        /// </summary>
        private void InitAcceleratorPropertiesOnDescendants()
        {
            this.ctrlProperties.textboxSearch.SetValue(AutomationProperties.AcceleratorKeyProperty, "Alt+E");
            this.ctrlProperties.lvProperties.SetValue(AutomationProperties.AcceleratorKeyProperty, "Alt+P");
            this.ctrlPatterns.treePatterns.SetValue(AutomationProperties.AcceleratorKeyProperty, "Alt+A");
            this.CtrlEventMessage.dgEvents.SetValue(AutomationProperties.AcceleratorKeyProperty, "Alt+V");
        }

        /// <summary>
        /// Set element and children control elements
        /// </summary>
        /// <param name="el"></param>
        /// <param name="enableAction"></param>
        public void SetElement(A11yElement el, bool enableAction = false, Guid ecId = default(Guid))
        {
            if (el != null)
            {
                this.tbElement.Text = el.Glimpse;
                this.ctrlProperties.SetElement(el);
                this.ctrlPatterns.SetElement(el, enableAction);
                this.ctrlTests.SetElement(el, ecId);
            }
            else
            {
                Clear();
            }
        }

        /// <summary>
        /// Clear controls
        /// </summary>
        public void Clear()
        {
            this.tbElement.Text = null;

            this.ctrlProperties.Clear();
            this.ctrlTests.Clear();
            this.ctrlPatterns.Clear();
            this.ctrlEventMessage.Clear();
        }

        /// <summary>
        /// Label as pane
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        /// <summary>
        /// Handles checking of show all available properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowCoreProps_Checked(object sender, RoutedEventArgs e)
        {
            this.ctrlProperties.mniShowCorePropertiesChecked();
        }

        /// <summary>
        /// Handles unchecking of show all available properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowCoreProps_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ctrlProperties.mniShowCorePropertiesUnchecked();
        }

        /// <summary>
        /// Open properties configuration dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniIncludeAllProps_Click(object sender, RoutedEventArgs e)
        {
            this.ctrlProperties.mniIncludeAllPropertiesClicked();
        }

        /// <summary>
        /// Update show selected properties check state
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowCoreProps_Loaded(object sender, RoutedEventArgs e)
        {
            Controls.PropertyInfoControl.mniShowCorePropertiesLoaded(sender);
        }

        private void OnPropertySettingsAccelerator(object sender, RoutedEventArgs e)
        {
            var peer = new ButtonAutomationPeer(this.btnSetting);
            var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            invokeProvider?.Invoke();
        }

        private void OnMoveFocusToPropertiesGrid(object sender, RoutedEventArgs e)
        {
            var traversalRequest = new TraversalRequest(FocusNavigationDirection.First);
            this.ctrlProperties.lvProperties.MoveFocus(traversalRequest);
        }

        private void OnMoveFocusToPropertiesSearch(object sender, RoutedEventArgs e)
        {
            if (!this.ctrlProperties.textboxSearch.IsVisible)
            {
                System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            this.ctrlProperties.textboxSearch.Focus();
        }

        private void OnMoveFocusToPatternsTree(object sender, RoutedEventArgs e)
        {
            var traversalRequest = new TraversalRequest(FocusNavigationDirection.First);
            this.ctrlPatterns.MoveFocus(traversalRequest);
        }

        private void OnMoveFocusToEventsMessages(object sender, RoutedEventArgs e)
        {
            if (!this.CtrlEventMessage.IsVisible)
            {
                System.Media.SystemSounds.Asterisk.Play();
                return;
            }

            var traversalRequest = new TraversalRequest(FocusNavigationDirection.First);
            this.CtrlEventMessage.MoveFocus(traversalRequest);
        }

        /// <summary>
        /// Update main window when tab selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // This handler can be called by selectionchanged events occurring in child controls of the tabcontrol
            // We check to make sure the event source is actually the intended tab control before handling the event.
            if (e.Source == tabControlInspect && e.AddedItems.Count > 0)
            {
                var tab = (e.AddedItems[0] as TabItem);

                if (e.RemovedItems.Count > 0 && tab.Tag is InspectTabType type)
                {
                    UpdateMainWinView?.Invoke(type);
                }
            }
        }

        private void EnableEventsPanel()
        {
            this.expEvents.Visibility = Visibility.Visible;
            this.gsEvents.Visibility = Visibility.Visible;
            this.rowEvents.MinHeight = 80;
            this.rowEvents.Height = new GridLength(3, GridUnitType.Star);
        }

        private void DisableEventsPanel()
        {
            this.expEvents.Visibility = Visibility.Collapsed;
            this.gsEvents.Visibility = Visibility.Collapsed;
            this.rowEvents.MinHeight = 0;
            this.rowEvents.Height = new GridLength(0);
        }
    } // class
}//namespace
