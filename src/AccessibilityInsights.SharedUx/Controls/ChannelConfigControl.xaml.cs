// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Controls;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.ViewModels;
using System;
using System.Windows.Automation.Peers;
using System.Windows.Controls;

namespace AccessibilityInsights.SharedUx.Controls
{
    /// <summary>
    /// Interaction logic for ChannelConfigControl.xaml
    /// </summary>
    public partial class ChannelConfigControl : UserControl
    {
        public ChannelConfigViewModel VM { get; } = new ChannelConfigViewModel();

        public bool GetConfigChanged(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return config.ReleaseChannel != VM.CurrentChannel;
        }

        public ChannelConfigControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Overriding LocalizedControlType
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new CustomControlOverridingAutomationPeer(this, "pane");
        }

        public void UpdateFromConfig(ConfigurationModel config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            VM.CurrentChannel = config.ReleaseChannel;
        }

        public void SetUpdateSaveButton(Action updateSaveButton)
        {
            VM.UpdateSaveButton = updateSaveButton;
        }
    }
}