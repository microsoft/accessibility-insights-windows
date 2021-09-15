// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Behaviors
{
    /// <summary>
    /// Keyboard tool tip button behavior
    /// Display a tool tip on keyboard focus
    /// </summary>
    public class KeyboardToolTipButtonBehavior : Behavior<Button>
    {
        private static object currentToolTipButton;

        /// <summary>
        /// Attach to necessary event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.AddHandler(Button.KeyDownEvent, new KeyEventHandler(CancelButtonTooltipOnEscape), true);
            AssociatedObject.AddHandler(Button.LostKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(ClearButtonTooltip), true);
            AssociatedObject.AddHandler(Button.GotKeyboardFocusEvent, new KeyboardFocusChangedEventHandler(ShowButtonTooltip), true);
            AssociatedObject.AddHandler(Button.MouseEnterEvent, new MouseEventHandler(ButtonMouseEnter), true);
        }

        private static void ClearButtonTooltip()
        {
            if ((currentToolTipButton as Control)?.ToolTip != null &&
                (currentToolTipButton as Control).ToolTip.GetType().Name.Equals("ToolTip", StringComparison.Ordinal))
            {
                ToolTip tt = (ToolTip)(currentToolTipButton as Control).ToolTip;
                tt.IsOpen = false;
            }
        }

        private static void CancelButtonTooltipOnEscape(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ClearButtonTooltip();
            }
        }

        private static void ClearButtonTooltip(object sender, KeyboardFocusChangedEventArgs e)
        {
            ClearButtonTooltip();
        }

        private static void ShowButtonTooltip(object sender, KeyboardFocusChangedEventArgs e)
        {
            if ((sender as Control)?.ToolTip != null &&
                (sender as Control).ToolTip.GetType().Name.Equals("ToolTip", StringComparison.Ordinal))
            {
                ToolTip tt = (ToolTip)(sender as Control).ToolTip;
                if ((sender as Control).IsKeyboardFocusWithin && !tt.IsOpen)
                {
                    currentToolTipButton = sender;
                    //Places the Tooltip under the control rather than at the mouse position
                    tt.Visibility = Visibility.Visible;
                    tt.PlacementTarget = (UIElement)sender;
                    tt.Placement = PlacementMode.Left;
                    tt.PlacementRectangle = new Rect(0, (sender as Control).Height + 5, 0, 0);
                    tt.IsOpen = true;
                }
            }
        }

        private static void ButtonMouseEnter(object sender, MouseEventArgs e)
        {
            ClearButtonTooltip();
            currentToolTipButton = sender;
        }
    }
}
