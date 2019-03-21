// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using System.Windows.Input;

namespace AccessibilityInsights.CommonUxComponents.Dialogs
{
    /// <summary>
    /// Interaction logic for MessageDialog.xaml
    /// </summary>
    public partial class MessageDialog : Window
    {
        public string Message { get; set; }

        public MessageDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show message dialog, return whether user clicked "OK"
        /// </summary>
        /// <param name="message"></param>
        public static bool? Show(string message)
        {
            var dlg = new MessageDialog() { Message = message };
            dlg.Owner = Application.Current.MainWindow;
            
            return dlg.ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            // If a user clicks "OK" it means the action was accepted
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Window loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbMessage.Text = this.Message;
            // set focus on this button by default
            this.btnClose.Focus();
        }

        /// <summary>
        /// Enter / space accepts action, escape denies it, all close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                this.DialogResult = false;
                this.Close();
            }
        }
    }
}
