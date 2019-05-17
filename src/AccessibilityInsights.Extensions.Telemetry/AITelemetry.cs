// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.Telemetry
{
    /// <summary>
    /// Application Insights-based implementation of ITelemetry
    /// </summary>
    [Export(typeof(ITelemetry))]
    public class AITelemetry : ITelemetry
    {
        /// <summary>
        /// Instrumentation key from Azure portal
        /// </summary>
        const string InstrumentationKey = "0ad67074-7af0-494c-adee-be70a786448a";

        private TelemetryClient TClient;

        /// <summary>
        /// Properties that are sent alongside all telemetry
        /// </summary>
        private Dictionary<string, string> ContextProperties { get; }

        public AITelemetry()
        {
            ContextProperties = new Dictionary<string, string>();

            InitializeTClient();
        }

        /// <summary>
        /// Initialize Telemetry Client with default values
        /// </summary>
        private void InitializeTClient()
        {
            var tc = new TelemetryClient();
            tc.InstrumentationKey = InstrumentationKey;
            tc.Context.Device.OperatingSystem = OSHelpers.GetVersion();
            tc.Context.Cloud.RoleInstance = "undefined";
            this.TClient = tc;
        }

        /// <summary>
        /// Publishes telemetry to AI with the given action and propertybag
        /// Sent telemetry will also include all context values unless they are overwritten
        /// </summary>
        /// <param name="action">Will be used as event name</param>
        /// <param name="propertyBag">if null, doesn't send any additional property values beyond context</param>
        public void PublishEvent(string action, IReadOnlyDictionary<string, string> propertyBag = null)
        {
            var aiEvent = MakeEventTelemetry(action, propertyBag);

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
            if (props == null) return;

            foreach (var pair in props)
            {
                aiEvent.Properties[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Process an event asynchronously
        /// </summary>
        private async void ProcessEvent(EventTelemetry aiEvent)
        {
            await Task.Run(new Action(() => TClient?.TrackEvent(aiEvent))).ConfigureAwait(false);
        }

        /// <summary>
        /// Sets property/value pair of context; all values in context are sent in telemetry events
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void AddOrUpdateContextProperty(string property, string value)
        {
            if (!string.IsNullOrWhiteSpace(property))
            {
                ContextProperties[property] = value;
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
                    TClient.TrackException(e, ContextProperties);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { } // Don't try to report this Exception
#pragma warning restore CA1031 // Do not catch general exception types
            }
        }
    }
}
