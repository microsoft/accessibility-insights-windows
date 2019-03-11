// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SharedUx.Enums
{
    /// <summary>
    /// The release channel for a specific client. Channels appear in increasing upgrade frequency
    /// </summary>
    public enum ReleaseChannel
    {
        Production, // Least frequent upgrades (default value)
        Insiders,
        Canary,     // Most frequent upgrades
    }
}
