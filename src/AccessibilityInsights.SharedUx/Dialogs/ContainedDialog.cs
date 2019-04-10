// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    public abstract class ContainedDialog : UserControl
    {
        /// <summary>
        /// Used to determine when the "dialog" is ready to return its result
        /// </summary>
        protected EventWaitHandle WaitHandle { get; } = new AutoResetEvent(true);

        protected bool DialogResult { get; set; } = false;

        /// <summary>
        /// Used to hide the ContainedDialog's container
        /// </summary>
        protected Action HideDialog { get; set; }

        protected abstract void SetFocusOnDefaultControl();

        public Task<bool> ShowDialog(Action hideDialog)
        {
            HideDialog = hideDialog;
            Dispatcher.Invoke(SetFocusOnDefaultControl, DispatcherPriority.Input);

            return Task.Run<bool>((Func<bool>)WaitAndHide);
        }

        /// <summary>
        /// Return DialogResult and hides dialog once ready
        /// </summary>
        /// <returns></returns>
        private bool WaitAndHide()
        {
            WaitHandle.WaitOne();
            HideDialog?.Invoke();
            return DialogResult;
        }
    }
}
