// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Extensions.Interfaces.Upgrades
{
    /// <summary>
    /// Values to indicate the current upgrade status
    /// </summary>
    public enum AutoUpdateOption
    {
        /// <summary>
        /// Status can not be determined
        /// </summary>
        Unknown,

        /// <summary>
        /// Current installation is ahead of the current version
        /// </summary>
        NewerThanCurrent,

        /// <summary>
        /// No upgrade exists
        /// </summary>
        Current,

        /// <summary>
        /// An optional upgrade exists
        /// </summary>
        OptionalUpgrade,

        /// <summary>
        /// A required upgrade exists
        /// </summary>
        RequiredUpgrade
    }
}
