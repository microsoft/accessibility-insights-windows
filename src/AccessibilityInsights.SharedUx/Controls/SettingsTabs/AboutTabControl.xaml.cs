// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Misc;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.Win32;
using System;
using System.Diagnostics;
using System.Globalization;
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
        /// <summary>
        /// Constructor
        /// </summary>
        public AboutTabControl()
        {
            InitializeComponent();
            InitializeVersionInformationLink();
            InitializeUIAccessLabel();
        }

        /// <summary>
        /// open a File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileLink_Click(object sender, RoutedEventArgs e)
        {
            var uri = ((Hyperlink)sender).NavigateUri;

            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, uri.OriginalString);

            try
            {
                if (System.IO.File.Exists(path))
                {
                    System.Diagnostics.Process.Start(path);
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

        private void InitializeVersionInformationLink()
        {
            string version = VersionTools.GetAppVersion();
            hlVersion.NavigateUri = new Uri(string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.VersionLinkFormat, version), UriKind.Absolute);
            hlVersion.Inlines.Clear();
            hlVersion.Inlines.Add(string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.VersionLinkContentFormat, version));
        }

        private void InitializeUIAccessLabel()
        {
            lbUIAccess.Content = NativeMethods.IsRunningWithUIAccess() ?
                Properties.Resources.LabelUIAccessAvailable :
                Properties.Resources.LabelUIAccessNotAvailable;
        }
    }
}
