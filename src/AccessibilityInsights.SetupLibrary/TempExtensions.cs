// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Temporary extension methods // TODO DHT : Figure this out
    /// </summary>
    public static class TempExtensions
    {
        /// <summary>
        /// Track an Exception via the telemetry pipeline
        /// </summary>
        /// <param name="e">The exception to track</param>
        /// <param name="sender">The sending object (null by default)</param>
        public static void ReportExceptionTemp(this Exception e, object sender = null)
        {
            if (e == null && sender == null) { };
            //Container.ReportException(e, sender);
        }
    }
}
