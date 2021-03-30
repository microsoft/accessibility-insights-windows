// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Desktop.UIAutomation;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for ElementInfoDialog.xaml
    /// </summary>
    public partial class ElementInfoDialog : Window
    {
        public ElementInfoDialog(DesktopElement e) : this(new List<DesktopElement>() { e }) { }

        public ElementInfoDialog(IList<DesktopElement> list)
        {
            InitializeComponent();
            this.lbElements.ItemsSource = list ?? throw new ArgumentNullException(nameof(list));
            this.lbElements.SelectedIndex = 0;
            this.lbElements.Visibility = list.Count <= 1 ? Visibility.Collapsed : Visibility.Visible;
            this.lElements.Visibility = list.Count <= 1 ? Visibility.Collapsed : Visibility.Visible;
        }

        private void lbElements_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.lbElements.SelectedItem != null)
            {
                var el = lbElements.SelectedItem as DesktopElement;

                this.ctrlProperties.SetElement(el);
                this.ctrlPatterns.SetElement(el, false);
            }
        }

        /// <summary>
        /// Handles unchecking of show all available properties
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mniShowCoreProps_Checked(object sender, RoutedEventArgs e)
        {
            this.ctrlProperties.mniShowCorePropertiesChecked();
        }

        /// <summary>
        /// Handles checking of show all available properties
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
    }
}
