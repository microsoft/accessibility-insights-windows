// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace AccessibilityInsights.Extensions.Interfaces.Upgrades
{
    /// <summary>
    /// Class to provide the ReleaseChannel to implementations
    /// </summary>
    public static class ReleaseChannelProvider
    {
        /// <summary>
        /// The ReleaseChannel to be used by AutoUpdate
        /// </summary>
        public static string ReleaseChannel { get; internal set; }
    }
}
