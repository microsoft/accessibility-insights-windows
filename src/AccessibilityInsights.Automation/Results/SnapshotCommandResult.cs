// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Globalization;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Result from Invoke-Snapshopt command
    /// </summary>
    public class SnapshotCommandResult : SharedCommandResult
    {
        /// <summary>
        /// The count of successful scan results
        /// </summary>
        public int ScanResultsPassed { get; set; }

        /// <summary>
        /// The count of failed scan results
        /// </summary>
        public int ScanResultsFailed { get; set; }

        /// <summary>
        /// The count of inconclusive scan results
        /// </summary>
        public int ScanResultsInconclusive { get; set; }

        /// <summary>
        /// The count of non-supported (typically web content) scan results
        /// </summary>
        public int ScanResultsUnsupported { get; set; }

        /// <summary>
        /// The sum of the reported results
        /// </summary>
        public int ScanResultsTotal { get; set; }

        /// <summary>
        /// Convert contents to a user-friendly description
        /// </summary>
        /// <returns>The user-friendly description</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SnapshotCommandResult: ScanResultsPassed={0}, ScanResultsInconclusive={1}, ScanResultsFailed={2}, ScanResultsUnsupported={3}, ScanResultsTotal={4}, {5}",
                ScanResultsPassed, ScanResultsInconclusive, ScanResultsFailed, ScanResultsUnsupported, ScanResultsTotal, ToStringProtected());
        }
    }
}
