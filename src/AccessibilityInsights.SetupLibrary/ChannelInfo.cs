// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;

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
        /// The minimum supported production version
        /// </summary>
        [JsonProperty(PropertyName = "production_minimum_version")]
        public Version ProductionMinimumVersion { get; set; }

        /// <summary>
        /// The size, in bytes, of the MSI file associated with this release
        /// </summary>
        [JsonProperty(PropertyName = "msi_size_in_bytes")]
        public int MsiSizeInBytes { get; set; }

        /// <summary>
        /// The SHA512 of the MSI file associated with this release
        /// </summary>
        [JsonProperty(PropertyName = "msi_sha_512")]
        public string MsiSha512 { get; set; }

        /// <summary>
        /// Indicates if the object has values for all fields
        /// </summary>
        [JsonIgnore]
        public bool IsValid => CurrentVersion != null && MinimumVersion != null && CurrentVersion >= MinimumVersion && InstallAsset != null && ReleaseNotesAsset != null;
    }
}
