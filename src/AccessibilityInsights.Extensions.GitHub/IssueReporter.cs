// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Reprting
    /// </summary>
    [Export(typeof(IIssueReporting))]
    public class IssueReporter : IIssueReporting
    {
        private IssueReporter _instance;
        private ConfigurationModel configurationControl;

        private IssueReporter()
        {
            configurationControl = new ConfigurationModel();
            configurationControl.Config = new ConnectionConfiguration(string.Empty);
        }

        public IssueReporter GetDefaultInstance()
        {
            if (_instance == null)
            {
                _instance = new IssueReporter();
            }
            return _instance;
        }

        public string ServiceName => Properties.Resources.extensionName;

        public Guid StableIdentifier => new Guid ("bbdf3582-d4a6-4b76-93ea-ef508d1fd4b8");
        public bool IsConfigured { get; private set; } = true;

        public IEnumerable<byte> Logo => null;

        public string LogoText => Properties.Resources.extensionName;

        public IssueConfigurationControl ConfigurationControl => configurationControl;

        public bool CanAttachFiles => false;

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return Task.Run<IIssueResult>(() => FileIssueAsyncAction(issueInfo));
        }

        private IIssueResult FileIssueAsyncAction(IssueInformation issueInfo)
        {
            if (this.IsConfigured)
            {
                try
                {
                    string url = IssueFormatterFactory.GetNewIssueLink(this.configurationControl.Config.RepoLink, issueInfo);
                    System.Diagnostics.Process.Start(url);
                }
                catch
                {
                    // TODO
                    // MessageDialog.Show("Invalid URL"); we can't use shared UX here for now, same issue for ADO extension
                }
            }

            return null;
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            return Task.Run(() => RestoreConfigurationAsyncAction(serializedConfig));
        }

        private void RestoreConfigurationAsyncAction(string serializedConfig)
        {
            this.configurationControl.Config = JsonConvert.DeserializeObject<ConnectionConfiguration>(serializedConfig);
            if (this.configurationControl.Config!=null && string.IsNullOrEmpty(this.configurationControl.Config.RepoLink))
            {
                this.IsConfigured = true;
            }
        }

        public IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton)
        {
            this.ConfigurationControl.UpdateSaveButton = UpdateSaveButton;
            return ConfigurationControl;
        }
    }
}
