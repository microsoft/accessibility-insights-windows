// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// Our Production ChannelInfoProvider
    /// </summary>
    internal class ProductionChannelInfoProvider : IChannelInfoProvider
    {
        private readonly IExceptionReporter _exceptionReporter;
        private readonly IGitHubWrapper _gitHubWrapper;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="gitHubWrapper">Provides access to GitHub</param>
        /// <param name="exceptionReporter">Provides a way to report exceptions</param>
        public ProductionChannelInfoProvider(IGitHubWrapper gitHubWrapper, IExceptionReporter exceptionReporter)
        {
            _gitHubWrapper = gitHubWrapper;
            _exceptionReporter = exceptionReporter;
        }

        /// <summary>
        /// Implements <see cref="IChannelInfoProvider.TryGetChannelInfo(string, out EnrichedChannelInfo)"/>
        /// </summary>
        public bool TryGetChannelInfo(ReleaseChannel releaseChannel, out EnrichedChannelInfo enrichedChannelInfo)
        {
            return ChannelInfoUtilities.TryGetChannelInfo(releaseChannel, out enrichedChannelInfo, _gitHubWrapper, exceptionReporter: _exceptionReporter);
        }
    }
}
