// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
namespace AccessibilityInsights.SharedUx.FileIssue
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

        // Production constructor
        private IssueReporterManager()
            : this(ConfigurationManager.GetDefaultInstance().AppConfig, Container.GetDefaultInstance().IssueReportingOptions)
        {
        }

        // Unit testing constructor
        internal IssueReporterManager(ConfigurationModel configs, IEnumerable<IIssueReporting> issueReportingOptions)
        {
            var serializedConfigsDict = configs.IssueReporterSerializedConfigs;
            Dictionary<Guid, string> configsDictionary = !string.IsNullOrWhiteSpace(serializedConfigsDict) ?
                JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigsDict)
                : new Dictionary<Guid, string>();

            foreach (IIssueReporting issueReporter in issueReportingOptions ?? Enumerable.Empty<IIssueReporting>())
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
                catch (Exception ex)
                {
                    // Fail silently in case of dups.
                    Console.WriteLine("Found duplicate extensions / Extension failed to restore " + ex.StackTrace);
                    Logger.ReportException(ex);
                }
            }
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
                IssueReporter.IssueReporting = selectedIssueReporter;
            }
        }
    }
}
