using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.Controls.SettingsTabs
{
    [Export(typeof(IIssueReporting))]
    public class TestIssueProvider2 : IIssueReporting
    {
       public string ServiceName => "Ashwins test extension 2";
        //e3ecd010-c9e1-44b1-a6da-24fe4e3f117c
        public Guid StableIdentifier => Guid.Parse("879798f2-fad8-486a-ab1c-3a748e0cef1e");

        public bool IsConfigured => false;

        public bool CanAttachFiles => true;

        public IEnumerable<byte> Logo => null;

        public string LogoText => null;

        public Task<IIssueResult> FileIssueAsync(IssueInformation issueInfo)
        {
            return new Task<IIssueResult>(() => { return null; });
        }

        public Task RestoreConfigurationAsync(string serializedConfig)
        {
            return new Task<IIssueReporting>(() => { return null; });
        }

        public IssueConfigurationControl RetrieveConfigurationControl(Action UpdateSaveButton)
        {
            return new TestIssueConfigurationControl2(UpdateSaveButton);
        }
    }
}
