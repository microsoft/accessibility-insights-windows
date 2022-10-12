// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.ActionViews
{
    /// <summary>
    /// Interaction logic for NotSupportActionView.xaml
    /// </summary>
    public partial class NotSupportActionView : UserControl
    {
        private readonly BaseActionViewModel ActionViewModel;

        public NotSupportActionView(BaseActionViewModel a)
        {
            InitializeComponent();
            this.ActionViewModel = a ?? throw new ArgumentNullException(nameof(a));
            this.actionName.Text = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ActionViewModel.PatternName, ActionViewModel.Name);
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                MessageDialog.Show(Properties.Resources.hlLink_RequestNavigateException);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
