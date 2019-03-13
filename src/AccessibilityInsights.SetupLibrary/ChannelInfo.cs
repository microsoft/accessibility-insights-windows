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
    /// Class to represent the state of a specific Release Channel. This allows
    /// us to use the same upgrade mechanism, but allow users some control over
    /// the rate of change of their client bits.
    /// </summary>
    public class ChannelInfo
    {
        /// <summary>
        /// The most recent version for this release channel
        /// </summary>
        [JsonProperty(PropertyName = "current_version")]
        public Version CurrentVersion { get; set; }

        /// <summary>
        /// The minimum required version for this release channel
        /// </summary>
        [JsonProperty(PropertyName = "minimum_version")]
        public Version MinimumVersion { get; set; }

        /// <summary>
        /// Path to the installer of the current version
        /// </summary>
        [JsonProperty(PropertyName = "installer_url")]
        public string InstallAsset { get; set; }

        /// <summary>
        /// Path to the release notes of the current version
        /// </summary>
        [JsonProperty(PropertyName = "release_notes_url")]
        public string ReleaseNotesAsset { get; set; }

        /// <summary>
        /// Indicates if the object has values for all fields
        /// </summary>
        [JsonIgnore]
        public bool IsValid => CurrentVersion != null && MinimumVersion != null && CurrentVersion >= MinimumVersion && InstallAsset != null && ReleaseNotesAsset != null;

        /// <summary>
        /// Given a stream containing a config file, try to find a specific channel
        /// </summary>
        /// <param name="stream">The stream containing the config file</param>
        /// <param name="requestedChannel">The channel being sought</param>
        /// <param name="channelInfo">The ChannelInfo that was located</param>
        /// <param name="exceptionReporter">Called to report exceptions</param>
        /// <returns>true if valid data was found, otherwise false</returns>
        public static bool TryGetChannelFromStream(Stream stream, string requestedChannel, out ChannelInfo channelInfo, IExceptionReporter exceptionReporter)
        {
            try
            {
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string channelString = reader.ReadToEnd();
                Dictionary<string, ChannelInfo> convertedData = JsonConvert.DeserializeObject<Dictionary<string, ChannelInfo>>(channelString);

                if (convertedData.TryGetValue(requestedChannel, out ChannelInfo rawChannelInfo) &&
                    rawChannelInfo.IsValid)
                {
                    channelInfo = rawChannelInfo;
                    return true;
                }
            }
            catch (Exception e)
            {
                exceptionReporter.ReportException(e);
            }

            channelInfo = null;
            return false;
        }
    }
}
