// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class IssueFormatter
    {
        public static string GetFormattedString(string URL, IssueInformation issueInfo)
        {
            string PostCallURL = URL+"?issues/new?" + "title=" + GetTitle(issueInfo) + "&" + GetBody(issueInfo);
            return PostCallURL;
        }

        private static string GetBody(IssueInformation issueInfo)
        {
            return GetStringValue(issueInfo.WindowTitle);
        }

        private static string GetTitle(IssueInformation issueInfo)
        {
            return GetStringValue(issueInfo.WindowTitle);
        }

        private static string GetStringValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            return value;
        }
    }
}
