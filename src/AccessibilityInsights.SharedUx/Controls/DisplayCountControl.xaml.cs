// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using System.Globalization;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for DisplayCountControl.xaml
    /// </summary>
    public partial class DisplayCountControl : UserControl
    {
#pragma warning disable CA1044 // Properties should not be write only
        /// <summary>
        /// Sets number to display on screen and fires LiveRegionChanged event
        /// </summary>
        public int Count
#pragma warning restore CA1044 // Properties should not be write only
        {
            set
            {
                // Use of LiveRegionChanged based on documentation at https://docs.microsoft.com/en-us/dotnet/framework/whats-new/whats-new-in-accessibility
                var peer = FrameworkElementAutomationPeer.FromElement(this.lbCount) ?? new FrameworkElementAutomationPeer(this.lbCount);
                this.lbCount.Text = value.ToString(CultureInfo.InvariantCulture);
                peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        public DisplayCountControl()
        {
            InitializeComponent();
        }
    }
}
