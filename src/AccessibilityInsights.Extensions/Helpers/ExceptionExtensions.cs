// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.Extensions.Helpers
{
    /// <summary>
    /// Extension methods around Exceptions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Track an Exception via the telemetry pipeline
        /// </summary>
        /// <param name="e">The exception to track</param>
        /// <param name="sender">The sending object (null by default)</param>
        public static void ReportException(this Exception e, object sender = null)
        {
            Container.ReportException(e, sender);
        }
    }
}
