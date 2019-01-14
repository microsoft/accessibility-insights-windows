// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Enums
{
    /// <summary>
    /// Views in Test Mode
    /// </summary>
    public enum TestView
    {
        NoSelection, // test mode without selection or results
        AutomatedTestResults,
        TabStop,
        CapturingData, // while snapshot is going on...
        ElementHowToFix, // when Inspect View is on by clicking element on Automated check list
        ElementDetails, // when Inspect view is on by selecting Inspect tab.
    }
}
