// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.Modes
{
    /// <summary>
    /// Interaction logic for ContainedDialogModeControl.xaml
    /// </summary>
    public partial class ContainedDialogModeControl : UserControl
    {
        public ContainedDialogModeControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "page");
        }

        public void HideControl() => Dispatcher.Invoke(() =>
        {
            this.Visibility = Visibility.Collapsed;
            brdrContainer.Child = null;
        });

        public async Task<bool> ShowControl(ContainedDialog containedDialog)
        {
            brdrContainer.Child = containedDialog;
            Visibility = Visibility.Visible;

            return await containedDialog.ShowDialog(HideControl).ConfigureAwait(false);
        }
    }
}
