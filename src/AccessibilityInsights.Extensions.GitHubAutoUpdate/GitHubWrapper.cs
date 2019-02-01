// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.GitHubAutoUpdate.REST;
using System;
using System.Globalization;
using System.IO;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// Concrete implementation of IGitHubWrapper
    /// </summary>
    public class GitHubWrapper : IGitHubWrapper
    {
        private readonly Uri _configFileUri;
        private readonly TimeSpan _timeout;

        private const string DefaultConfigFileUrl = "";  // TODO: Populate with actual path once we have it

        public GitHubWrapper()
        {
            const double defaultTimeout = 60.0;
            OverridableConfig config = new OverridableConfig("GitHubUpdate.settings");
            string url = config.GetConfigSetting("ConfigFileUrl", DefaultConfigFileUrl);

            if (!Uri.TryCreate(url, UriKind.Absolute, out _configFileUri))
                _configFileUri = null;

            string configTimeout = config.GetConfigSetting("TimeoutInSeconds", string.Empty);
            if (!double.TryParse(configTimeout, NumberStyles.Number, CultureInfo.InvariantCulture, out double timeoutInSeconds) || timeoutInSeconds <= 0)
                timeoutInSeconds = defaultTimeout;

            _timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        public bool TryGetSpecificAsset(Uri uri, Stream stream)
        {
            // Expect that the client's TryGet method will not leak Exceptions
            return GitHubClient.TryGet(uri, stream, _timeout);
        }

        public bool TryGetConfigInfo(Stream stream)
        {
            // Expect that the client's TryGet method will not leak Exceptions
            return GitHubClient.TryGet(_configFileUri, stream, _timeout);
        }
    }
}
