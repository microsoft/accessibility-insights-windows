// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    }
}
