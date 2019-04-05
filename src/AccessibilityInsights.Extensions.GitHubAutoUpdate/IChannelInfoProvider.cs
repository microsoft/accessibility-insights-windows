// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// Interface to allow stubbing out of the code to fetch the ChannelInfo object
    /// </summary>
    internal interface IChannelInfoProvider
    {
        /// <summary>
        /// Attempt to load a ChannelInfo (not necessarily completely valid) for the given channel
        /// </summary>
        /// <param name="releaseChannel">The channel being requested</param>
        /// <param name="channelInfo">The ChannelInfo</param>
        /// <returns>true if a ChannelInfo was found</returns>
        bool TryGetChannelInfo(ReleaseChannel releaseChannel, out ChannelInfo channelInfo);
    }
}
