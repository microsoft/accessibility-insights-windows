// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.Text;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Windows Event Logger
    /// Add Event entry into Windows ETW
    /// </summary>
    public static class WindowsEventLogger
    {
        /// <summary>
        /// using existing value to avoid security exception
        /// since ETW source creation and usage can't be done in sequence.
        /// </summary>
        const string LogTarget = "Application";
        const string LogSource = "Application";

        /// <summary>
        /// Write a Log entry to ETW
        /// it will be written into Windows Event log under Windows logs/Application
        /// </summary>
        /// <param name="logType">Log entry type</param>
        /// <param name="title">title of log</param>
        /// <param name="message">message of log</param>
        public static void WriteLogEntry(EventLogEntryType logType, string title, string message)
        {
            using (EventLog eventLog = new EventLog(LogTarget))
            {
                eventLog.Source = LogSource;
                StringBuilder sb = new StringBuilder(title);
                sb.AppendLine();
                sb.AppendLine(message);
                eventLog.WriteEntry(sb.ToString(), logType);
            }
        }
    }
}
