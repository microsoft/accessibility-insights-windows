// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// A "pane" control. Use this as a parent of controls if you want narrator to speak,
    /// "My AutomationProperties.Name pane" before speak the name of the newly focused child control
    /// </summary>
    public class NamedPane : UserControl
    {
        /// <summary>
        /// Orientation of pane
        /// </summary>
        public Orientation Orientation { get; set; }

        private AutomationOrientation AutomationOrientation =>
            Orientation == Orientation.Horizontal ? AutomationOrientation.Horizontal : AutomationOrientation.Vertical;

        /// <summary>
        /// DependencyProperty Orientation
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(NamedPane),
            new PropertyMetadata(new PropertyChangedCallback(OrientationPropertyChanged)));

        public NamedPane() : base()
        {
            this.KeyDown += NamedPane_KeyDown;
        }

        /// <summary>
        /// This allows users to navigate a vertical pane as though it is horizontal to address
        /// accessibility concerns from screen reader users
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NamedPane_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var elem = Keyboard.FocusedElement as FrameworkElement;
            var dir = GetNavDir(e.Key);

            if (dir.HasValue)
            {
                elem.MoveFocus(new TraversalRequest(dir.Value));
            }
        }

        private FocusNavigationDirection? GetNavDir(Key key)
        {
            if (Orientation != Orientation.Vertical)
                return null;

            switch (key)
            {
                case Key.Left:
                    return FocusNavigationDirection.Up;
                case Key.Right:
                    return FocusNavigationDirection.Down;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Orientation Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as NamedPane;

            if (control == null)
                return;

            control.Orientation = (Orientation)e.NewValue;
        }

        /// <summary>
        /// Identify as pane
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            var peer = new CustomControlOverridingAutomationPeer(this, "pane")
            {
                AutomationOrientation = AutomationOrientation
            };
            return peer;
        }
    }
}
