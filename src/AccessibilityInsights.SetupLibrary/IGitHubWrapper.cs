// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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
        /// Given a link to an asset, load its contents into the provided Stream
        /// </summary>
        /// <param name="uri">Uri to the specific asset (assumed from metadata)</param>
        /// <param name="stream">Where the contents of the asset should be put</param>
        void LoadUriContentsIntoStream(Uri uri, Stream stream);

        /// <summary>
        /// Load the channel-specific channel information file into the Stream
        /// </summary>
        /// <param name="releaseChannel">The channel being requested</param>
        /// <param name="stream">The stream to populate</param>
        void LoadChannelInfoIntoStream(string releaseChannel, Stream stream);
    }
}
