// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;

namespace AccessibilityInsights.Extensions.GitHub
{
    public class IssueResult : IIssueResult
    {
        public IssueResult(String displayText, Uri issueLink)
        {
            this.DisplayText = displayText;
            this.IssueLink = issueLink;
        }
        public string DisplayText { get; }

        public Uri IssueLink { get; }
    }
}
