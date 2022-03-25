// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Behaviors
{
    /// <summary>
    /// Expander behavior for Element Information tab
    /// </summary>
    public class ExpanderBehavior : Behavior<Expander>
    {
        private Grid ParentGrid;

        /// <summary>
        /// Attach to necessary event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Expander.CollapsedEvent, new RoutedEventHandler(Expander_Collapsed), true);
            AssociatedObject.AddHandler(Expander.ExpandedEvent, new RoutedEventHandler(Expander_Expanded), true);
            this.ParentGrid = AssociatedObject.Parent as Grid;
        }

        /// <summary>
        /// Accordion behavior on expander collapse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            double height = (double)Application.Current.Resources["ExpanderCollapsedHeight"];

            var exp = sender as Expander;
            int row = Grid.GetRow(exp);
            ParentGrid.RowDefinitions[row].Tag = ParentGrid.RowDefinitions[row].Height;
            ParentGrid.RowDefinitions[row].MinHeight = height;
            ParentGrid.RowDefinitions[row].Height = new GridLength(height);
            ParentGrid.RowDefinitions[row].MaxHeight = height;
        }

        /// <summary>
        /// Accordion behavior on expander expand
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            var exp = sender as Expander;
            int row = Grid.GetRow(exp);
            if (ParentGrid.RowDefinitions[row].Tag != null)
            {
                ParentGrid.RowDefinitions[row].Height = (GridLength)ParentGrid.RowDefinitions[row].Tag;
                ParentGrid.RowDefinitions[row].MaxHeight = Double.PositiveInfinity;
                ParentGrid.RowDefinitions[row].MinHeight = 80;
            }
        }
    }
}
