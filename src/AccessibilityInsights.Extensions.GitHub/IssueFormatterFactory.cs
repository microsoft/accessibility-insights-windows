// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;
using System.Globalization;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Formatting
    /// </summary>
    public static class IssueFormatterFactory
    {
        public static string GetNewIssueLink(string link, IssueInformation issueInfo)
        {
            if (issueInfo == null)
                throw new ArgumentNullException(nameof(issueInfo));

            IIssueFormatter formatter;
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

            string FormattedURL = string.Format(CultureInfo.InvariantCulture, Properties.Resources.FormattedLink,
                link,
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
