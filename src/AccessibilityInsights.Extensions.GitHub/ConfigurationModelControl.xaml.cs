// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;
using System;

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
            string trimmedLink = this.tbURL.Text.Trim();
            if (LinkValidator.IsValidGitHubRepoLink(trimmedLink))
            {
                this.Config.RepoLink = trimmedLink;
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

        private void TextChangeUpdateSaveButton(object sender, EventArgs e)
        {
            TextChangeUpdateSaveButtonHelper();
        }

        public void TextChangeUpdateSaveButtonHelper()
        {
            string curURL = this.tbURL.Text;
            if (!string.IsNullOrEmpty(curURL) && ((this.Config != null && string.IsNullOrEmpty(this.Config.RepoLink)) || !this.Config.RepoLink.Equals(curURL, StringComparison.Ordinal)))
            {
                canSave = true;
            }
            else
            {
                canSave = false;
            }
            UpdateSaveButton();
        }

        private bool canSave;
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
            if (sender!=null && (bool)e.NewValue)
            {
                if (Config != null && !string.IsNullOrEmpty(Config.RepoLink) && LinkValidator.IsValidGitHubRepoLink(Config.RepoLink) && !Config.RepoLink.Equals(this.tbURL.Text, StringComparison.Ordinal))
                {
                    tbURL.Text = Config.RepoLink;
                    IsConfigured(true);
                    TextChangeUpdateSaveButtonHelper();
                }
            }
        }
    }
}
