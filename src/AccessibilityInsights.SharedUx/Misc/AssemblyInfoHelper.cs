// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Telemetry;
using System;

namespace AccessibilityInsights.SharedUx.Misc
{
    /// <summary>
    /// Class to pass as an IExceptionReporter when calling into SetupLibrary
    /// </summary>
    public class SetupExceptionReporter : IExceptionReporter
    {
        /// <summary>
        /// Implements <see cref="IExceptionReporter.ReportException(Exception)"/>
        /// </summary>
        /// <param name="exception">The Exception being reported</param>
        public void ReportException(Exception exception)
        {
            exception.ReportException();
        }
    }
}
