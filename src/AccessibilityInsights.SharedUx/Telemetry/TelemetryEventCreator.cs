// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.FileIssue;
using System.Collections.Generic;
using System.Globalization;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Responsible for creating payloads for telemetry from user actions in the application
    /// </summary>
    public static class TelemetryEventCreator
    {
        public static IReadOnlyDictionary<TelemetryProperty, string> ForIssueFilingRequest(FileBugRequestSource source)
        {
            return new Dictionary<TelemetryProperty, string>()
            {
                    { TelemetryProperty.By, source.ToString() },
                    { TelemetryProperty.IsAlreadyLoggedIn, IssueReporter.IsConnected.ToString(CultureInfo.InvariantCulture) },
                    { TelemetryProperty.IssueReporter, IssueReporter.DisplayName?.ToString(CultureInfo.InvariantCulture) },
            };
        }
    }
}
