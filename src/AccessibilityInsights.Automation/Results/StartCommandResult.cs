// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Globalization;

namespace AccessibilityInsights.Automation
{
    /// <summary>
    /// Result from a Start-AccessibilityInsights command
    /// </summary>
    public class StartCommandResult : SharedCommandResult
    {
        /// <summary>
        /// If true, AccessibilityInsights started successfully and is now ready for use
        /// </summary>
        public bool Succeeded { get; set; }

        /// <summary>
        /// Convert contents to a user-friendly description
        /// </summary>
        /// <returns>The user-friendly description</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "StartCommandResult: Succeeded={0}, {1}",
                Succeeded, ToStringProtected());
        }
    }
}
