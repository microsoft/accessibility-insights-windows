// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Controls;
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Behaviors
{
    /// <summary>
    /// Behavior which autosizes columns upon invocation of ctrl + add (numpad plus)
    /// This behavior will listen to and handle the ctrl + add hotkey
    /// </summary>
    public class ColumnResizeHotkeyBehavior : Behavior<UIElement>
    {
        /// <summary>
        /// Attach to necessary event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            RoutedCommand command = new RoutedCommand();
            KeyBinding binding = new KeyBinding(command, Key.Add, ModifierKeys.Control);
            AssociatedObject.InputBindings.Add(binding);
            var cmdBinding = new CommandBinding(command, OnColumnResizeHotkeyPress);
            AssociatedObject.CommandBindings.Add(cmdBinding);
        }

        /// <summary>
        /// Handles resize hotkey press
        /// </summary>
        private void OnColumnResizeHotkeyPress(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid dg)
            {
                ResizeDataGrid(dg);
            }
            else if (sender is CustomListView customList)
            {
                customList.UpdateAllColumnWidths();
            }
            else if (sender is ListView lv)
            {
                ResizeListView(lv);
            }
            else if (sender is ScannerResultControl srCtrl)
            {
                srCtrl.nonFrameworkListControl.HasUserResizedLvHeader = true;
                ResizeListView(srCtrl.nonFrameworkListControl.lvDetails);
            }
        }

        /// <summary>
        /// Sets listview column widths to auto
        /// </summary>
        private static void ResizeListView(ListView lv)
        {
            if (lv.View is GridView gv)
            {
                foreach (var col in gv.Columns)
                {
                    col.Width = Double.NaN;
                }
            }
        }

        /// <summary>
        /// Sets datagrid column widths to auto
        /// </summary>
        private static void ResizeDataGrid(DataGrid dg)
        {
            foreach (var col in dg.Columns)
            {
                col.Width = new DataGridLength(0, DataGridLengthUnitType.Auto);
            }
        }
    }
}