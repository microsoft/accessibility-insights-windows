// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using System.Globalization;
using System.Windows;

namespace AccessibilityInsights.SharedUx.Dialogs
{
    /// <summary
    /// Interaction logic for ChangeChannelContainedDialog.xaml
    /// </summary>
    public partial class ChangeChannelContainedDialog : ContainedDialog
    {
        public ChangeChannelContainedDialog(ReleaseChannel channel)
        {
            InitializeComponent();
            runChannelChange.Text = string.Format(CultureInfo.InvariantCulture, Properties.Resources.ChangeChannelContainedDialog_Text, channel);
            WaitHandle.Reset();
        }

        public override void SetFocusOnDefaultControl()
        {
            btnOk.Focus();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            WaitHandle.Set();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            WaitHandle.Set();
        }
    }
}
