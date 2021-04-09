// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Highlighting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using Axe.Windows.Actions;
using Axe.Windows.Actions.Contexts;
using Axe.Windows.Core.Bases;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.ActionViews
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Interaction logic for ReturnA11yElementView.xaml
    /// </summary>
    public partial class ReturnA11yElementsView
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        public ReturnA11yElementsViewModel ActionViewModel { get; private set; }
        int Counter;
        System.Timers.Timer timerInvoke;
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
        /// Element context
        /// </summary>
        public ElementContext ElementContext { get; set; }

        /// <summary>
        /// Gets/sets fastpass highlighter visibility, updating as necessary
        /// </summary>
        public static bool HighlightVisibility
        {
            get
            {
                return Configuration.IsHighlighterOn;
            }
            set
            {
                var overlayDriver = ClearOverlayDriver.GetDefaultInstance();
                var hollowDriver = HollowHighlightDriver.GetDefaultInstance();

                if (value)
                {
                    if(hollowDriver.IsEnabled)
                    {
                        overlayDriver.Show();
                        ClearOverlayDriver.BringMainWindowOfPOIElementToFront();
                        hollowDriver.HighlighterMode = HighlighterMode.Highlighter;
                    }
                    Application.Current.MainWindow.Activate();
                }
                else
                {
                    hollowDriver.HighlighterMode = ConfigurationManager.GetDefaultInstance().AppConfig.HighlighterMode;
                    overlayDriver.Hide();
                }
            }
        }

        /// <summary>
        /// Implicitly turn on highlighter when recording start.
        /// </summary>
        private static void TurnOnHighlighter()
        {
            HighlightVisibility = true;
        }

        /// <summary>
        /// Implicitly turn off highlighter when recording start.
        /// </summary>
        private static void TurnOffHighlighter()
        {
            HighlightVisibility = false;
        }

        /// <summary>
        /// Mark selected element
        /// </summary>
        public void MarkElement(A11yElement ele)
        {
            lock (_lockObject)
            {
                if (HollowHighlightDriver.GetDefaultInstance().IsEnabled)
                {
                    var overlayDriver = ClearOverlayDriver.GetDefaultInstance();
                    ClearOverlayDriver.BringMainWindowOfPOIElementToFront();
                    overlayDriver.MarkElement(ele);
                }
            }
        }

        /// <summary>
        /// Sets element context and updates UI
        /// </summary>
        /// <param name="ec"></param>
        public void SetElement()
        {
            var ecId = SelectAction.GetDefaultInstance().SelectedElementContextId;
            if (ecId.HasValue)
            {
                ElementContext ec = GetDataAction.GetElementContext(ecId.Value);
                this.ElementContext = ec;
                var brush = Application.Current.Resources["HLTextBrush"] as SolidColorBrush;
                var overlayDriver = ClearOverlayDriver.GetDefaultInstance();
                overlayDriver.SetElement(ElementContext.Element, brush, null, 0);
                if (this.IsVisible && HighlightVisibility)
                {
                    overlayDriver.Show();
                }
            }
        }

        /// <summary>
        /// control loaded action
        /// </summary>
        void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TurnOnHighlighter();
            Window window = Window.GetWindow(this);
            window.Closing += Window_Closing;
        }

        /// <summary>
        /// On closing control action
        /// </summary>
        void Window_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            TurnOffHighlighter();
        }

        public ReturnA11yElementsView(ReturnA11yElementsViewModel a )
        {
            this.ActionViewModel = a ?? throw new ArgumentNullException(nameof(a));
            this.Counter = 0;

            InitializeComponent();

            if (this.ActionViewModel.Parameters == null || this.ActionViewModel.Parameters.Count == 0)
            {
                this.dgParams.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.dgParams.ItemsSource = this.ActionViewModel.Parameters;
            }

            this.tbResult.Visibility = Visibility.Collapsed;
            this.timerInvoke = new System.Timers.Timer(1000);
            this.timerInvoke.Enabled = false;
            this.timerInvoke.AutoReset = false;
            this.timerInvoke.Elapsed += new ElapsedEventHandler(this.ontimerElapsed);
            this.Loaded += Window_Loaded;
        }

        /// <summary>
        /// tick on every seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ontimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(delegate ()
            {
                lock (_lockObject)
                {
                    this.Counter--;
                    if (this.Counter == 0)
                    {
                        this.ctrlCountDown.Visibility = Visibility.Collapsed;
                        ExecuteAction();
                        this.btnAction.IsEnabled = true;
                    }
                    else
                    {
                        this.ctrlCountDown.Count = this.Counter;
                        this.timerInvoke.Enabled = true;
                    }
                }
            });
        }

         private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            SetElement();
            int delay;
            if (Int32.TryParse(this.tbDelay.Text, out delay))
            {
                if (delay >= 1)
                {
                    this.btnAction.IsEnabled = false;
                    this.Counter = delay;
                    this.timerInvoke.Enabled = true;
                    ctrlCountDown.Count = this.Counter;
                    ctrlCountDown.Visibility = Visibility.Visible;
                }
                else
                {
                    ExecuteAction();
                }
            }
            else
            {
                MessageDialog.Show(Properties.Resources.ReturnA11yElementsView_btnRun_Click_Input_should_be_int_value_greater_than_equal_to_0);
            }
        }

        private void ExecuteAction()
        {
            this.ActionViewModel.DoAction();

            if (this.ActionViewModel.ReturnType == typeof(void))
            {
                ReadyForShowingText();

                if (this.ActionViewModel.IsSucceeded)
                {
                    this.tbResult.Text = Properties.Resources.ReturnA11yElementsView_ExecuteAction_Succeeded;
                    AutomationProperties.SetName(this.tbResult, Properties.Resources.ReturnA11yElementsView_ExecuteAction_Execution_result_passed);
                }
                else
                {
                    ShowErrorHResult();
                }
            }
            else
            {
                if (this.ActionViewModel.IsSucceeded == true)
                {
                    if (this.ActionViewModel.ReturnValue != null && this.ActionViewModel.ReturnValue.Count != 0)
                    {
                        ReadyForShowingElements();

                        this.cbElements.ItemsSource = this.ActionViewModel.ReturnValue;
                        this.cbElements.SelectedIndex = 0;
                    }
                    else
                    {
                        ReadyForShowingText();

                        this.tbResult.Text = Properties.Resources.ReturnA11yElementsView_ExecuteAction_Succeeded_with_no_element_returned;
                    }
                    AutomationProperties.SetName(this.tbResult, Properties.Resources.ReturnA11yElementsView_ExecuteAction_Execution_result_passed);
                }
                else
                {
                    ShowErrorHResult();
                }
            }
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

        /// <summary>
        /// Make UI ready for showing Element(s) data
        /// </summary>
        private void ReadyForShowingElements()
        {
            this.cbElements.Visibility = Visibility.Visible;
            this.ctrlProperties.Visibility = Visibility.Visible;
            this.ctrlPatterns.Visibility = Visibility.Visible;
            this.tbResult.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Make UI ready for showing text message
        /// </summary>
        private void ReadyForShowingText()
        {
            this.tbResult.Visibility = Visibility.Visible;
            this.cbElements.ItemsSource = null;
            this.cbElements.Visibility = Visibility.Collapsed;
            this.ctrlProperties.Visibility = Visibility.Collapsed;
            this.ctrlPatterns.Visibility = Visibility.Collapsed;
        }

        private void ShowErrorHResult()
        {
            this.tbResult.Visibility = Visibility.Visible;
            this.cbElements.Visibility = Visibility.Collapsed;
            this.ctrlProperties.Visibility = Visibility.Collapsed;
            this.ctrlPatterns.Visibility = Visibility.Collapsed;
            this.cbElements.ItemsSource = null;

            if (this.ActionViewModel.ReturnValue != null)
            {
                this.tbResult.Text = string.Format(Properties.Resources.ReturnA11yElementsView_ShowErrorHResult_Failed_HResult_0x_0_X8, this.ActionViewModel.ReturnValue);
            }
            else
            {
                this.tbResult.Text = Properties.Resources.FailedEmphasizedText;
            }
            AutomationProperties.SetName(this.tbResult, "Execution result failed");
        }

        private void cbElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ctrlProperties.SetElement((A11yElement)this.cbElements.SelectedItem);
            this.ctrlPatterns.SetElement((A11yElement)this.cbElements.SelectedItem, false);
            try
            {
                MarkElement(((A11yElement)this.cbElements.SelectedItem));
            }
            catch (NullReferenceException ex)
            {
                ex.ReportException();
            }
            catch (ArgumentNullException ex)
            {
                ex.ReportException();
            }
        }

        private void tbDelay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.IsTextAllowed();
        }

#pragma warning disable CA1801 // unused parameter
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
#pragma warning restore CA1801 // unused parameter
        {
            lock (_lockObject)
            {
                if (Counter != 0 && this.IsVisible == false)
                {
                    this.timerInvoke.Enabled = false;
                    this.btnAction.IsEnabled = true;
                }
            }
        }
    }
}
