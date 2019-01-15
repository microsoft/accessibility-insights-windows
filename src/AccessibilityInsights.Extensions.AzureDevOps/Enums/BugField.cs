// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Extensions.AzureDevOps.Enums
{
    /// <summary>
    /// These values are exposed so that they can be used in bug-filing template strings
    /// </summary>
    public enum BugField
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
        InternalGuid, // Guid used internally when filing bugs
        ElementPath, // multi-line string of glimpses from ancestor to current element
        RuleForTelemetry,
        UIFramework,
        ContrastRatio,
        FirstColorHex,
        SecondColorHex,
        ContrastFailureText,
    }
}
