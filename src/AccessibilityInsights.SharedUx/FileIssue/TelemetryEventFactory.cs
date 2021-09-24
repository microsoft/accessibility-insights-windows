// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using System.Collections.Generic;
using System.Globalization;

namespace AccessibilityInsights.SharedUx.FileIssue
{
    /// <summary>
    /// Responsible for creating payloads for telemetry from user actions in the application
    /// </summary>
    public static class TelemetryEventFactory
    {
        public static TelemetryEvent ForIssueFilingRequest(FileBugRequestSource source)
        {
            return new TelemetryEvent(TelemetryAction.Scan_File_Bug, new Dictionary<TelemetryProperty, string>()
            {
                { TelemetryProperty.By, source.ToString() },
                { TelemetryProperty.IsAlreadyLoggedIn, IssueReporter.IsConnected.ToString(CultureInfo.InvariantCulture) },
                { TelemetryProperty.IssueReporter, IssueReporter.DisplayName?.ToString(CultureInfo.InvariantCulture) },
            });
        }

        public static TelemetryEvent ForIssueFilingCompleted(IIssueResult issueResult, IssueInformation issueInformation)
        {
            if (issueResult?.IssueLink != null && issueInformation?.RuleForTelemetry != null)
            {
                return new TelemetryEvent(TelemetryAction.Issue_Save, new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.RuleId, issueInformation.RuleForTelemetry },
                    { TelemetryProperty.UIFramework, issueInformation.UIFramework ?? string.Empty },
                    { TelemetryProperty.IssueReporter, IssueReporter.DisplayName?.ToString(CultureInfo.InvariantCulture) },
                });
            }

            // if the bug is coming from the hierarchy tree, it will not have ruleID or UIFramework
            var action = issueResult?.IssueLink == null ?
                TelemetryAction.Issue_File_Attempt : TelemetryAction.Issue_Save;
            return new TelemetryEvent(action, new Dictionary<TelemetryProperty, string>
            {
                { TelemetryProperty.IssueReporter, IssueReporter.DisplayName?.ToString(CultureInfo.InvariantCulture) },
            });
        }

        public static TelemetryEvent ForReleaseNotesClick(string error)
        {
            return new TelemetryEvent(TelemetryAction.Upgrade_Update_ReleaseNote,
                new Dictionary<TelemetryProperty, string>
                {
                    { TelemetryProperty.Error, error },
                });
        }
    }
}
