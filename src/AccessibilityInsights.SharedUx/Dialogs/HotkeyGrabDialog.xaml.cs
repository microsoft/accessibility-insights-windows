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
        ModifierKeys ModKey;

        Key SelectedKey;

        /// <summary>
        /// Button to update
        /// </summary>
        readonly Button ButtonParent;

        public HotkeyGrabDialog(Button tb)
        {
            ButtonParent = tb;
            Topmost = Application.Current.MainWindow.Topmost;
            InitializeComponent();
        }

        /// <summary>
        /// Handles key down event. Stores modifier or key. Exits dialog
        /// if appropriate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            Key tempKey = (e.Key == Key.System ? e.SystemKey : e.Key);

            switch (tempKey)
            {
                case Key.Escape:
                case Key.Tab:
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

            UpdateHotkeyText();

            if (ModKey != ModifierKeys.None && SelectedKey != Key.None)
            {
                ButtonParent.Content = string.Format(CultureInfo.InvariantCulture, "{0} + {1}", GetCleanModifierString(), GetCleanKeyString());
                this.Close();
            }
        }

        private void UpdateHotkeyText() => tbHotkey.Text = $"{GetCleanModifierString()} + {GetCleanKeyString()}";

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

        private string GetCleanKeyString()
        {
            if (SelectedKey == Key.None)
                return string.Empty;

            var str = SelectedKey.ToString();

            if (SelectedKey >= Key.D0 && SelectedKey <= Key.D9)
            {
                str = str.Substring(1);
            }

            return str;
        }

        private void WindowAndTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        private void TbHotkey_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = true;

        private void TbHotkey_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste || e.Command == ApplicationCommands.Cut || e.Command == ApplicationCommands.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
