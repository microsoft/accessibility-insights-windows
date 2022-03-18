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
    /// Application Insights-based implementation of ITelemetry
    /// </summary>
    [Export(typeof(ITelemetry))]
    public class LocalTelemetry : ITelemetry
    {
        private Dictionary<string, string> _contextProperties = new Dictionary<string, string>();
        private FileWrapper _fileWrapper;

#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        /// <summary>
        /// Production ctor--must be public for MEF
        /// </summary>
        public LocalTelemetry()
            : this(new FileWrapper(() => DateTime.UtcNow))
        {
        }

        internal LocalTelemetry(FileWrapper fileWrapper)
        {
            _fileWrapper = fileWrapper;
            _fileWrapper.InitializeFile();
        }
#pragma warning restore RS0034 // Exported parts should have [ImportingConstructor]

        /// <summary>
        /// Publishes telemetry to AI with the given action and propertybag
        /// Sent telemetry will also include all context values unless they are overwritten
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

            _fileWrapper.LogThisData("Event was published",
                JsonConvert.SerializeObject(eventData, Formatting.Indented));
        }

        private static Dictionary<string, string> CopyToDictionary(IReadOnlyDictionary<string, string> original)
        {
            Dictionary<string, string> copy = new Dictionary<string, string>();

            foreach(KeyValuePair<string, string> pair in original)
            {
                copy.Add(pair.Key, pair.Value);
            }

            return copy;
        }

        /// <summary>
        /// Sets property/value pair of context; all values in context are sent in telemetry events
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void AddOrUpdateContextProperty(string propertyName, string propertyValue)
        {
            _contextProperties[propertyName] = propertyValue;
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public void ReportException(Exception e)
        {
            ReportedExceptionData exceptionData = new ReportedExceptionData
            {
                ContextProperties = _contextProperties,
                Exception = e,
            };

            _fileWrapper.LogThisData("Exception was reported",
                JsonConvert.SerializeObject(exceptionData, Formatting.Indented));
        }
    }
}
