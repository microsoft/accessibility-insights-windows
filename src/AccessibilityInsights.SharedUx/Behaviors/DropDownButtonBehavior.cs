// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace AccessibilityInsights.SharedUx.Behaviors
{
    /// <summary>
    /// Dropdown button behavior for on the fly options.
    /// disable context menu via right click.
    /// </summary>
    public class DropDownButtonBehavior : Behavior<Button>
    {
        private bool isContextMenuOpen;

        /// <summary>
        /// Stores the button's background so that it can be restored later
        /// </summary>
        private Brush origBackground;

        /// <summary>
        /// Attach to necessary event handlers
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AddHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click), true);
            AssociatedObject.AddHandler(Button.ContextMenuOpeningEvent, new ContextMenuEventHandler(AssocisatedObject_ContextMenuOpening), true);
            AssociatedObject.AddHandler(Button.LoadedEvent, new RoutedEventHandler(AssociatedObject_Loaded), true);
        }

        /// <summary>
        /// Save button's original background once loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            origBackground = AssociatedObject.Background;
        }

        /// <summary>
        /// block Context menu opening
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssocisatedObject_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Open context menu when button is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void AssociatedObject_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button source = sender as Button;
            if (source != null && source.ContextMenu != null)
            {                
                source.Background = Application.Current.Resources["ButtonHoverBrush"] as SolidColorBrush;

                if (!isContextMenuOpen)
                {
                    // Add handler to detect when the ContextMenu closes
                    source.ContextMenu.AddHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed), true);
                    // If there is a drop-down assigned to this button, then position and display it 
                    source.ContextMenu.PlacementTarget = source;
                    source.ContextMenu.Placement = PlacementMode.Bottom;
                    source.ContextMenu.IsOpen = true;
                    isContextMenuOpen = true;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.RemoveHandler(Button.ClickEvent, new RoutedEventHandler(AssociatedObject_Click));
        }

        /// <summary>
        /// Handles context menu closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Background = origBackground;
            isContextMenuOpen = false;
            var contextMenu = sender as ContextMenu;
            if (contextMenu != null)
            {
                contextMenu.RemoveHandler(ContextMenu.ClosedEvent, new RoutedEventHandler(ContextMenu_Closed));
            }
        }
    }
}
