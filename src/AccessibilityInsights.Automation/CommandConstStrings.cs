// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// This class groups all of the Command-related strings into a single location
    /// </summary>
    public static class CommandConstStrings
    {
        /// <summary>
        /// Key to specify the target to scan (value is the process ID in decimal format)
        /// </summary>
        public const string TargetProcessId = "TargetProcessId";

        /// <summary>
        /// Key to specify the output path for the scans (path will be created if it doesn't already exist)
        /// </summary>
        public const string OutputPath = "OutputPath";

        /// <summary>
        /// Key to specify the file to receive the scan
        /// </summary>
        public const string OutputFile = "OutputFile";

        /// <summary>
        /// Key to specify the format of the scan result
        /// </summary>
        public const string OutputFileFormat = "OutputFileFormat";

        /// <summary>
        /// Key to specify the data retention is no violations are found
        /// </summary>
        public const string NoViolationPolicy = "NoViolationPolicy";

        /// <summary>
        /// Discard data if no violations are found (value for NoViolationPolicy key)
        /// </summary>
        public const string Discard = "Discard";

        /// <summary>
        /// Retain the data even if no violations are found (value for NoViolationPolicy key)
        /// </summary>
        public const string Retain = "Retain";


        /// <summary>
        /// Set the team name in telemetry
        /// </summary>
        public const string TeamName = "TeamName";
    }
}
