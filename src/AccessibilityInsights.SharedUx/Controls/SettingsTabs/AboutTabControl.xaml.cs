// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
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
            lbVersion.Content = AccessibilityInsights.Core.Misc.Utility.GetAppVersion();
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
            catch
            {
                // silently ignore. 
            }
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
            catch
            {
                // silently ignore. 
            }
        }
    }
}
