// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHub
{
    [Export(typeof(IIssueReporting))]
    public class IssueReporter : IIssueReporting
    {
        private IssueReporter _instance;
        private ConfigurationModel _configurationControl = new ConfigurationModel();

        private IssueReporter()
        {
        }

        public IssueReporter getDeafualtInstance()
        {
            if (_instance == null)
            {
                _instance = new IssueReporter();
            }
            return _instance;
        }

        public string ServiceName => Properties.Resources.extentionName;

        public Guid StableIdentifier => new Guid ("bbdf3582-d4a6-4b76-93ea-ef508d1fd4b8");
        public bool IsConfigured { get; private set; } = false;

        public IEnumerable<byte> Logo => null;

        public string LogoText => Properties.Resources.extentionName;

        public IssueConfigurationControl ConfigurationControl => _configurationControl;

        public bool CanAttachFiles => false;

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return Task.Run<IIssueResult>(()=> FileIssueAsyncAction(issueInfo));
        }

        private IIssueResult FileIssueAsyncAction(IssueInformation issueInfo)
        {
            if (this.IsConfigured)
            {
                string url = IssueFormatterFactory.GetNewIssueURL(this._configurationControl.Config, issueInfo);
                System.Diagnostics.Process.Start(url);
            }

            return null;
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            return Task.Run(()=> RestoreConfigurationAsyncAction(serializedConfig));
        }

        private void RestoreConfigurationAsyncAction(string serializedConfig)
        {
            Configuration deserializedConfig = JsonConvert.DeserializeObject<Configuration>(serializedConfig); ;
            if (string.IsNullOrEmpty(deserializedConfig.RepoLink))
            {
                this._configurationControl.Config = deserializedConfig.RepoLink;
                this.IsConfigured = true;
            }
        }

        public IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton)
        {
            _configurationControl.UpdateSaveButton = UpdateSaveButton;
            return _configurationControl;
        }
    }
}
