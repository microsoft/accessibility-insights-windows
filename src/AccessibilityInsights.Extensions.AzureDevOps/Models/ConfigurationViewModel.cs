// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.ComponentModel;
using System.Windows;
using static AccessibilityInsights.Extensions.AzureDevOps.ConfigurationControl;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// View model for the ConfigurationControl
    /// </summary>
    public class ConfigurationViewModel : ViewModelBase
    {
        private byte[] avatar;
#pragma warning disable CA1819 // Properties should not return arrays
        public byte[] Avatar
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get
            {
                return avatar;
            }
            set
            {
                avatar = value;
                OnPropertyChanged(nameof(Avatar));
                OnPropertyChanged(nameof(AvatarVisibility));
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
                OnPropertyChanged(nameof(DisplayNameVisibility));
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
                OnPropertyChanged(nameof(EmailVisibility));
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
                OnPropertyChanged(nameof(IsSelectTeamGridEnabled));
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
                return State == ControlState.HasServer ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public bool IsSelectTeamGridEnabled
        {
            get
            {
                return State == ControlState.EditingServer;
            }
        }

        public Visibility AvatarVisibility
        {
            get
            {
                return (avatar == null || avatar.Length == 0) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility DisplayNameVisibility
        {
            get
            {
                return string.IsNullOrEmpty(displayName) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility EmailVisibility
        {
            get
            {
                return string.IsNullOrEmpty(email) ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public BindingList<TeamProjectViewModel> Projects { get; } = new BindingList<TeamProjectViewModel>();
    }
}
