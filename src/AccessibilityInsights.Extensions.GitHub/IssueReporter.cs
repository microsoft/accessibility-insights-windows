// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class IssueReporter : IIssueReporting
    {
        private IssueReporter _instance;
        private ConfigurationModel _configurationControl;

        private IssueReporter()
        {
            _configurationControl = new ConfigurationModel();
        }

        public IssueReporter getDeafualtInstance()
        {
            if (_instance == null)
            {
                _instance = new IssueReporter();
            }
            return _instance;
        }

        public string ServiceName => "GitHub Issues Reporting";

        public Guid StableIdentifier => new Guid ("bbdf3582-d4a6-4b76-93ea-ef508d1fd4b8");
        public bool IsConfigured { get; private set; } = false;

        public IEnumerable<byte> Logo => null;

        public string LogoText => "GitHub";

        public IssueConfigurationControl ConfigurationControl => _configurationControl;

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return new Task<IIssueResult>(()=> {
                if (this.IsConfigured)
                {
                    string url = IssueFormatter.GetFormattedString(this._configurationControl.Config, issueInfo);
                    System.Diagnostics.Process.Start(url);
                }

                return null;
            });
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            return new Task(()=>
            {
                Configuration deserializedConfig = JsonConvert.DeserializeObject<Configuration>(serializedConfig); ;
                if(string.IsNullOrEmpty(deserializedConfig.RepoLink))
                {
                    this._configurationControl.Config = deserializedConfig.RepoLink;
                    this.IsConfigured = true;
                }
            });
        }
    }
}
