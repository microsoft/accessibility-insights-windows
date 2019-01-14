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
        [JsonProperty]
        public Guid InstallationGuid { get; private set; }

        [JsonProperty]
        public DateTime LastReset { get; private set; }

        /// <summary>
        /// Instantiates a new Guid and populates LastReset with the current time
        /// </summary>
        public InstallationInfo()
        {
            RegenerateFields();
        }

        /// <summary>
        /// Instantiates a new Guid and populates LastReset with the current time
        /// </summary>
        private void RegenerateFields()
        {
            this.InstallationGuid = Guid.NewGuid();
            this.LastReset = DateTime.UtcNow;
        }

        /// <summary>
        /// Resets this InstallationInfo object if the current month
        /// is later than the LastReset month
        /// </summary>
        /// <returns>whether the object reset</returns>
        private bool ResetIfMonthChanged()
        {
            bool needToReset = DateTime.UtcNow.Month > LastReset.Month || DateTime.UtcNow.Year > LastReset.Year;
            // months range from 1 to 12, year from 1 to 9999
            // reset if the current month is later than old month. 
            // - need to check year too in case we compare 12/2017 to 1/2018
            if (needToReset)
            {
                RegenerateFields();
            }
            return needToReset;
        }

        #region serialization
        /// <summary>
        /// Filename in which InstallationInfo information is persisted
        /// </summary>
        private const string InstallationInfoFileName = "InstallationInfo.json";

        /// <summary>
        /// Serializes this InstallationInfo object in json format to the given path
        /// </summary>
        /// <param name="path">file path in which to serialize the object</param>
        private void JsonSerialize(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json, Encoding.UTF8);
        }

        /// <summary>
        /// Attempts to deserialize a InstallationInfo object from the given directory path,
        /// returns a newly constructed InstallationInfo object otherwise
        /// 
        /// Resets/reserializes the InstallationInfo object if needed
        /// </summary>
        /// <param name="path">directory path from which to load the json</param>
        /// <returns></returns>
        public static InstallationInfo LoadFromPath(string path)
        {
            var installInfoPath = Path.Combine(path, InstallationInfo.InstallationInfoFileName);
            InstallationInfo info = new InstallationInfo();
            bool fileExists = File.Exists(installInfoPath);
            if (fileExists) {
                var text = File.ReadAllText(installInfoPath, Encoding.UTF8);
                try
                {
                    info = JsonConvert.DeserializeObject<InstallationInfo>(text);
                }
                catch (JsonException)
                {
                    // ignore silently
                }
            }

            bool reset = info.ResetIfMonthChanged();
            if (!fileExists || reset)
            {
                info.JsonSerialize(installInfoPath);
            }
            return info;
        }
        #endregion
    }
}
