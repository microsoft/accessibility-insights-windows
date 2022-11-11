// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.SetupLibrary;
using System;

namespace AccessibilityInsights.Extensions.GitHubAutoUpdate
{
    /// <summary>
    /// Simple class to bridge from SetupLibrary's IExceptionReporter to Extension helper
    /// </summary>
    internal class ExceptionReporter : IExceptionReporter
    {
        /// <summary>
        /// Reports an error to the telemetry extension methods
        /// </summary>
        /// <param name="exception">The exception being reported</param>
        public void ReportException(Exception exception)
        {
            exception?.ReportException();
        }
    }
}
