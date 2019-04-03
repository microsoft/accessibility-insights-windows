// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Enums to provide telemetry when a user changes channels
    /// </summary>
    public enum ReleaseChannelChangeResult
    {
        /// <summary>
        /// Our default value -- should never be used in the code
        /// </summary>
        Unknown,

        /// <summary>
        /// The user canceled at the confirmation dialog
        /// </summary>
        UserCanceled,

        /// <summary>
        /// User confirmed, but we were unable to launch the Version Switcher
        /// </summary>
        VersionSwitcherLaunchFailed,

        /// <summary>
        /// User confirmed and the Version Switcher launched successfully
        /// </summary>
        VersionSwitcherLaunchSucceeded,
    }
}
