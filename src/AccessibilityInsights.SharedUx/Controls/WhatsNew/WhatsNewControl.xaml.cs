// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls.WhatsNew
{
    /// <summary>
    /// Interaction logic for WhatsNewControl.xaml
    /// </summary>
    public partial class WhatsNewControl : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WhatsNewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set localized control type
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane", false, false);
        }

        /// <summary>
        /// Version of what's new to put in title
        /// </summary>
        private string version;
        public string Version
        {
            get
            {
                return this.version;
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    this.tbTitle.Text = Properties.Resources.WhatsNewControl_Version_What_s_new;
                }
                else
                {
                    this.tbTitle.Text = string.Format(CultureInfo.InvariantCulture, Properties.Resources.WhatsNewControl_Version_What_s_new_value, value);
                }
                this.version = value;
            }
        }

        /// <summary>
        /// DependencyProperty Version
        /// </summary>
        public static readonly DependencyProperty VersionProperty =
            DependencyProperty.Register("Version", typeof(string), typeof(WhatsNewControl),
            new PropertyMetadata(new PropertyChangedCallback(VersionPropertyChanged)));

        /// <summary>
        /// Version Property Change event handler
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void VersionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as WhatsNewControl;
            if (control != null)
            {
                control.Version = e.NewValue as string;
            }
        }

        /// <summary>
        /// Handles hyperlink click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Uri.OriginalString))
            {
                Process.Start(e.Uri.ToString());
            }
        }
    }
}
