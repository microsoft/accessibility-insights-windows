using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class IssueRepoter : IIssueReporting
    {
        private IssueRepoter _instance;
        
        private IssueRepoter()
        {

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

        public bool IsConfigured => throw new NotImplementedException();

        public IEnumerable<byte> Logo => null;

        public string LogoText => "GitHub";

        public IssueConfigurationControl ConfigurationControl => throw new NotImplementedException();

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return new Task<IIssueResult>(()=> {
                return null;
            });
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            throw new NotImplementedException();
        }
    }
}
