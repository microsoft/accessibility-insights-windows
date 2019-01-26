// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// Wrap the GitHub API to decouple parsing from GitHub operations
    /// (and to facilitate testing)
    /// </summary>
    internal interface IGitHubWrapper
    {
        /// <summary>
        /// Given a link to an asset, download its contents into the provided stream
        /// </summary>
        /// <param name="uri">Uri to the specific asset (assumed from metadata)</param>
        /// <param name="stream">Where the contents of the asset should be put</param>
        /// <returns>true if the call succeeded</returns>
        bool TryGetSpecificAsset(Uri uri, Stream stream);

        /// <summary>
        /// Attempt to get the Config Info. This will be pulled from an online source
        /// </summary>
        /// <param name="stream">The stream to populate</param>
        /// <returns>true if the config was successfully read</returns>
        bool TryGetConfigInfo(Stream stream);
    }
}
