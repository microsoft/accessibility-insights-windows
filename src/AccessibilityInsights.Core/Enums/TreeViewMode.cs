// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Core.Enums
{
    /// <summary>
    /// Mode for TreeWalking
    /// Raw: Everything
    /// Control: ...
    /// Content: ...
    /// Based on UIAutomation definition. but we can use it for other platforms later.
    /// </summary>
    public enum TreeViewMode
    {
        Raw,
        Control,
        Content
    }
}
