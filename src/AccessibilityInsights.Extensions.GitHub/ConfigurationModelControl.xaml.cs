// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// Interaction logic for ConfigurationModel.xaml
    /// </summary>
    public partial class ConfigurationModel : IssueConfigurationControl
    {
        public ConnectionConfiguration Config { get; set; }

        public Action<bool> IsConfigured { get; set; }

        public ConfigurationModel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called when save button clicked.
        /// </summary>
        /// <returns>The extension’s new configuration, serialized</returns>
        public override string OnSave()
        {
            if (LinkValidator.IsValidGitHubRepoLink(this.tbURL.Text))
            {
                this.Config.RepoLink = this.tbURL.Text;
                canSave = false;
                UpdateSaveButton();
                IsConfigured(true);
            }
            else
            {
                MessageDialog.Show(Properties.Resources.InvalidLink);
            }
            return JsonConvert.SerializeObject(this.Config);
        }

        /// <summary>
        /// Called when settings page dismissed
        /// </summary>
        public override void OnDismiss()
        {
            this.tbURL.Text = this.Config.RepoLink;
            canSave = false;
            UpdateSaveButton();
        }

        public void TextChangeUpdateSaveButton(object sender, EventArgs e)
        {
            string curURL = this.tbURL.Text;
            if (!string.IsNullOrEmpty(curURL) && ((this.Config != null && string.IsNullOrEmpty(this.Config.RepoLink)) || !this.Config.Equals(curURL)))
            {
                canSave = true;
            }
            else
            {
                canSave = false;
            }
            UpdateSaveButton();
        }

        private bool canSave = false;
        /// <summary>
        /// Can the save button be clicked
        /// </summary>
        public override bool CanSave => canSave;


        private Action updateSaveButton;
        public override Action UpdateSaveButton
        {
            get
            {
                return updateSaveButton;
            }
            set
            {
                updateSaveButton = value;
                this.tbURL.TextChanged += TextChangeUpdateSaveButton;
            }
        }

        private void IssueConfigurationControl_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (Config != null && !string.IsNullOrEmpty(Config.RepoLink) && LinkValidator.IsValidGitHubRepoLink(Config.RepoLink) && !Config.RepoLink.Equals(UIResources.PlaceHolder, StringComparison.InvariantCulture))
                {
                    tbURL.Text = Config.RepoLink;
                    tbURL.Foreground = UIResources.BlackBrush;
                    IsConfigured(true);
                }
                else
                {
                }

            }
        }
    }
}
