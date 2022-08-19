// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AccessibilityInsights.SharedUx.Utilities
{
    /// <summary>
    /// Encapsulates a semi-persistent guid helpful for
    /// tracking usage in telemetry
    ///
    /// The guid is reset when the month changes
    /// </summary>
    public class InstallationInfo
    {
        #region Unit test overrides
        internal static Func<string, InstallationInfo> ReadFromDiskOverride;
        internal static Action<string, InstallationInfo> WriteToDiskOverride;
        #endregion

        private static readonly DateTime DistantPast = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [JsonProperty]
        public Guid InstallationGuid { get; private set; }

        [JsonProperty]
        public DateTime LastReset { get; private set; }

        /// <summary>
        /// Production constructor - Uses a new installId and current time
        /// </summary>
        public InstallationInfo()
            : this(Guid.NewGuid(), DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Unit test constructor - lets caller specify installID and lastReset time
        /// </summary>
        /// <param name="installId">The initial value of InstallationGuid</param>
        /// <param name="lastReset">The initial value of LastReset</param>
        internal InstallationInfo(Guid installId, DateTime lastReset)
        {
            RegenerateFields(installId, lastReset);

        }
        /// <summary>
        /// Updates InstallationGuid and LastReset
        /// </summary>
        private void RegenerateFields(Guid installationGuid, DateTime lastReset)
        {
            this.InstallationGuid = installationGuid;
            this.LastReset = lastReset;
        }

        /// <summary>
        /// Resets this InstallationInfo object if the "utcNow" month
        /// is later than the "LastReset" month
        /// </summary>
        /// <returns>whether the object reset</returns>
        private bool ResetIfMonthChanged(DateTime utcNow)
        {
            // We need to reset unless we're in the same month and year
            bool needToReset = utcNow.Month != LastReset.Month || utcNow.Year != LastReset.Year;
            if (needToReset)
            {
                RegenerateFields(Guid.NewGuid(), utcNow);
            }
            return needToReset;
        }

        #region serialization
        /// <summary>
        /// Filename in which InstallationInfo information is persisted
        /// </summary>
        private const string InstallationInfoFileName = "InstallationInfo.json";

        /// <summary>
        /// Attempts to deserialize an InstallationInfo object from the given directory path,
        /// returns a newly constructed InstallationInfo object otherwise
        ///
        /// Resets/reserializes the InstallationInfo object if needed
        /// </summary>
        /// <param name="path">directory path from which to load the json</param>
        /// <param name="utcNow">The current UTC time</param>
        /// <returns>The current InstallationInfo object</returns>
        public static InstallationInfo LoadFromPath(string path, DateTime utcNow)
        {
            var installInfoPath = Path.Combine(path, InstallationInfoFileName);
            InstallationInfo info = ReadFromDisk(installInfoPath);

            if (info.ResetIfMonthChanged(utcNow))
            {
                WriteToDisk(installInfoPath, info);
            }
            return info;
        }

        private static InstallationInfo ReadFromDisk(string installInfoPath)
        {
            if (ReadFromDiskOverride != null)
            {
                return ReadFromDiskOverride(installInfoPath);
            }

            try
            {
                var text = File.ReadAllText(installInfoPath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<InstallationInfo>(text);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // ignore silently without sending to telemetry (not yet initialized)
                // Use a date in the distant past and reset will occur automatically
                return new InstallationInfo(Guid.NewGuid(), DistantPast);
            }
        }

        private static void WriteToDisk(string installInfoPath, InstallationInfo info)
        {
            if (WriteToDiskOverride != null)
            {
                WriteToDiskOverride(installInfoPath, info);
                return;
            }

            try
            {
                var json = JsonConvert.SerializeObject(info, Formatting.Indented);
                File.WriteAllText(installInfoPath, json, Encoding.UTF8);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                // ignore silently without sending to telemetry (not yet initialized)
            }
        }
        #endregion
    }
}
