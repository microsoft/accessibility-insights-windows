// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary.REST;
using System;
using System.Globalization;
using System.IO;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Concrete implementation of IGitHubWrapper
    /// </summary>
    public class GitHubWrapper : IGitHubWrapper
    {
        private readonly Action<Exception> _exceptionReporter;
        private readonly Uri _configFileUri;
        private readonly TimeSpan _timeout;

        private const string DefaultConfigFileUrl = "https://www.github.com/Microsoft/accessibility-insights-windows/blob/Control/Channels/Production/release_info.json?raw=true";

        public GitHubWrapper(Action<Exception> exceptionReporter)
        {
            const double defaultTimeout = 60.0;
            OverridableConfig config = new OverridableConfig("GitHubWrapper.settings", exceptionReporter);
            string url = config.GetConfigSetting("ConfigFileUrl", DefaultConfigFileUrl);

            if (!Uri.TryCreate(url, UriKind.Absolute, out _configFileUri))
                _configFileUri = null;

            string configTimeout = config.GetConfigSetting("TimeoutInSeconds", string.Empty);
            if (!double.TryParse(configTimeout, NumberStyles.Number, CultureInfo.InvariantCulture, out double timeoutInSeconds) || timeoutInSeconds <= 0)
                timeoutInSeconds = defaultTimeout;

            _exceptionReporter = exceptionReporter;
            _timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        public bool TryGetSpecificAsset(Uri uri, Stream stream)
        {
            // Expect that the client's TryGet method will not leak Exceptions
            return GitHubClient.TryGet(uri, stream, _timeout, _exceptionReporter);
        }

        public bool TryGetChannelInfo(Stream stream)
        {
            // Expect that the client's TryGet method will not leak Exceptions
            return GitHubClient.TryGet(_configFileUri, stream, _timeout, _exceptionReporter);
        }
    }
}
