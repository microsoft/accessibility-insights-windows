// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AccessibilityInsights.Misc.FrameworkNavigator;

namespace AccessibilityInsights.Misc
{
    /// <summary>
    /// Implements the majority of logic providing for F6 (pane) navigation inside an AccessibilityInsights mode class
    /// </summary>
    public class UserControlWithPanes : UserControl, ISupportInnerF6Navigation
    {
        /// <summary>
        /// Contains the FrameworkElements, in order, that make up the F6 and shift + F6 pane cycle for this control.
        /// </summary>
        private IReadOnlyList<FrameworkElement> F6Panes = new List<FrameworkElement>();

        public UserControlWithPanes()
        {
            InitCommandBindings();
        }

        protected void InitF6Panes(params FrameworkElement[] f6Panes)
        {
            if (!f6Panes.Any())
                return;

            if (F6Panes.Any())
            {
                throw new InvalidOperationException(Properties.Resources.PanesCanBeSetOnlyOnce);
            }

            F6Panes = new List<FrameworkElement>(f6Panes);
        }

        private void InitCommandBindings()
        {
            var f6binding = new CommandBinding(MainWindow.F6Command, OnF6);
            this.CommandBindings.Add(f6binding);
            var shiftF6binding = new CommandBinding(MainWindow.ShiftF6Command, OnShiftF6);
            this.CommandBindings.Add(shiftF6binding);
        }

        private void OnF6(object sender, ExecutedRoutedEventArgs e)
        {
            var pane = GetSubsequentFrameworkElement(e.Parameter as FrameworkElement, this.F6Panes);
            MoveFocusToFrameworkElement(pane);
        }

        private void OnShiftF6(object sender, ExecutedRoutedEventArgs e)
        {
            var shiftF6panes = new List<FrameworkElement>(this.F6Panes);
            shiftF6panes.Reverse();
            var pane = GetSubsequentFrameworkElement(e.Parameter as FrameworkElement, shiftF6panes);
            MoveFocusToFrameworkElement(pane);
        }

        public FrameworkElement GetFirstPane()
        {
            if (this.F6Panes == null) return null;
            if (this.F6Panes.Count <= 0) return null;

            return this.F6Panes[0];
        }

        public FrameworkElement GetLastPane()
        {
            if (this.F6Panes == null) return null;

            var count = this.F6Panes.Count;
            if (count <= 0) return null;

            return this.F6Panes[count - 1];
        }
    } // class
} // namespace
