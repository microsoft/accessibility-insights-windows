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
    /// Utilities to help fetch EnrichedChannelInfo objects
    /// </summary>
    public static class ChannelInfoUtilities
    {
        /// <summary>
        /// Given a stream containing a config file, get a specific channel
        /// </summary>
        /// <param name="stream">The stream containing the config file</param>
        /// <param name="keyName">The key name to use when parsing the data</param>
        /// <returns>The valid EnrichedChannelInfo</returns>
        public static EnrichedChannelInfo GetChannelFromStream(Stream stream, string keyName = "default")
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // TODO: Remove GetChannelInfoFromLegacyManifest when we deprecate unsigned manifests
            EnrichedChannelInfo info = GetChannelInfoFromSignedManifest(stream) ??
                GetChannelInfoFromLegacyManifest(stream, keyName);

#pragma warning disable CA1303 // Do not pass literals as localized parameters
            return info ?? throw new InvalidDataException("Unable to get ChannelInfo");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
        }

        internal static EnrichedChannelInfo GetChannelInfoFromLegacyManifest(Stream stream, string keyName)
        {
            stream.Position = 0;
#pragma warning disable CA2000 // Dispose objects before losing scope
            // Don't dispose the StreamReader here, since it has the side effect of also
            // disposing the passed-in Stream, which we don't own.
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
#pragma warning restore CA2000 // Dispose objects before losing scope
            string channelString = reader.ReadToEnd();

            Dictionary<string, EnrichedChannelInfo> convertedData = JsonConvert.DeserializeObject<Dictionary<string, EnrichedChannelInfo>>(channelString);

            if (convertedData.TryGetValue(keyName, out EnrichedChannelInfo enrichedChannelInfo) &&
                enrichedChannelInfo.IsValid)
            {
                return enrichedChannelInfo;
            }

            return null;
        }

        private static bool IsStreamTrusted(Stream stream)
        {
            using (TrustVerifier verifier = new TrustVerifier(stream))
            {
                return verifier.IsVerified;
            }
        }

        internal static EnrichedChannelInfo GetChannelInfoFromSignedManifest(Stream stream, Func<Stream, bool> streamTrustCheck = null)
        {
            Func<Stream, bool> trustCheck = streamTrustCheck ?? IsStreamTrusted;

            if (!trustCheck(stream))
            {
                return null; // TODO: Capture this case in telemetry when we deprecate unsigned manifests
            }

            stream.Position = 0;
            byte[] bytes = FileHelpers.ExtractResourceFromStream(stream, "AccessibilityInsights.Manifest.ReleaseInfo.json");
            string json = StringFromResourceByteArray(bytes);
            EnrichedChannelInfo info = JsonConvert.DeserializeObject<EnrichedChannelInfo>(json);
            return info;
        }

        private static string StringFromResourceByteArray(byte[] bytes)
        {
            // Skip the Byte Order Mark when deserializing, since it's invalid JSON
            int index = (bytes[0] == 0xFF & bytes[1] == 0xFE) ? 2 : 0;
            return Encoding.Unicode.GetString(bytes, index, bytes.Length - index);
        }

        /// <summary>
        /// Given a ReleaseChannel and a keyName, attempt to load the corresponding EnrichedChannelInfo object
        /// </summary>
        /// <param name="releaseChannel">The ReleaseChannel being queried</param>
        /// <param name="enrichedChannelInfo">Returns the EnrichedChannelInfo here</param>
        /// <param name="gitHubWrapper">An optional wrapper to the GitHub data</param>
        /// <param name="keyName">An optional override of the key to use when reading the EnrichedChannelInfo data</param>
        /// <param name="exceptionReporter">An optional IExceptionReporter if you want exception details</param>
        /// <returns>true if we found data</returns>
        public static bool TryGetChannelInfo(ReleaseChannel releaseChannel, out EnrichedChannelInfo enrichedChannelInfo, IGitHubWrapper gitHubWrapper, string keyName = "default", IExceptionReporter exceptionReporter = null)
        {
            try
            {
                IGitHubWrapper wrapper = gitHubWrapper ?? new GitHubWrapper(exceptionReporter);
                using (Stream stream = new MemoryStream())
                {
                    StreamMetadata streamMetadata = wrapper.LoadChannelInfoIntoStream(releaseChannel, stream);
                    enrichedChannelInfo = GetChannelFromStream(stream, keyName);
                    enrichedChannelInfo.Metadata = streamMetadata;

                    if (enrichedChannelInfo.MinimumVersion == null)
                    {
                        if (releaseChannel == ReleaseChannel.Production)
                        {
                            enrichedChannelInfo.MinimumVersion = enrichedChannelInfo.ProductionMinimumVersion;
                        }
                        else
                        {
                            enrichedChannelInfo.MinimumVersion = enrichedChannelInfo.CurrentVersion;
                        }
                    }
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
            enrichedChannelInfo = null;
            return false;
        }
    }
}
