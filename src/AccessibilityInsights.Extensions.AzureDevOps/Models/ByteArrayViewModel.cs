// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows;
using static AccessibilityInsights.Extensions.AzureDevOps.ConfigurationControl;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// View model for a byte array, currently used for avatar image
    /// </summary>
    public class ConfigurationlViewModel : ViewModelBase
    {
        private byte[] array;
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] ByteData
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get
            {
                return array;
            }
            set
            {
                array = value;
                OnPropertyChanged(nameof(this.ByteData));
            }
        }

        private string displayName = "Name";
        public string DisplayName
        {
            get => displayName;

            set
            {
                displayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        private string email = "Email";

        public string Email
        {
            get => email;

            set
            {
                email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        private ControlState state;
        public ControlState State
        {
            get => state;

            set
            {
                state = value;
                OnPropertyChanged(nameof(TeamSelectedGridVisibility));
                OnPropertyChanged(nameof(BtnConnectVisibility));
                OnPropertyChanged(nameof(BtnDisconnectVisibility));
                OnPropertyChanged(nameof(SelectTeamGridVisibility));
            }
        }
        public Visibility TeamSelectedGridVisibility
        {
            get
            {
                return State == ControlState.HasServer ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility BtnConnectVisibility
        {
            get
            {
                return State == ControlState.NoServer ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility BtnDisconnectVisibility
        {
            get
            {
                return State == ControlState.NoServer ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility SelectTeamGridVisibility
        {
            get
            {
                return State == ControlState.EditingServer ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
