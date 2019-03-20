// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Issue Formatting
    /// </summary>
    public static class IssueFormatterFactory
    {
        public static string GetNewIssueURL(string URL, IssueInformation issueInfo)
        {
            IIssueFormatter formatter = null;
            switch (issueInfo.IssueType)
            {
                case IssueType.NoFailure:
                    formatter = new NoFailuresIssueFormatter(issueInfo);
                    break;
                case IssueType.SingleFailure:
                    formatter = new SingleFailureIssueFormatter(issueInfo);
                    break;
                default:
                    return string.Empty;
            }

            string FormattedURL = string.Format("{0}/issues/new?title={1}&body={2}",
                URL,
                formatter.GetFormattedTitle(),
                formatter.GetFormattedBody());
            string escapedURL = Uri.EscapeUriString(FormattedURL).Replace("#", "%23");

            return escapedURL;
        }

        public static string GetStringValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return String.Empty;
            }
            return value;
        }
    }
}
