// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// Interaction logic for ConfigurationModel.xaml
    /// </summary>
    public partial class ConfigurationModel : IssueConfigurationControl
    {
        public string Config { get; set;}
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
            this.Config = this.tbURL.Text;
            return Config;
        }

        /// <summary>
        /// Called when settings page dismissed
        /// </summary>
        public override void OnDismiss()
        {
            this.tbURL.Text = this.Config;
        }

        public void TextChange_UpdateSaveButton(object sender, EventArgs e)
        {
            if (this.Config!= null && !this.Config.Equals(this.tbURL.Text))
            {
                canSave = true;
            }
            else
            {
                canSave = false;
            }
            Dispatcher.Invoke(UpdateSaveButton);
        }

        private bool canSave = false;
        /// <summary>
        /// Can the save button be clicked
        /// </summary>
        public override bool CanSave => canSave;


        private Action updateSaveButton;
        public override Action UpdateSaveButton {
            get
            {
                return updateSaveButton;
            }
            set
            {
                updateSaveButton = value;
                this.tbURL.TextChanged += TextChange_UpdateSaveButton;
            }
        }
    }
}
