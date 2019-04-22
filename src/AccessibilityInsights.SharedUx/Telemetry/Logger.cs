// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Use this class to send telemetry from anywhere within
    /// the Accessibility Insights code. This does not include extensions,
    /// which should use the EventTelemetrySink mechanism.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        public static bool IsEnabled => TelemetrySink.IsEnabled;


        /// <summary>
        /// Publishes an event to the current telemetry pipeline
        /// </summary>
        /// <param name="ev">the event being recorded</param>
        public static void PublishTelemetryEvent(TelemetryEvent ev)
        {
            PublishTelemetryEvent(ev.Action, ev.Properties);
        }

        /// <summary>
        /// Publishes an event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void PublishTelemetryEvent(TelemetryAction action, TelemetryProperty property, string value)
        {
            // Conversions to strings are expensive, so skip it if possible
            if (!IsEnabled) return;

            TelemetrySink.PublishTelemetryEvent(action.ToString(), property.ToString(), value);
        }

        /// <summary>
        /// Publishes an event to the current telemetry pipeline
        /// </summary>
        /// <param name="action">The action being recorded</param>
        /// <param name="propertyBag">Associated property bag--this may be null</param>
        public static void PublishTelemetryEvent(TelemetryAction action, IReadOnlyDictionary<TelemetryProperty, string> propertyBag = null)
        {
            // Conversions to strings are expensive, so skip it if possible
            if (!IsEnabled) return;

            TelemetrySink.PublishTelemetryEvent(action.ToString(), ConvertFromProperties(propertyBag));
        }

        /// <summary>
        /// Explicitly updates context properties to be appended to future calls to the current telemetry pipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void AddOrUpdateContextProperty(TelemetryProperty property, string value)
        {
            if (!IsEnabled) return;

            TelemetrySink.AddOrUpdateContextProperty(property.ToString(), value);
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public static void ReportException(this Exception e)
        {
            if (!IsEnabled) return;
            if (e == null) return;

            TelemetrySink.ReportException(e);
        }

        internal static IReadOnlyDictionary<string, string> ConvertFromProperties(IReadOnlyDictionary<TelemetryProperty, string> properties)
        {
            if (properties == null || !properties.Any())
                return null;

            return properties.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value);
        }
    } // class
} // namespace
