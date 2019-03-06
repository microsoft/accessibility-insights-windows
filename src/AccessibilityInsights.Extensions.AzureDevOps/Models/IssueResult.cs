// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOps.Models
{
    public class IssueResult : IIssueResult
    {
        public string DisplayText { get; set; }

        public Uri IssueLink { get; set; }
    }
}
