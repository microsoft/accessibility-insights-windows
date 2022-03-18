// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    /// <summary>
    /// Abstraction to allow unit testing of LogFileWriter
    /// </summary>
    internal interface ILogFileHelper
    {
        /// <summary>
        /// Append the provided lines to the log file
        /// </summary>
        void AppendLinesToLogFile(IEnumerable<string> lines);

        /// <summary>
        /// Reset the log file for a new session
        /// </summary>
        void ResetLogFile();
    }
}
