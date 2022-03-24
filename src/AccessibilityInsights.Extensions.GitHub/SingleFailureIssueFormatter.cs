// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System.Globalization;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Single Issue Formatting
    /// </summary>
    public class SingleFailureIssueFormatter : IIssueFormatter
    {
        public IssueInformation IssueInfo { get; }
        public SingleFailureIssueFormatter(IssueInformation issueInfo)
        {
            this.IssueInfo = issueInfo;
        }
        public string GetFormattedBody()
        {
            return string.Format(CultureInfo.InvariantCulture,
                Properties.Resources.SingleFailureIssueBody,
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleDescription),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleSource),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.HelpUri.ToString()),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.TestMessages));
        }

        public string GetFormattedTitle()
        {
            return string.Format(CultureInfo.InvariantCulture, Properties.Resources.SingleFailureIssueTitle,
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleSource),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleDescription));
        }
    }
}
