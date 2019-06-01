// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for HotkeyGrabDialog.xaml
    /// </summary>
    public partial class HotkeyGrabDialog : Window
    {
        /// <summary>
        /// Modifier key
        /// </summary>
        ModifierKeys ModKey;

        /// <summary>
        /// Selected non-modifier key
        /// </summary>
        Key SelectedKey;

        /// <summary>
        /// Button to update
        /// </summary>
        Button ButtonParent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tb">Button which triggered dialog</param>
        public HotkeyGrabDialog(Button tb)
        {
            ButtonParent = tb;
            InitializeComponent();            
        }

        /// <summary>
        /// Handles key down event. Stores modifier or key. Exits dialog
        /// if appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Key tempKey = (e.Key == Key.System ? e.SystemKey : e.Key);

            switch (tempKey)
            {
                case Key.Escape:
                    return;
                case Key.LeftAlt:
                case Key.RightAlt:
                    ModKey |= ModifierKeys.Alt;
                    break;
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    ModKey |= ModifierKeys.Control;
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    ModKey |= ModifierKeys.Shift;
                    break;
                case Key.LWin:
                case Key.RWin:
                    ModKey |= ModifierKeys.Windows;
                    break;
                default:
                    SelectedKey = tempKey;
                    break;
            }

            if (ModKey != ModifierKeys.None)
            {
                this.lbModifiers.Content = GetCleanModifierString();
            }

            if (SelectedKey != Key.None)
            {
                this.lbKey.Content = GetCleanKeyString();
            }

            if(ModKey != ModifierKeys.None && SelectedKey != Key.None)
            {
                ButtonParent.Content = string.Format(CultureInfo.InvariantCulture, "{0} + {1}", GetCleanModifierString(), GetCleanKeyString());
                this.Close();
            }
        }

        /// <summary>
        /// Get the string to represent ModKeys. This string removes any spaces that get
        /// injected by .NET (for compatibility with existing settings)
        /// </summary>
        /// <returns></returns>
        private string GetCleanModifierString()
        {
            if (ModKey == ModifierKeys.None)
                return string.Empty;

            return ModKey.ToString().Replace(" ", "");
        }
        /// <summary>
        /// Get clean key string for display
        /// </summary>
        /// <returns></returns>
        private string GetCleanKeyString()
        {
            var str = SelectedKey.ToString();

            if (SelectedKey >= Key.D0 && SelectedKey <= Key.D9)
            {
                str = str.Substring(1);
            }

            return str;
        }

        /// <summary>
        /// Handles key up. Closes window on esc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            } 
        }
    }
}
