// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Extensions.GitHub
{
    /// <summary>
    /// GitHub Issue Formatting Interface
    /// </summary>
    public interface IIssueFormatter
    {
        string GetFormattedTitle();
        string GetFormattedBody();
    }
}
