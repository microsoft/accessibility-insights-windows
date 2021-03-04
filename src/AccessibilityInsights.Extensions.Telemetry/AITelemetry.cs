// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace AccessibilityInsights.Extensions.Telemetry
{
    /// <summary>
    /// Application Insights-based implementation of ITelemetry
    /// </summary>
    [Export(typeof(ITelemetry))]
    public class AITelemetry : ITelemetry
    {
        private readonly ITelemetryClientWrapper _clientWrapper;
        /// <summary>
        /// Instrumentation key from Azure portal
        /// </summary>
        const string InstrumentationKey = "0ad67074-7af0-494c-adee-be70a786448a";

        /// <summary>
        /// Properties that are sent alongside all telemetry
        /// </summary>
        private Dictionary<string, string> ContextProperties { get; }

#pragma warning disable RS0034 // Exported parts should have [ImportingConstructor]
        /// <summary>
        /// Production ctor--must be public for MEF
        /// </summary>
        public AITelemetry()
            :this(new TelemetryClientWrapper(TelemetryClientFactory.GetClient(InstrumentationKey)))
        {
        }

        /// <summary>
        /// Unit test ctor
        /// </summary>
        /// <param name="clientWrapper">The wrapper of the TelemetryClient</param>
        internal AITelemetry(ITelemetryClientWrapper clientWrapper)
        {
            if (clientWrapper == null)
                throw new ArgumentNullException(nameof(clientWrapper));

            _clientWrapper = clientWrapper;
            ContextProperties = new Dictionary<string, string>();
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
            var aiEvent = MakeEventTelemetry(eventName, properties);

            ProcessEvent(aiEvent);
        }

        private EventTelemetry MakeEventTelemetry(string action, IReadOnlyDictionary<string, string> propertyBag = null)
        {
            var aiEvent = new EventTelemetry(action);

            MergeIntoProperties(aiEvent, ContextProperties);
            MergeIntoProperties(aiEvent, propertyBag);

            return aiEvent;
        }

        private static void MergeIntoProperties(EventTelemetry aiEvent, IReadOnlyDictionary<string, string> props)
        {
            if (props == null)
                return;

            foreach (var pair in props)
            {
                aiEvent.Properties[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Process an event
        /// </summary>
        private void ProcessEvent(EventTelemetry aiEvent)
        {
            _clientWrapper.TrackEvent(aiEvent);
        }

        /// <summary>
        /// Sets property/value pair of context; all values in context are sent in telemetry events
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        public void AddOrUpdateContextProperty(string propertyName, string propertyValue)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
            {
                ContextProperties[propertyName] = propertyValue;
            }
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public void ReportException(Exception e)
        {
            if (e != null)
            {
                try
                {
                    _clientWrapper.TrackException(e, ContextProperties);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { } // Don't try to report this Exception
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}
