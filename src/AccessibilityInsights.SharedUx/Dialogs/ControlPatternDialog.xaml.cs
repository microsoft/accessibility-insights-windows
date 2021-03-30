// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for ControlPatternDialog.xaml
    /// </summary>
    public partial class ControlPatternDialog : Window
    {
        public PatternViewModel PatternViewModel { get; private set; }

        public ControlPatternDialog(PatternViewModel patternViewModel)
        {
            InitializeComponent();
            this.PatternViewModel = patternViewModel ?? throw new ArgumentNullException(nameof(patternViewModel));
            this.Title = string.Format(CultureInfo.InvariantCulture, Properties.Resources.ControlPatternDialog_ControlPatternDialog__0__Action, this.PatternViewModel.Name);
            this.tcActions.ItemsSource = this.PatternViewModel.GetActionViewModels();
        }

        /// <summary>
        /// Key up event handler to close window when ESC is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
