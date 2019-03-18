// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;

namespace AccessibilityInsights.Extensions.GitHub
{
    public interface IIssueFormatter
    {
        string GetFormattedTitle();
        string GetFormattedBody();

    }
}
