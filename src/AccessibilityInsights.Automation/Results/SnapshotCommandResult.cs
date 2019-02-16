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
        public int ScanResultsPassedCount { get; set; }

        /// <summary>
        /// The count of failed scan results
        /// </summary>
        public int ScanResultsFailedCount { get; set; }

        /// <summary>
        /// The count of inconclusive scan results
        /// </summary>
        public int ScanResultsInconclusiveCount { get; set; }

        /// <summary>
        /// The count of non-supported (typically web content) scan results
        /// </summary>
        public int ScanResultsUnsupportedCount { get; set; }

        /// <summary>
        /// The sum of the reported results
        /// </summary>
        public int ScanResultsTotalCount { get; set; }

        /// <summary>
        /// Convert contents to a user-friendly description
        /// </summary>
        /// <returns>The user-friendly description</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SnapshotCommandResult: ScanResultsPassedCount={0}, ScanResultsInconclusiveCount={1}, ScanResultsFailedCount={2}, ScanResultsUnsupportedCount={3}, ScanResultsTotalCount={4}, {5}",
                ScanResultsPassedCount, ScanResultsInconclusiveCount, ScanResultsFailedCount, ScanResultsUnsupportedCount, ScanResultsTotalCount, ToStringProtected());
        }
    }
}
