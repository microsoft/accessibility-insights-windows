// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Behaviors
{
    /// <summary>
    /// This behavior will propagate input and command bindings up to the parent window
    /// of the attached FrameworkElement when the element IsVisible and remove them when
    /// it is not
    /// Based on solution found at https://stackoverflow.com/questions/23316274/inputbindings-work-only-when-focused
    /// </summary>
    public static class PropagateBindingsBehavior
    {
        public static bool GetPropagateBindings(FrameworkElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return (bool)element.GetValue(PropagateBindingsProperty);
        }

        public static void SetPropagateBindings(FrameworkElement element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.SetValue(PropagateBindingsProperty, value);
        }

        /// <summary>
        /// Set this property to use this behavior
        /// </summary>
        public static readonly DependencyProperty PropagateBindingsProperty =
            DependencyProperty.RegisterAttached("PropagateBindings", typeof(bool), typeof(PropagateBindingsBehavior),
            new PropertyMetadata(false, OnPropagateBindingsChanged));

        private static void OnPropagateBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((FrameworkElement)d).IsVisibleChanged += PropagateBindingsBehavior_IsVisibleChanged;
            }
        }

        /// <summary>
        /// Propagate/remove bindings when visibility changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void PropagateBindingsBehavior_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var send = sender as FrameworkElement;
            var win = Window.GetWindow(send);

            if ((bool)e.NewValue)
            {
                foreach (InputBinding bind in send.InputBindings)
                {
                    win.InputBindings.Add(bind);
                }

                foreach (CommandBinding bind in send.CommandBindings)
                {
                    win.CommandBindings.Add(bind);
                }
            }
            else
            {
                foreach (InputBinding bind in send.InputBindings)
                {
                    win.InputBindings.Remove(bind);
                }

                foreach (CommandBinding bind in send.CommandBindings)
                {
                    win.CommandBindings.Remove(bind);
                }
            }
        }
    }
}
