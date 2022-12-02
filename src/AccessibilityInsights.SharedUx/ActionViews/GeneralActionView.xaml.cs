// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.ActionViews
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Interaction logic for General actions
    /// handle cases with any number of parameters and any type of returns except A11yElement
    /// </summary>
    public partial class GeneralActionView : UserControl
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        public GeneralActionViewModel ActionViewModel { get; private set; }
        int Counter;
        readonly Timer timerInvoke;
        private readonly object _lockObject = new object();

        // Keep track of combobox dropdown state
        bool isDropDownOpen;

        public GeneralActionView(GeneralActionViewModel a)
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

            this.dgReturn.Visibility = Visibility.Collapsed;
            this.tbResult.Visibility = Visibility.Collapsed;
            this.timerInvoke = new Timer(1000)
            {
                Enabled = false,
                AutoReset = false
            };
            this.timerInvoke.Elapsed += new ElapsedEventHandler(this.ontimerElapsed);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(this.tbDelay.Text, out int delay))
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
                MessageDialog.Show(Properties.Resources.GeneralActionView_Button_Click_Input_should_be_int_value_greater_than_equal_to_0);
            }
        }

        private void ExecuteAction()
        {
            this.ActionViewModel.DoAction();

            if (this.ActionViewModel.ReturnType == typeof(void) || this.ActionViewModel.IsSucceeded == false)
            {
                this.dgReturn.Visibility = Visibility.Collapsed;
                this.tbResult.Visibility = Visibility.Visible;

                if (this.ActionViewModel.IsSucceeded)
                {
                    this.tbResult.Text = Properties.Resources.GeneralActionView_ExecuteAction_Succeeded;
                    AutomationProperties.SetName(this.tbResult, Properties.Resources.GeneralActionView_ExecuteAction_Execution_result_passed);
                }
                else
                {
                    if (this.ActionViewModel.ReturnValue != null)
                    {
                        this.tbResult.Text = string.Format(CultureInfo.InvariantCulture, Properties.Resources.GeneralActionView_ExecuteAction_Failed_HResult_0x_0_X8, this.ActionViewModel.ReturnValue);
                    }
                    else
                    {
                        this.tbResult.Text = Properties.Resources.GeneralActionView_ExecuteAction_Failed;
                    }
                    AutomationProperties.SetName(this.tbResult, Properties.Resources.GeneralActionView_ExecuteAction_Execution_result_failed);
                }
            }
            else
            {
                this.dgReturn.Visibility = Visibility.Visible;
                this.tbResult.Visibility = Visibility.Collapsed;
                this.dgReturn.ItemsSource = this.ActionViewModel.ReturnValue;
            }
        }

        private void tbDelay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void UserControl_IsVisibleChanged(object _, DependencyPropertyChangedEventArgs _1)
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

        /// <summary>
        /// Custom keyboard navigation for datagrid cells in second column
        /// Don't let focus go to datagridcell. Instead, send focus to the cell's content as appropriate
        /// </summary>
        private void DataGridCell_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var navDir = e.OldFocus is ComboBox || e.OldFocus is TextBox ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;

            (sender as DataGridCell).MoveFocus(new TraversalRequest(navDir));
        }

        /// <summary>
        /// Open combobox on down arrow
        /// </summary>
        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cmbx = sender as ComboBox;

            if (e.Key == Key.Down)
            {
                cmbx.IsDropDownOpen = true;
            }
            else if (e.Key == Key.Escape)
            {
                this.isDropDownOpen = cmbx.IsDropDownOpen;
            }
        }
        /// <summary>
        /// Stop esc from closing both dropdown and dialog
        /// </summary>
        private void ComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            var cmbx = sender as ComboBox;

            if (e.Key == Key.Escape)
            {
                e.Handled = this.isDropDownOpen; // if the combobox dropdown was open, don't let Esc close the dialog
                this.isDropDownOpen = cmbx.IsDropDownOpen = false;
            }
        }
    }
}
