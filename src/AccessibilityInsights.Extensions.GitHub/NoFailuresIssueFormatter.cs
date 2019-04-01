// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System.Globalization;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Single Issue Formatting
    /// </summary>
    public class NoFailuresIssueFormatter : IIssueFormatter
    {
        public IssueInformation IssueInfo { get; }
        public NoFailuresIssueFormatter(IssueInformation issueInfo)
        {
            this.IssueInfo = issueInfo;
        }
        public string GetFormattedBody()
        {
            return string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.NoFailureIssueBody,
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse));
        }

        public string GetFormattedTitle()
        {
            return string.Format(CultureInfo.InvariantCulture, Properties.Resources.NoFailureIssueTitle,
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse));
        }
    }
}
