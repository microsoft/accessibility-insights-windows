// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AccessibilityInsights.SharedUx.ViewModels
{
    /// <summary>
    /// Keep the status of ChannelConfigControl
    /// </summary>
    public class ChannelConfigViewModel : ViewModelBase
    {
        public ObservableCollection<ReleaseChannel> Channels { get; } = new ObservableCollection<ReleaseChannel>(Enum.GetValues(typeof(ReleaseChannel)).Cast<ReleaseChannel>());

        private readonly Dictionary<ReleaseChannel, string> ChannelTextMapping = new Dictionary<ReleaseChannel, string>()
        {
            { ReleaseChannel.Production, Resources.ChannelConfigControl_ProductionDescription },
            { ReleaseChannel.Insider,Resources.ChannelConfigControl_InsiderDescription },
            { ReleaseChannel.Canary, Resources.ChannelConfigControl_CanaryDescription },
        };

        public string ChannelDescription => ChannelTextMapping[CurrentChannel];

        private ReleaseChannel currentChannel;
        public ReleaseChannel CurrentChannel
        {
            get => currentChannel;

            set
            {
                currentChannel = value;
                OnPropertyChanged(nameof(CurrentChannel));
                OnPropertyChanged(nameof(ChannelDescription));
                UpdateSaveButton?.Invoke();
            }
        }

        public Action UpdateSaveButton { get; set; }
        public ChannelConfigViewModel() { }
    }
}
