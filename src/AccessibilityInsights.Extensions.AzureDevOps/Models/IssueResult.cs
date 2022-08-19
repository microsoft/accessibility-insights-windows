// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    /// <summary>
    /// Contains data to be displayed in AI-Win after an issue is filed
    /// </summary>
    public class IssueResult : IIssueResult
    {
        /// <summary>
        /// Text to be displayed in UI
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Link to filed issue
        /// </summary>
        public Uri IssueLink { get; set; }
    }

    public class IssueResultWithPostAction : IssueResult, IIssueResultWithPostAction
    {
        /// <summary>
        /// Action to run on UI thread after issue filing is completed
        /// </summary>
        public Action PostAction { get; set; }
    }
}
