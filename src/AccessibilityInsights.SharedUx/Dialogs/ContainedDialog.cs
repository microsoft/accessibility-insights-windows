// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    public abstract class ContainedDialog : UserControl
    {
        protected EventWaitHandle WaitHandle { get; } = new AutoResetEvent(true);

        protected bool DialogResult { get; set; } = false;

        protected Action HideDialog { get; set; }

        public Task<bool> ShowDialog(Action hideDialog)
        {
            HideDialog = hideDialog;
            return Task.Run<bool>(() =>
            {
                WaitAndHide();
                return DialogResult;
            });
        }

        private void WaitAndHide()
        {
            WaitHandle.WaitOne();
            HideDialog?.Invoke();
        }
    }
}
