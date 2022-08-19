// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.CommonUxComponents.Dialogs;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Reporting
    /// </summary>
    [Export(typeof(IIssueReporting))]
    public class IssueReporter : IIssueReporting
    {
        private IssueReporter _instance;
        private readonly ConfigurationModel configurationControl;

#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        private IssueReporter()
        {
            configurationControl = new ConfigurationModel
            {
                IsConfigured = SetIsConfigured,
                Config = new ConnectionConfiguration(string.Empty)
            };
        }
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]

        private void SetIsConfigured(bool isConfigured)
        {
            this.IsConfigured = isConfigured;
        }

#pragma warning disable CA1024 // This should not be a property
        public IssueReporter GetDefaultInstance()
        {
            if (_instance == null)
            {
                _instance = new IssueReporter();
            }
            return _instance;
        }
#pragma warning restore CA1024 // This should not be a property

        public string ServiceName => Properties.Resources.extensionName;

        public Guid StableIdentifier => new Guid("bbdf3582-d4a6-4b76-93ea-ef508d1fd4b8");
        public bool IsConfigured { get; private set; }

        public string ConfigurationPath { get; private set; }

        public ReporterFabricIcon Logo => ReporterFabricIcon.GitHubLogo;

        public string LogoText => Properties.Resources.extensionName;

        public IssueConfigurationControl ConfigurationControl => configurationControl;

        public bool CanAttachFiles => false;

        public Task<IIssueResultWithPostAction> FileIssueAsync(IssueInformation issueInfo)
        {
            return Task.Run<IIssueResultWithPostAction>(() => FileIssueAsyncAction(issueInfo));
        }

        private IIssueResultWithPostAction FileIssueAsyncAction(IssueInformation issueInfo)
        {
            if (this.IsConfigured)
            {
                try
                {
                    string url = IssueFormatterFactory.GetNewIssueLink(this.configurationControl.Config.RepoLink, issueInfo);
                    System.Diagnostics.Process.Start(url);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    e.ReportException();
                    MessageDialog.Invoke(Properties.Resources.InvalidLink);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            return null;
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            return Task.Run(() => RestoreConfigurationAsyncAction(serializedConfig));
        }

        private void RestoreConfigurationAsyncAction(string serializedConfig)
        {
            ConnectionConfiguration config = JsonConvert.DeserializeObject<ConnectionConfiguration>(serializedConfig);
            if (config != null && !string.IsNullOrEmpty(config.RepoLink) && LinkValidator.IsValidGitHubRepoLink(config.RepoLink))
            {
                this.configurationControl.Config = config;
                this.IsConfigured = true;
            }
            else
            {
                this.configurationControl.Config = new ConnectionConfiguration(string.Empty);
                this.IsConfigured = false;
                MessageDialog.Show(Properties.Resources.InvalidLink);
            }
        }

        public IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton)
        {
            this.ConfigurationControl.UpdateSaveButton = UpdateSaveButton;
            return ConfigurationControl;
        }

        public bool TryGetCurrentSerializedSettings(out string settings)
        {
            settings = null;
            return false;
        }

        public void SetConfigurationPath(string configurationPath)
        {
            this.ConfigurationPath = configurationPath;
        }
    }
}
