// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Wrap the GitHub API to decouple parsing from GitHub operations
    /// (and to facilitate testing)
    /// </summary>
    public interface IGitHubWrapper
    {
        /// <summary>
        /// Load the channel-specific channel information file into the Stream
        /// </summary>
        /// <param name="releaseChannel">The channel being requested</param>
        /// <param name="stream">The stream to populate</param>
        /// <returns>Metadata about the stream</returns>
        StreamMetadata LoadChannelInfoIntoStream(ReleaseChannel releaseChannel, Stream stream);
    }
}
