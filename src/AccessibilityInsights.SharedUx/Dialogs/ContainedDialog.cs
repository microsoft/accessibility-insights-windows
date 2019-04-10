// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;
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

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "dialog");
        }

        public Task<bool> ShowDialog(Action hideDialog)
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
            HideDialog?.Invoke();
            return DialogResult;
        }
    }
}
