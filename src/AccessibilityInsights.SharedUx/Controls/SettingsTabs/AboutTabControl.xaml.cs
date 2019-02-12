// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;

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
        /// open EULA file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlTerms_Click(object sender, RoutedEventArgs e)
        {
            OpenFileFromAppDirectory("eula.rtf");
        }

        /// <summary>
        /// Open license file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlNotices_Click(object sender, RoutedEventArgs e)
        {
            OpenFileFromAppDirectory("ThirdpartyNotices.html");
        }

        /// <summary>
        /// Open a file from App bin folder
        /// </summary>
        /// <param name="v"></param>
        private static void OpenFileFromAppDirectory(string file)
        {
            var path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);

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
    }
}
