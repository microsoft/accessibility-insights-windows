// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Controls.CustomControls
{
    /// <summary>
    /// Custom DataGrid class with enhancements to keyboard nav behavior.
    /// Ensures DataGrid is only one tab stop, despite WPF's best efforts
    /// </summary>
    class CustomDataGrid : DataGrid
    {
        public CustomDataGrid() : base()
        {
            this.PreviewKeyDown += CustomDataGrid_PreviewKeyDown;
            KeyboardNavigation.SetTabNavigation(this, KeyboardNavigationMode.Once);
        }

        /// <summary>
        /// Ensures tab/shift tab will always change focus away from datagrid
        /// </summary>
        private void CustomDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var modKey = e.KeyboardDevice.Modifiers;

            if (!(e.Key == Key.Tab) || !(modKey == ModifierKeys.None || modKey == ModifierKeys.Shift)) return;

            var dir = modKey == ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;

            // keep trying to move focus until focused element is not a descendant of the datagrid
            while (Keyboard.FocusedElement is Visual v && v.IsDescendantOf(this) && Keyboard.FocusedElement is FrameworkElement elem)
            {
                if (!elem.MoveFocus(new TraversalRequest(dir)))
                {
                    break;
                }
            }

            e.Handled = true;
        }
    }
}
