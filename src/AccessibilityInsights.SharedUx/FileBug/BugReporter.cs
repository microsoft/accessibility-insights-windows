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

        //Uncomment and replace with the line below this.
        public static bool IsEnabled => (IssueReporterManager.GetInstance().GetIssueFilingOptionsDict() != null && IssueReporterManager.GetInstance().GetIssueFilingOptionsDict().Any());
        //public static bool IsEnabled => true;

        public static bool IsConnected => IsEnabled && (IssueReporter == null ? false : IssueReporter.IsConfigured);

#pragma warning disable CA1819 // Properties should not return arrays
        public static byte[] Logo => IsEnabled ? IssueReporter.Logo?.ToArray() : null;
#pragma warning restore CA1819 // Properties should not return arrays

        public static string DisplayName => IsEnabled ? IssueReporter.ServiceName : null;

        //public static Task ConnectAsync(Uri uri, bool prompt)
        //{
        //    if (IsEnabled)
        //        return IssueReporter.ConnectAsync(uri, prompt);

        //    return Task.CompletedTask;
        //}

        public static Dictionary<Guid, IIssueReporting> GetIssueReporters() {
            return IssueReporterManager.GetInstance().GetIssueFilingOptionsDict();
        }

        public static Task RestoreConfigurationAsync(string serializedConfig)
        {
            // This is the correct version. Uncomment and make sure it plays well
            //if (IsEnabled && IssueFilingManager.SelectedIssueReporterGuid != null)
            //    return IssueReporter.RestoreConfigurationAsync(serializedConfig);

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

        public static Task<IIssueResult> FileIssueAsync(IssueInformation issueInformation)
        {
            if (IsEnabled && IsConnected)
                return IssueReporter.FileIssueAsync(issueInformation);

            return Task.FromResult((IIssueResult)null);
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
