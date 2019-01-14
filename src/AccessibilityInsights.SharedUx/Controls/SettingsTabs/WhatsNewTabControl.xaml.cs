// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Controls.CustomControls;
using System.Windows.Automation.Peers;
using System.Windows.Controls;


namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    /// <summary>
    /// Interaction logic for WhatsNewTabControl.xaml
    /// </summary>
    public partial class WhatsNewTabControl : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WhatsNewTabControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set localized control type
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }
    }
}
