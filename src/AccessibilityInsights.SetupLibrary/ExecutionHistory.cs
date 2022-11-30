// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Class to capture the execution history of the VersionSwitcher. Gets serialized to disk by
    /// the VersionSwitcher, then gets uploaded to telemetry the next time the app boots
    /// </summary>
    public class ExecutionHistory
    {
        public static string GetDataFilePath()
        {
            const string DataFile = "AccessibilityInsights.VersionSwitcher.ExecutionHistory.json";
            return Path.Combine(Path.GetTempPath(), DataFile);
        }

        private ExecutionResult _typedExecutionResult;

        public ExecutionHistory()
        {
            LocalDetails = new List<string>();

            // ExecutionResult is uninitialized by default. Set the TypedExecutionResult
            // to properly initialize ExecutionResult
            TypedExecutionResult = _typedExecutionResult;
        }

        public void AddLocalDetail(string detail)
        {
            LocalDetails.Add(detail);
        }

        public void AddLocalDetail(string format, params object[] args)
        {
            string detail = string.Format(CultureInfo.InvariantCulture, format, args);
            AddLocalDetail(detail);
        }

        [JsonIgnore]
        public ExecutionResult TypedExecutionResult
        {
            get => _typedExecutionResult;
            set
            {
                _typedExecutionResult = value;
                ExecutionResult = value.ToString();
            }
        }

        public string StartingVersion { get; set; }

        public string ExecutionResult { get; set; }

        public string RequestedMsi { get; set; }

        public string ResolvedMsi { get; set; }

        public int ExpectedMsiSizeInBytes { get; set; }

        public int ActualMsiSizeInBytes { get; set; }

        public string ExpectedMsiSha512 { get; set; }

        public string ActualMsiSha512 { get; set; }

        public string NewChannel { get; set; }

        public long ExecutionTimeInMilliseconds { get; set; }

        public List<string> LocalDetails { get; set; }
    }
}
