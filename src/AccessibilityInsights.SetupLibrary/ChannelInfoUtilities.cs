// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Utilities to help fetch ChannelInfo objects
    /// </summary>
    public static class ChannelInfoUtilities
    {
        /// <summary>
        /// Given a stream containing a config file, get a specific channel
        /// </summary>
        /// <param name="stream">The stream containing the config file</param>
        /// <param name="keyName">The key name to use when parsing the data</param>
        /// <returns>The valid ChannelInfo</returns>
        public static ChannelInfo GetChannelFromStream(Stream stream, string keyName = "default")
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            stream.Position = 0;
#pragma warning disable CA2000 // Dispose objects before losing scope
            // Don't dispose the StreamReader here, since it has the side effect of also
            // disposing the passed-in Stream, which we don't own.
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
#pragma warning restore CA2000 // Dispose objects before losing scope
            string channelString = reader.ReadToEnd();

            Dictionary<string, ChannelInfo> convertedData = JsonConvert.DeserializeObject<Dictionary<string, ChannelInfo>>(channelString);

            if (convertedData.TryGetValue(keyName, out ChannelInfo channelInfo) &&
                channelInfo.IsValid)
            {
                return channelInfo;
            }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
            throw new InvalidDataException("Unable to get ChannelInfo");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        /// <summary>
        /// Given a ReleaseChannel and a keyName, attempt to load the corresponding ChannelInfo objecvt
        /// </summary>
        /// <param name="releaseChannel">The ReleaseChannel being queried</param>
        /// <param name="channelInfo">Returns the ChannelInfo here</param>
        /// <param name="gitHubWrapper">An optional wrapper to the GitHub data</param>
        /// <param name="keyName">An optional override of the key to use when reading the ChannelInfo data</param>
        /// <param name="exceptionReporter">An optional IExceptionReporter if you want exception details</param>
        /// <returns>true if we found data</returns>
        public static bool TryGetChannelInfo(ReleaseChannel releaseChannel, out ChannelInfo channelInfo, IGitHubWrapper gitHubWrapper, string keyName = "default", IExceptionReporter exceptionReporter = null)
        {
            try
            {
                IGitHubWrapper wrapper = gitHubWrapper ?? new GitHubWrapper(exceptionReporter);
                using (Stream stream = new MemoryStream())
                {
                    wrapper.LoadChannelInfoIntoStream(releaseChannel, stream);
                    channelInfo = GetChannelFromStream(stream, keyName);
                    return true;
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                if (exceptionReporter != null)
                {
                    exceptionReporter.ReportException(e);
                }
            }
#pragma warning restore CA1031 // Do not catch general exception types

            // Default values
            channelInfo = null;
            return false;
        }
    }
}
