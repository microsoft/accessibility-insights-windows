// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUx.FileBug
{
    /// <summary>
    /// Adapter between the core app and the bug reporting extension
    /// </summary>
    static public class BugReporter
    {
        public static IIssueReporting IssueReporter { get; set; }

        public static bool IsEnabled => (IssueReporterManager.GetInstance().GetIssueFilingOptionsDict() != null && IssueReporterManager.GetInstance().GetIssueFilingOptionsDict().Any());

        public static bool IsConnected => IsEnabled && (IssueReporter == null ? false : IssueReporter.IsConfigured);

#pragma warning disable CA1819 // Properties should not return arrays
        public static byte[] Logo => IsEnabled ? IssueReporter.Logo?.ToArray() : null;
#pragma warning restore CA1819 // Properties should not return arrays

        public static string DisplayName => IsEnabled ? IssueReporter.ServiceName : null;

        public static Dictionary<Guid, IIssueReporting> GetIssueReporters() {
            return IssueReporterManager.GetInstance().GetIssueFilingOptionsDict();
        }

        public static Task RestoreConfigurationAsync(string serializedConfig)
        {
            if (IsEnabled && IssueReporterManager.SelectedIssueReporterGuid != null)
                return IssueReporter.RestoreConfigurationAsync(serializedConfig);
            return Task.CompletedTask;
        }

        // this should move to the issue reporter
        public static void SetSelectedIssueReporter(Guid issueReporterGuid) {
            IssueReporterManager.SelectedIssueReporterGuid = issueReporterGuid;
            IReadOnlyDictionary<Guid, IIssueReporting> issuReporterss= GetIssueReporters();
            IIssueReporting x;
            issuReporterss.TryGetValue(issueReporterGuid, out x);
            IssueReporter = x;
        }

        //public static Task<int?> AttachTestResultToBugAsync(string path, int bugId)
        //{
        //    if (IsEnabled)
        //        //return IssueReporter.AttachTestResultToBugAsync(path, bugId);

        //    return Task.FromResult((int?)null);
        //}

        //public static Task<string> AttachScreenshotToBugAsync(string path, int bugId)
        //{
        //    if (IsEnabled)
        //        //return IssueReporter.AttachScreenshotToBugAsync(path, bugId);

        //    return Task.FromResult(string.Empty);
        //}

        //public static Task<Uri> GetExistingBugUriAsync(int bugId)
        //{
        //    if (IsEnabled)
        //        //return IssueReporter.GetExistingBugUriAsync(bugId);

        //    return Task.FromResult((Uri)null);
        //}

        public static IIssueResult FileIssueAsync(IssueInformation issueInformation)
        {
            if (IsEnabled && IsConnected) {
                // Coding to the agreement that FileIssueAsync will return a kicked off task. 
                // This will block the main thread. 
                // It does seem like we currently block the main thread when we show the win form for azure devops
                // so keeping it as is till we have a discussion. Check for blocking behavior at that link.
                // https://github.com/Microsoft/accessibility-insights-windows/blob/master/src/AccessibilityInsights.SharedUx/Controls/HierarchyControl.xaml.cs#L858
                return IssueReporter.FileIssueAsync(issueInformation).Result;
            }
            return null;
        }

        //public static IConnectionInfo CreateConnectionInfo(Uri serverUri, IProject project, ITeam team)
        //{
        //    if (IsEnabled)
        //        return IssueReporter.CreateConnectionInfo(serverUri, project, team);

        //    return null;
        //}

        //public static IConnectionInfo CreateConnectionInfo(string configString)
        //{
        //    if (IsEnabled)
        //        return IssueReporter.CreateConnectionInfo(configString);

        //    return null;
        //}

        //public static IConnectionCache CreateConnectionCache(string configString)
        //{
        //    if (IsEnabled)
        //        return IssueReporter.CreateConnectionCache(configString);

        //    return null;
        //}
    }
}
