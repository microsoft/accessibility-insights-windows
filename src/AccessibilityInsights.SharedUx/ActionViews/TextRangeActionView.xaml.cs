// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.SharedUx.Dialogs;
using AccessibilityInsights.SharedUx.Utilities;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.ActionViews
{
#pragma warning disable CA1001 // Types that own disposable fields should be disposable
    /// <summary>
    /// Interaction logic for TextRange ations
    /// handle cases with any number of parameters and any type of returns except A11yElement
    /// </summary>
    public partial class TextRangeActionView : UserControl
#pragma warning restore CA1001 // Types that own disposable fields should be disposable
    {
        public TextRangeActionViewModel ActionViewModel { get; private set; }
        int Counter;
        System.Timers.Timer timerInvoke;
        private readonly object _lockObject = new object();

        public TextRangeActionView(TextRangeActionViewModel a)
        {
            this.ActionViewModel = a ?? throw new ArgumentNullException(nameof(a));
            this.Counter = 0;
            InitializeComponent();

            if(this.ActionViewModel.Parameters == null || this.ActionViewModel.Parameters.Count == 0)
            {
                this.dgParams.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.dgParams.ItemsSource = this.ActionViewModel.Parameters;
            }

            this.ctrlTextRange.Visibility = Visibility.Collapsed;
            this.tbResult.Visibility = Visibility.Collapsed;
            this.timerInvoke = new System.Timers.Timer(1000);
            this.timerInvoke.Enabled = false;
            this.timerInvoke.AutoReset = false;
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
                MessageDialog.Show(Properties.Resources.TextRangeActionView_Button_Click_Input_should_be_int_value_greater_than_equal_to_0);
            }
        }

        private void ExecuteAction()
        {
            this.ActionViewModel.DoAction();

            if (this.ActionViewModel.ReturnType == typeof(void) || this.ActionViewModel.IsSucceeded == false)
            {
                this.ctrlTextRange.Visibility = Visibility.Collapsed;
                this.tbResult.Visibility = Visibility.Visible;

                if (this.ActionViewModel.IsSucceeded)
                {
                    this.tbResult.Text = Properties.Resources.TextRangeActionView_ExecuteAction_Succeeded;
                    AutomationProperties.SetName(this.tbResult, Properties.Resources.TextRangeActionView_ExecuteAction_Execution_result_passed);
                }
                else
                {
                    if (this.ActionViewModel.ReturnValue != null)
                    {
                        this.tbResult.Text = string.Format(Properties.Resources.TextRangeActionView_ExecuteAction_Failed_HResult__0x_0_X8, this.ActionViewModel.ReturnValue);
                    }
                    else
                    {
                        this.tbResult.Text = Properties.Resources.TextRangeActionView_ExecuteAction_Failed;
                    }
                    AutomationProperties.SetName(this.tbResult, Properties.Resources.TextRangeActionView_ExecuteAction_Execution_result_failed);
                }
            }
            else
            {
                if (this.ActionViewModel.ReturnValue != null)
                {
                    this.ctrlTextRange.Visibility = Visibility.Visible;
                    this.tbResult.Visibility = Visibility.Collapsed;
                    this.ctrlTextRange.SetTextRangeViewModel(new TextRangeViewModel(this.ActionViewModel.ReturnValue, null));
                }
                else
                {
                    this.tbResult.Visibility = Visibility.Visible;
                    this.ctrlTextRange.Visibility = Visibility.Collapsed;
                    this.tbResult.Text = Properties.Resources.TextRangeActionView_ExecuteAction_No_error_but_No_TextRange_is_returned;
                }
            }
        }

        private void tbDelay_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = e.Text.IsTextAllowed();
        }

#pragma warning disable CA1801 // unused parameter
        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
#pragma warning restore CA1801 // unused parameter
        {
            lock (_lockObject)
            {
                if(Counter != 0 && this.IsVisible == false)
                {
                    this.timerInvoke.Enabled = false;
                    this.btnAction.IsEnabled = true;
                }
            }
        }
    }
}
