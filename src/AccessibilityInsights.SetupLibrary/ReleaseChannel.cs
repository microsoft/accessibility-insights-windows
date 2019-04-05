// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// The release channel for a specific client. Channels appear in increasing upgrade frequency
    /// </summary>
    public enum ReleaseChannel
    {
        Production, // Least frequent upgrades (default value)
        Insider,
        Canary,     // Most frequent upgrades
    }
}
