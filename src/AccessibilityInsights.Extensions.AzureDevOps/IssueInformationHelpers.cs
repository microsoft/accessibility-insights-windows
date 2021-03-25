// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    internal static class IssueInformationHelpers
    {
        internal static Dictionary<IssueField, string> ToAzureDevOpsIssueFields(this IssueInformation issueInfo)
        {
            return new Dictionary<IssueField, string>
            {
                { IssueField.WindowTitle, GetStringValue(issueInfo.WindowTitle) },
                { IssueField.Glimpse, GetStringValue(issueInfo.Glimpse) },
                { IssueField.HowToFixLink, GetStringValue(issueInfo.HowToFixLink) },
                { IssueField.HelpURL, GetStringValue(issueInfo.HelpUri) },
                { IssueField.RuleSource, GetStringValue(issueInfo.RuleSource) },
                { IssueField.RuleDescription, GetStringValue(issueInfo.RuleDescription) },
                { IssueField.TestMessages, GetStringValue(issueInfo.TestMessages) },
                { IssueField.ProcessName, GetStringValue(issueInfo.ProcessName) },
                { IssueField.InternalGuid, GetStringValue(issueInfo.InternalGuid) },
                { IssueField.ElementPath, GetStringValue(issueInfo.ElementPath) },
                { IssueField.RuleForTelemetry, GetStringValue(issueInfo.RuleForTelemetry) },
                { IssueField.UIFramework, GetStringValue(issueInfo.UIFramework) },
            };
        }

        private static string GetStringValue(Uri value)
        {
            return value?.ToString();
        }

        private static string GetStringValue(IssueType? value)
        {
            return value?.ToString();
        }

        private static string GetStringValue(Guid? value)
        {
            return value?.ToString("D", CultureInfo.InvariantCulture);
        }

        private static string GetStringValue(double? value)
        {
            return value?.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetStringValue(Color? value)
        {
            return value?.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetStringValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value;
        }
    }
}
