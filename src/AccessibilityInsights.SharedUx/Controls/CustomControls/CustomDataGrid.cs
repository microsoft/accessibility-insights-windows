﻿using System.Windows;
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
            if (!(e.Key == Key.Tab)) return;

            var dir = e.KeyboardDevice.Modifiers == 
                ModifierKeys.Shift ? FocusNavigationDirection.Previous : FocusNavigationDirection.Next;

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
