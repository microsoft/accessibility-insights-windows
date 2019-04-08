// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Globalization;

namespace Axe.Windows.Automation
{
    /// <summary>
    /// The base class for all command results
    /// </summary>
    public class SharedCommandResult
    {
        /// <summary>
        /// Did the command complete? This is different from the specific command status
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// The summary of the command (content and format may vary with the specific command)
        /// </summary>
        public string SummaryMessage { get; set; }

        /// <summary>
        /// Render this object's contents as a human-readable string
        /// </summary>
        /// <returns>A human-friendly representation of the object contents</returns>
        protected string ToStringProtected()
        {
            return string.Format(CultureInfo.InvariantCulture, "Completed={0}, SummaryMessage={1}", Completed, SummaryMessage);
        }
    }
}
