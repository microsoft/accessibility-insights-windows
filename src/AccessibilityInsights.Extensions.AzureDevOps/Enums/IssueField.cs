// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Extensions.AzureDevOps.Enums
{
    /// <summary>
    /// These values are exposed so that they can be used in issue-filing template strings
    /// </summary>
    public enum IssueField
    {
        WindowTitle,    // title of window that element belongs to
        Glimpse,
        HowToFixLink,   // snippet query URL
        HelpURL,
        RuleSource,     // MSDN, A11y, etc
        RuleDescription,
        TestMessages,    // Messages shown in "Fix the following"
        ProcessName,
        ScreenshotLink,
        InternalGuid, // Guid used internally when filing issues
        ElementPath, // multi-line string of glimpses from ancestor to current element
        RuleForTelemetry,
        UIFramework,
        ContrastRatio,
        FirstColorHex,
        SecondColorHex,
        ContrastFailureText,
    }
}
