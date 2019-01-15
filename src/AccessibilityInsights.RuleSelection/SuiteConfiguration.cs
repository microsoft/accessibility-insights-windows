// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.IO;

namespace AccessibilityInsights.RuleSelection
{
    /// <summary>
    /// Suite configuration
    /// </summary>
    public class SuiteConfiguration
    {
        public string Name { get; set; }

        /// <summary>
        /// Enabled
        /// if it is true, this suite will be picked up
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        public SuiteConfiguration() { }

        public SuiteConfiguration(FileInfo fi)
        {
            this.Name = fi.Name.ToUpperInvariant();
        }
    }
}
