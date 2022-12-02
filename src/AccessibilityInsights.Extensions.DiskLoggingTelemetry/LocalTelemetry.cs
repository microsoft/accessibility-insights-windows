// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace AccessibilityInsights.Extensions.DiskLoggingTelemetry
{
    /// <summary>
    /// Disk logging implementation of ITelemetry
    /// </summary>
    [Export(typeof(ITelemetry))]
    public class LocalTelemetry : ITelemetry
    {
        private readonly Dictionary<string, string> _contextProperties = new Dictionary<string, string>();
        private readonly ILogWriter _logWriter;

#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        /// <summary>
        /// Production constructor--must be public for MEF
        /// </summary>
        public LocalTelemetry()
            : this(new LogWriter(() => DateTime.UtcNow, new LogFileHelper()))
        {
        }

        internal LocalTelemetry(ILogWriter logWriter)
        {
            _logWriter = logWriter ?? throw new ArgumentNullException(nameof(logWriter));
        }
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]

        /// <summary>
        /// Writes telemetry to a local disk log
        /// </summary>
        /// <param name="eventName">Will be used as event name</param>
        /// <param name="properties">if null, doesn't send any additional property values beyond context</param>
        public void PublishEvent(string eventName, IReadOnlyDictionary<string, string> properties = null)
        {
            PublishedEventData eventData = new PublishedEventData
            {
                EventName = eventName,
                ContextProperties = _contextProperties,
            };

            if (properties != null)
            {
                eventData.EventProperties = new Dictionary<string, string>(CopyToDictionary(properties));
            }

            _logWriter.LogThisData("Event was published",
                JsonConvert.SerializeObject(eventData, Formatting.Indented));
        }

        private static Dictionary<string, string> CopyToDictionary(IReadOnlyDictionary<string, string> original)
        {
            Dictionary<string, string> copy = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> pair in original)
            {
                copy.Add(pair.Key, pair.Value);
            }

            return copy;
        }

        /// <summary>
        /// Sets property/value pair of context; all values in context are sent in telemetry events
        /// </summary>
        public void AddOrUpdateContextProperty(string propertyName, string propertyValue)
        {
            _contextProperties[propertyName] = propertyValue;
        }

        /// <summary>
        /// Log a reported Exception
        /// </summary>
        public void ReportException(Exception e)
        {
            ReportedExceptionData exceptionData = new ReportedExceptionData
            {
                ContextProperties = _contextProperties,
                Exception = e,
            };

            _logWriter.LogThisData("Exception was reported",
                JsonConvert.SerializeObject(exceptionData, Formatting.Indented));
        }

        /// <summary>
        /// Application is shutting down, so flush any pending telemetry
        /// </summary>
        public void FlushAndShutDown()
        {
            // We never cache telemetry, so this does nothing
        }
    }
}
