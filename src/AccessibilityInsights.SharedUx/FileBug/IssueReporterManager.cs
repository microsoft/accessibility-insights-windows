// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Controls.SettingsTabs;
using AccessibilityInsights.SharedUx.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AccessibilityInsights.SharedUx.FileBug
{
    /// <summary>
    /// Class that deals with maintaining the available / selected issue reporters.
    /// </summary>
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
            Dictionary<Guid, string> configsDictionary = new Dictionary<Guid, string>();
            if (serializedConfigsDict != null)
            {
                configsDictionary = JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigsDict);
            }

            List<IIssueReporting> IssueReportingOptions = Container.GetDefaultInstance().IssueReportingOptions.ToList();
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
                    Console.WriteLine("Found duplicate extensions " + ex.StackTrace);
                }
            }

            // The following 4 lines will be removed after the integration is completed.
            TestIssueProvider TIP = new TestIssueProvider();
            IssueReportingOptionsDict.Add(TIP.StableIdentifier, TIP);

            TestIssueProvider2 TIP2 = new TestIssueProvider2();
            IssueReportingOptionsDict.Add(TIP2.StableIdentifier, TIP2);

            SetIssueReporter(configs.SelectedIssueReporter);
        }

        public Dictionary<Guid, IIssueReporting> GetIssueFilingOptionsDict()
        {
            return IssueReportingOptionsDict;
        }

        /// <summary>
        /// Sets the issue reporter guid in the issue reporter manager and makes sure that the bug reporter instance is updated.
        /// </summary>
        /// <param name="issueReporterGuid"> The StableId of the selected issue reporter</param>
        public void SetIssueReporter(Guid issueReporterGuid)
        {
            IssueReportingOptionsDict.TryGetValue(issueReporterGuid, out IIssueReporting selectedIssueReporter);
            if (selectedIssueReporter != null)
            {
                SelectedIssueReporterGuid = issueReporterGuid;
                BugReporter.IssueReporting = selectedIssueReporter;
            }
        }
    }

}
