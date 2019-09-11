// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for DialogContainerModeControl.xaml
    /// </summary>
    public partial class DialogContainerModeControl : UserControl
    {
        public DialogContainerModeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, Properties.Resources.LocalizedControlType_Page);
        }

        private void HideControl() => Dispatcher.Invoke(() =>
        {
            this.Visibility = Visibility.Collapsed;
            brdrContainer.Child = null;
        });

        private void ShowControl(ContainedDialog containedDialog) => Dispatcher.InvokeAsync(() =>
        {
            this.Visibility = Visibility.Visible;
            brdrContainer.Child = containedDialog;
        });

        public async Task<bool> ShowDialog(ContainedDialog containedDialog)
        {
            if (containedDialog == null)
                throw new ArgumentNullException(nameof(containedDialog));

            if (brdrContainer.Child is ContainedDialog oldDlg)
            {
                oldDlg.DismissDialog();
            }

            ShowControl(containedDialog);

            return await containedDialog.ShowDialog(HideControl).ConfigureAwait(false);
        }
    }
}
