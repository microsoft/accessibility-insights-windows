// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.FileIssue
{
    /// <summary>
    /// Adapter between the core app and the issue reporting extension
    /// </summary>
    static public class IssueReporter
    {
        #region Unit test overrides
        internal static bool? TestControlledIsEnabled;
        internal static string TestControlledDisplayName;
        internal static Func<IssueInformation, IIssueResult> TestControlledFileIssueAsync;
        #endregion

        public static IIssueReporting IssueReporting { get; set; }

        public static bool IsEnabled
        {
            get
            {
                if (TestControlledIsEnabled.HasValue)
                {
                    return TestControlledIsEnabled.Value;
                }

                return (IssueReporterManager.GetInstance().IssueFilingOptionsDict != null && IssueReporterManager.GetInstance().IssueFilingOptionsDict.Any());
            }
        }

        public static bool IsConnected => IsEnabled && (IssueReporting == null ? false : IssueReporting.IsConfigured);

        public static ReporterFabricIcon Logo => (IsEnabled && IssueReporting != null )? IssueReporting.Logo : ReporterFabricIcon.PlugDisconnected ;

        public static string DisplayName
        {
            get
            {
                if (TestControlledDisplayName != null)
                {
                    return TestControlledDisplayName;
                }

                return (IsEnabled && IssueReporting != null) ? IssueReporting.ServiceName : null;
            }
        }

        public static Dictionary<Guid, IIssueReporting> GetIssueReporters()
        {
            return IssueReporterManager.GetInstance().IssueFilingOptionsDict;
        }

        public static Task RestoreConfigurationAsync(string serializedConfig)
        {
            if (IsEnabled && IssueReporting != null && IssueReporterManager.SelectedIssueReporterGuid != Guid.Empty)
            {
                return IssueReporting.RestoreConfigurationAsync(serializedConfig);
            }
            return Task.CompletedTask;
        }

        public static void SetConfigurationPath(string configurationPath)
        {
            if (IsEnabled && IssueReporting != null && IssueReporterManager.SelectedIssueReporterGuid != Guid.Empty)
            {
                IssueReporting.SetConfigurationPath(configurationPath);
            }
        }

        public static IIssueResult FileIssueAsync(IssueInformation issueInformation)
        {
            if (TestControlledFileIssueAsync != null)
            {
                return TestControlledFileIssueAsync(issueInformation);
            }

            if (IsEnabled && IsConnected)
            {
                // Coding to the agreement that FileIssueAsync will return a kicked off task.
                // This will block the main thread.
                // It does seem like we currently block the main thread when we show the win form for azure devops
                // so keeping it as is till we have a discussion. Check for blocking behavior at that link.
                // https://github.com/Microsoft/accessibility-insights-windows/blob/main/src/AccessibilityInsights.SharedUx/Controls/HierarchyControl.xaml.cs#L858
                IIssueResult result = IssueReporting.FileIssueAsync(issueInformation).Result;
                IssueReporterManager.GetInstance().UpdateIssueReporterSettings(IssueReporting);
                return result;
            }
            return null;
        }
    }
}
