using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Controls.SettingsTabs;
using AccessibilityInsights.SharedUx.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.FileBug
{
    public class IssueReporterManager
    {
        public static Guid SelectedIssueReporterGuid { get; set; }
        readonly static object _lockObject = new object();
        Dictionary<Guid, IIssueReporting> IssueReportingOptionsDict = new Dictionary<Guid, IIssueReporting>();
        private static IssueReporterManager _defaultInstance = null;

        public static IssueReporterManager GetInstance()
        {
            if (_defaultInstance == null)
            {
                lock (_lockObject)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new IssueReporterManager();
                    }
                }
            }
            return _defaultInstance;
        }

        private IssueReporterManager()
        {
            // Get all serialized configs
            ConfigurationModel configs = ConfigurationManager.GetDefaultInstance().AppConfig;
            var serializedConfigsDict = configs.IssueReporterSerializedConfigs;
            Dictionary<Guid, string> configsDictionary = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigsDict);

            List<IIssueReporting> IssueReportingOptions = Container.GetDefaultInstance().IssueReporting;
            foreach (IIssueReporting issueReporter in IssueReportingOptions)
            {
                try
                {
                    if (issueReporter != null)
                    {
                        IssueReportingOptionsDict.Add(issueReporter.StableIdentifier, issueReporter);

                        // If config exists, restore it.
                        configsDictionary.TryGetValue(issueReporter.StableIdentifier, out string serializedConfig);
                        if (!string.IsNullOrWhiteSpace(serializedConfig))
                        {
                            issueReporter.RestoreConfigurationAsync(serializedConfig);
                        }
                    }
                }
                catch (ArgumentException ex)
                {
                    // Fail silently in case of dups.
                    Console.WriteLine("Found duplicate extensions" + ex.StackTrace);
                }
            }
            TestIssueProvider TIP = new TestIssueProvider();
            IssueReportingOptionsDict.Add(TIP.StableIdentifier, TIP);

            TestIssueProvider2 TIP2 = new TestIssueProvider2();
            IssueReportingOptionsDict.Add(TIP2.StableIdentifier, TIP2);
        }

        public Dictionary<Guid, IIssueReporting> GetIssueFilingOptionsDict()
        {
            return IssueReportingOptionsDict;
        }

        public void SetIssueReporter(Guid issueReporterGuid)
        {
            SelectedIssueReporterGuid = issueReporterGuid;
            IIssueReporting selectedIssueReporter;
            IssueReportingOptionsDict.TryGetValue(issueReporterGuid, out selectedIssueReporter);
            BugReporter.IssueReporter = selectedIssueReporter;
        }
    }

}
