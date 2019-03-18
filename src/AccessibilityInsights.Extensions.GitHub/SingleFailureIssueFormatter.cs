// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class SingleFailureIssueFormatter:IIssueFormatter
    {
        public IssueInformation IssueInfo { get; }
        public SingleFailureIssueFormatter(IssueInformation issueInfo)
        {
            this.IssueInfo = issueInfo;
        }
        public string GetFormattedBody()
        {
            return string.Format(
                "The following accessibility issue that needs investigation.\n\n" +
                "**App:** {0}\n\n" +
                "**Element path:** {1}\n\n" +
                "**Issue Details:** {2} [{3}]({4})\n\n" +
                "**How To Fix:** {5}\n\n" +
                "This accessibility issue was found using Accessibility Insights for Windows, a tool that helps debug and find accessibility issues earlier. Get more information and download this tool at https://aka.ms/AccessibilityInsights.",
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleSource),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.HelpUri.ToString()),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleDescription),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.TestMessages));
        }

        public string GetFormattedTitle()
        {
            return string.Format("{0}: ({1}/{2}) {3}",
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleSource),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.ProcessName),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.Glimpse),
                IssueFormatterFactory.GetStringValue(this.IssueInfo.RuleDescription));
        }
    }
}
