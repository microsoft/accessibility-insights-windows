// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue No Issue Formatting
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
            return string.Format(
                "The following accessibility issue needs investigation.\n\n" +
                "**App:** {0}\n\n" +
                "**Element path:** {1}\n\n" +
                "**Issue Details:** []\n\n" +
                "**How To Fix:** []\n\n",
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse));
        }

        public string GetFormattedTitle()
        {
            return string.Format("({0}/{1}) has an accessibility issue that needs investigation",
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse));
        }
    }
}
