// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.Extensions.Helpers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    /// <summary>
    /// Interaction logic for WebviewRuntimeNotInstalled.xaml
    /// </summary>
    public partial class WebviewRuntimeNotInstalled : Window
    {
        public WebviewRuntimeNotInstalled(bool onTop)
        {
            InitializeComponent();
            Topmost = onTop;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;

            try
            {
                Process.Start(uri.AbsoluteUri);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                // silently ignore.
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}