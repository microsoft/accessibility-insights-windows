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
        private readonly Uri _productionConfigFileUri;
        private readonly Uri _insiderConfigFileUri;
        private readonly Uri _canaryConfigFileUri;
        private readonly TimeSpan _timeout;

        private const string DefaultProductionConfigFileUrl = "https://www.github.com/Microsoft/accessibility-insights-windows/blob/Control/Channels/Production/release_info.json?raw=true";
        private const string DefaultInsiderConfigFileUrl = "https://www.github.com/Microsoft/accessibility-insights-windows/blob/Control/Channels/Insider/release_info.json?raw=true";
        private const string DefaultCanaryConfigFileUrl = "https://www.github.com/Microsoft/accessibility-insights-windows/blob/Canary/Channels/Canary/release_info.json?raw=true";

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="exceptionReporter">Mechanism for reporting recoverable exceptions from the config</param>
        public GitHubWrapper(IExceptionReporter exceptionReporter)
        {
            const double defaultTimeout = 60.0;
            OverridableConfig config = new OverridableConfig("GitHubWrapper.settings", exceptionReporter);

            _productionConfigFileUri = GetConfiguredUri(config, "ProductionConfigFileUrl", DefaultProductionConfigFileUrl);
            _insiderConfigFileUri = GetConfiguredUri(config, "InsiderConfigFileUrl", DefaultInsiderConfigFileUrl);
            _canaryConfigFileUri = GetConfiguredUri(config, "CanaryConfigFileUrl", DefaultCanaryConfigFileUrl);

            string configTimeout = config.GetConfigSetting("TimeoutInSeconds", string.Empty);
            if (!double.TryParse(configTimeout, NumberStyles.Number, CultureInfo.InvariantCulture, out double timeoutInSeconds) || timeoutInSeconds <= 0)
                timeoutInSeconds = defaultTimeout;

            _timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        /// <summary>
        /// Implements <see cref="IGitHubWrapper.LoadChannelInfoIntoStream(string, Stream)(Uri, Stream)"/>
        /// </summary>
        public void LoadChannelInfoIntoStream(ReleaseChannel releaseChannel, Stream stream)
        {
            GitHubClient.LoadUriContentsIntoStream(GetChannelSpecificUri(releaseChannel), stream, _timeout, null);
        }

        /// <summary>
        /// Gets the configured Uri for a given configurable setting
        /// </summary>
        /// <param name="config">The OverridableConfig object that contains the settings</param>
        /// <param name="key">The key to seek</param>
        /// <param name="defaultValue">The default Uri (represented as a string)</param>
        /// <returns>A valid Uri if possible, null if not</returns>
        private static Uri GetConfiguredUri(OverridableConfig config, string key, string defaultValue)
        {
            string url = config.GetConfigSetting(key, defaultValue);

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                uri = null;

            return uri;
        }

        private Uri GetChannelSpecificUri(ReleaseChannel releaseChannel)
        {
            // Check for special cases, otherwise use production value
            switch (releaseChannel)
            {
                case ReleaseChannel.Insider: return _insiderConfigFileUri;
                case ReleaseChannel.Canary: return _canaryConfigFileUri;
            }

            return _productionConfigFileUri;
        }
    }
}
