// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary>
    /// Interaction logic for LoadingBox.xaml
    /// </summary>
    public partial class LoadingBox : Window
    {
        /// <summary>
        /// Delegate for Action after dialog is loaded. 
        /// </summary>
        Action<Action> ActionAfterLoaded;
        bool NoClose = true;

        public LoadingBox(String message, Action<Action> actionAfterLoaded)
        {
            InitializeComponent();
            MessageText.Text = message;
            this.ctrlProgressRing.Activate();
            this.ActionAfterLoaded = actionAfterLoaded;
            this.Closing += new System.ComponentModel.CancelEventHandler((object sender, System.ComponentModel.CancelEventArgs e) => e.Cancel = NoClose);
        }

        /// <summary>
        /// Window loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ActionAfterLoaded(() =>
            {
                NoClose = false;
                Dispatcher.Invoke(() => this.Close());
            });
        }
    }
}
