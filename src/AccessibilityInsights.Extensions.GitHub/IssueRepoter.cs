// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class IssueRepoter : IIssueReporting
    {
        private IssueRepoter _instance;
        private ConfigurationModel _configurationControl;

        private IssueRepoter()
        {
            _configurationControl = new ConfigurationModel();
        }

        public IssueRepoter getDeafualtInstance()
        {
            if (_instance == null)
            {
                _instance = new IssueRepoter();
            }
            return _instance;
        }

        public string ServiceName => "GitHub Issues Reporting";

        public Guid StableIdentifier => Guid.NewGuid();
        public bool IsConfigured { get; private set; } = false;

        public IEnumerable<byte> Logo => null;

        public string LogoText => "GitHub";

        public IssueConfigurationControl ConfigurationControl => _configurationControl;

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return new Task<IIssueResult>(()=> {
                if (!this.IsConfigured)
                {
                    return null;
                }

                string url = IssueFormatter.GetFormattedString(this._configurationControl.Config, issueInfo);
                System.Diagnostics.Process.Start(url);

                IssueResult retVal = new IssueResult("GitHub", new Uri(url));

                return retVal;
            });
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            return new Task(()=>
            {
                string deseriaizedConfig = string.Empty;
                if(string.IsNullOrEmpty(deseriaizedConfig))
                {
                    deseriaizedConfig = JsonConvert.DeserializeObject<string>(serializedConfig);
                    this._configurationControl.Config = deseriaizedConfig;
                    this.IsConfigured = true;
                }
            });
        }
    }
}
