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
        private static IssueReporterManager _defaultInstance;
        private readonly ConfigurationModel _appConfig;
        private readonly IEnumerable<IIssueReporting> _issueReportingOptions;

        public static IssueReporterManager GetInstance()
        {
            if (_defaultInstance == null)
            {
                lock (_lockObject)
                {
#pragma warning disable CA1508 // Analyzer doesn't understand threading
                    if (_defaultInstance == null)
                    {
                        IssueReporterManager newInstance = new IssueReporterManager();
                        newInstance.RestorePersistedConfigurations();
                        _defaultInstance = newInstance;
                    }
#pragma warning restore CA1508 // Analyzer doesn't understand threading
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
        internal IssueReporterManager(ConfigurationModel appConfig, IEnumerable<IIssueReporting> issueReportingOptions)
        {
            if (appConfig == null)
                throw new ArgumentNullException(nameof(appConfig));

            _appConfig = appConfig;
            _issueReportingOptions = issueReportingOptions ?? Enumerable.Empty<IIssueReporting>();
        }

        internal void RestorePersistedConfigurations()
        {
            Dictionary<Guid, string> configsDictionary = GetConfigsDictionary();

            foreach (IIssueReporting issueReporter in _issueReportingOptions)
            {
                try
                {
                    if (issueReporter != null)
                    {
                        IssueReportingOptionsDict.Add(issueReporter.StableIdentifier, issueReporter);

                        // If configs exist, restore them.
                        configsDictionary.TryGetValue(issueReporter.StableIdentifier, out string serializedConfig);
                        if (!string.IsNullOrWhiteSpace(serializedConfig))
                        {
                            issueReporter.RestoreConfigurationAsync(serializedConfig);
                        }
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
                {
                    // Fail silently in case of dups.
                    Logger.ReportException(ex);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }

        private Dictionary<Guid, string> GetConfigsDictionary()
        {
            var serializedConfigsDict = _appConfig.IssueReporterSerializedConfigs;
            Dictionary<Guid, string> configsDictionary = !string.IsNullOrWhiteSpace(serializedConfigsDict) ?
                JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedConfigsDict)
                : new Dictionary<Guid, string>();
            return configsDictionary;
        }

        private void SaveConfigsDictionary(Dictionary<Guid, string> configsDictionary)
        {
            _appConfig.IssueReporterSerializedConfigs =
                JsonConvert.SerializeObject(configsDictionary);
        }

        /// <summary>
        /// Call this when settings for an IIssueReporting object may have changed. The
        /// settings will be fetched and updated in the store.
        /// </summary>
        /// <param name="issueReporting">The object whose settings may have changed</param>
        public void UpdateIssueReporterSettings(IIssueReporting issueReporting)
        {
            if (issueReporting == null)
                throw new ArgumentNullException(nameof(issueReporting));

            if (issueReporting.TryGetCurrentSerializedSettings(out string settings))
            {
                Dictionary<Guid, string> configsDictionary = GetConfigsDictionary();
                configsDictionary[issueReporting.StableIdentifier] = settings;
                SaveConfigsDictionary(configsDictionary);
            }
        }

        public Dictionary<Guid, IIssueReporting> IssueFilingOptionsDict => IssueReportingOptionsDict;

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
