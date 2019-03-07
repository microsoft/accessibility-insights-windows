// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
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
        private static IBugReporting BugReporting => Container.GetDefaultInstance().BugReporting;

        public static bool IsEnabled => BugReporting != null;

        public static bool IsConnected => IsEnabled && BugReporting.IsConnected;

#pragma warning disable CA1819 // Properties should not return arrays
        public static byte[] Avatar => IsEnabled ? BugReporting.Avatar?.ToArray() : null;
#pragma warning restore CA1819 // Properties should not return arrays

        public static string DisplayName => IsEnabled ? BugReporting.DisplayName : null;

        public static string Email => IsEnabled ? BugReporting.Email : null;

        public static Task ConnectAsync(Uri uri, bool prompt)
        {
            if (IsEnabled)
                return BugReporting.ConnectAsync(uri, prompt);

            return Task.CompletedTask;
        }

        public static List<string> getIssueReporters() {
            return new List<string>() { "Github", "Azure Devops" };
        }
        public static Task RestoreConfigurationAsync(String serializedConfig)
        {
            //if (IsEnabled)
            //    return BugReporting.ConnectAsync(serializedConfig);
            System.Diagnostics.Trace.WriteLine(serializedConfig);

            return Task.CompletedTask;
        }

        public static void SetSelectedIssueReporter(Guid issueReporterGuid) {
            System.Diagnostics.Trace.WriteLine(issueReporterGuid);
        }

        public static void FlushToken(Uri uri)
        {
            if (IsEnabled)
                BugReporting.FlushToken(uri);
        }

        public static Task PopulateUserProfileAsync()
        {
            if (IsEnabled)
                return BugReporting.PopulateUserProfileAsync();

            return Task.CompletedTask;
        }

        public static void Disconnect()
        {
            if (IsEnabled)
                BugReporting.Disconnect();
        }

        public static Task<IEnumerable<IProject>> GetProjectsAsync()
        {
            if (IsEnabled)
                return BugReporting.GetProjectsAsync();

            return Task.FromResult(Enumerable.Empty<IProject>());
        }

        public static Task<string> GetExistingBugDescriptionAsync(int bugId)
        {
            if (IsEnabled)
                return BugReporting.GetExistingBugDescriptionAsync(bugId);

            return Task.FromResult(string.Empty);
        }

        public static Task<int?> ReplaceBugDescriptionAsync(string description, int bugId)
        {
            if (IsEnabled)
                return BugReporting.ReplaceBugDescriptionAsync(description, bugId);

            return Task.FromResult((int?)null);
        }

        public static Task<int?> AttachTestResultToBugAsync(string path, int bugId)
        {
            if (IsEnabled)
                return BugReporting.AttachTestResultToBugAsync(path, bugId);

            return Task.FromResult((int?)null);
        }

        public static Task<string> AttachScreenshotToBugAsync(string path, int bugId)
        {
            if (IsEnabled)
                return BugReporting.AttachScreenshotToBugAsync(path, bugId);

            return Task.FromResult(string.Empty);
        }

        public static Task<Uri> GetExistingBugUriAsync(int bugId)
        {
            if (IsEnabled)
                return BugReporting.GetExistingBugUriAsync(bugId);

            return Task.FromResult((Uri)null);
        }

        public static Task<Uri> CreateBugPreviewAsync(IConnectionInfo connectionInfo, BugInformation bugInfo)
        {
            if (IsEnabled)
                return BugReporting.CreateBugPreviewAsync(connectionInfo, bugInfo);

            return Task.FromResult((Uri)null);
        }

        public static IConnectionInfo CreateConnectionInfo(Uri serverUri, IProject project, ITeam team)
        {
            if (IsEnabled)
                return BugReporting.CreateConnectionInfo(serverUri, project, team);

            return null;
        }

        public static IConnectionInfo CreateConnectionInfo(string configString)
        {
            if (IsEnabled)
                return BugReporting.CreateConnectionInfo(configString);

            return null;
        }

        public static IConnectionCache CreateConnectionCache(string configString)
        {
            if (IsEnabled)
                return BugReporting.CreateConnectionCache(configString);

            return null;
        }
    }
}
