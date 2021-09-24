// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    public abstract class ContainedDialog : Border
    {
        /// <summary>
        /// Used to determine when the "dialog" is ready to return its result
        /// </summary>
        protected EventWaitHandle WaitHandle { get; } = new AutoResetEvent(true);

        protected bool DialogResult { get; set; }

        /// <summary>
        /// Used to hide the ContainedDialog's container
        /// </summary>
        protected Action<ContainedDialog> HideDialog { get; set; }

        protected ContainedDialog()
        {
            KeyDown += ContainedDialog_KeyUp;
            var rd = new ResourceDictionary()
            {
                Source = new Uri(@"pack://application:,,,/AccessibilityInsights.SharedUx;component/Resources/Styles.xaml", UriKind.Absolute),
            };

            Style = rd[nameof(ContainedDialog)] as Style;
        }

        private void ContainedDialog_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void ContainedDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Escape)
                return;

            e.Handled = true;
            DismissDialog();
        }

        public void DismissDialog()
        {
            DialogResult = false;
            WaitHandle.Set();
        }

        public abstract void SetFocusOnDefaultControl();

        public Task<bool> ShowDialog(Action<ContainedDialog> hideDialog)
        {
            HideDialog = hideDialog;
            Dispatcher.InvokeAsync(SetFocusOnDefaultControl, DispatcherPriority.Input);

            return Task.Run((Func<bool>)WaitAndHide);
        }

        /// <summary>
        /// Waits for user input and hides dialog once ready
        /// </summary>
        /// <returns>Current value of DialogResult</returns>
        private bool WaitAndHide()
        {
            WaitHandle.WaitOne();
            HideDialog?.Invoke(this);
            return DialogResult;
        }
    }
}
