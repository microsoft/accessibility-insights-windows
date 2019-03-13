// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace AccessibilityInsights.SetupLibrary
{
    /// <summary>
    /// Interface to streamline exception reporting
    /// </summary>
    public interface IExceptionReporter
    {
        /// <summary>
        /// Reports an error to an implementation-specific sink
        /// </summary>
        /// <param name="exception">The exception being reported</param>
        void ReportException(Exception exception);
    }
}
