// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    /// <summary>
    /// Interaction logic for AboutTabControl.xaml
    /// </summary>
    public partial class AboutTabControl : UserControl
    {
        // ViewModel computes dialog values at run time
        public AboutTabViewModel ViewModel { get; } = new AboutTabViewModel();

        /// <summary>
        /// Constructor
        /// </summary>
        public AboutTabControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// open a File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uri.OriginalString);

            try
            {
                if (File.Exists(path))
                {
                    Process.Start(path);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                ex.ReportException();
                // silently ignore.
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// open a Hyper Link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HyperLink_Click(object sender, RoutedEventArgs e)
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
