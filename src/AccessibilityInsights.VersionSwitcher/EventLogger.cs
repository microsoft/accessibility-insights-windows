// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Globalization;

namespace AccessibilityInsights.VersionSwitcher
{
    /// <summary>
    /// Class to put system event logging into a single location. This class
    /// will be used to generate output that we can use for diagnostics of
    /// upgrade-related issues
    /// </summary>
    internal class EventLogger : IDisposable
    {
        private const string EventSource = "AccessibilityInsights.VersionSwitcher";
        private const string ApplicationLog = "Application";

        // This gives us an easy static option without making the whole class static
        private static readonly Lazy<EventLogger> Lazy = new Lazy<EventLogger>(() => new EventLogger());

        private EventLog _log;

        /// <summary>
        /// ctor
        /// </summary>
        internal EventLogger()
        {
            // Ensure that we have a valid source
            if (!EventLog.SourceExists(EventSource))
                EventLog.CreateEventSource(EventSource, ApplicationLog);

            _log = new EventLog
            {
                Source = EventSource,
                EnableRaisingEvents = true,
            };
        }

        /// <summary>
        /// Write the message to the event log
        /// </summary>
        /// <param name="entryType">The entry type</param>
        /// <param name="messageFormat">The message formatting string</param>
        /// <param name="args">optional parameter list for formatting string</param>
        private void WriteToLog(EventLogEntryType entryType, string messageFormat, params object[] args)
        {
            string message = string.Format(CultureInfo.InvariantCulture, messageFormat, args);
            _log.WriteEntry(message, entryType);
        }

        /// <summary>
        /// Write an error message to the event log
        /// </summary>
        /// <param name="messageFormat">The message formatting string</param>
        /// <param name="args">optional parameter list for formatting string</param>
        internal static void WriteErrorMessage(string messageFormat, params object[] args)
        {
            Lazy.Value.WriteToLog(EventLogEntryType.Error, messageFormat, args);
        }

        /// <summary>
        /// Write a warning message to the event log
        /// </summary>
        /// <param name="messageFormat">The message formatting string</param>
        /// <param name="args">optional parameter list for formatting string</param>
        internal static void WriteWarningMessage(string messageFormat, params object[] args)
        {
            Lazy.Value.WriteToLog(EventLogEntryType.Warning, messageFormat, args);
        }

        /// <summary>
        /// Write an informational message to the event log
        /// </summary>
        /// <param name="messageFormat">The message formatting string</param>
        /// <param name="args">optional parameter list for formatting string</param>
        internal static void WriteInformationalMessage(string messageFormat, params object[] args)
        {
            Lazy.Value.WriteToLog(EventLogEntryType.Information, messageFormat, args);
        }

        #region IDisposable Support

        private bool IsDisposed { get ; set ; }

        private void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                if (_log != null)
                {
                    _log.Dispose();
                    _log = null;
                }

                IsDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
