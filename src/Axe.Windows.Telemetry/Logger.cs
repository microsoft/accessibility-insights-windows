// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Wraps telemetry logging and sends it to 
    /// a supplied IAxeWindowsTelemetry implementation
    /// </summary>
    public static class Logger
    {
        private static IAxeWindowsTelemetry Telemetry = null;
        private static readonly Object LockObject = new Object();

        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        public static bool IsEnabled => Telemetry != null && Telemetry.IsEnabled;

        /// <summary>
        /// Sets the telemetry sink for Axe.Windows.
        /// <param name="telemetry">Telemetry from Axe.Windows will be forwarded to this object</param>
        public static void SetTelemetrySink(IAxeWindowsTelemetry telemetry)
        {
            lock (LockObject)
            {
                Telemetry = telemetry;
            }
        }

        /// <summary>
        /// Publishes event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void PublishTelemetryEvent(TelemetryAction action, TelemetryProperty property, string value)
        {
            // Check IsEnabled because ToString on enums is expensive
            if (!IsEnabled) return;

            PublishTelemetryEvent(action, new Dictionary<TelemetryProperty, string>
                    {
                        { property, value }
                    });
        }

        /// <summary>
        /// Publishes event to the current telemetry pipeline
        /// </summary>
        /// <param name="action">The action being recorded</param>
        /// <param name="propertyBag">Associated property bag--this may be null</param>
        public static void PublishTelemetryEvent(TelemetryAction action, IReadOnlyDictionary<TelemetryProperty, string> propertyBag = null)
        {
            // Check IsEnabled because ToString on enums is expensive
            if (!IsEnabled) return;

            lock (LockObject)
            {
                if (!IsEnabled) return;

                try
                {
                    Telemetry?.PublishEvent(action.ToString(), ConvertFromProperties(propertyBag));
                }
#pragma warning disable CA1031
                catch { }
#pragma warning restore CA1031
            } // lock
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public static void ReportException(this Exception e)
        {
            if (e == null) return;

            lock (LockObject)
            {
                try
                {
                    Telemetry?.ReportException(e);
                }
#pragma warning disable CA1031
                catch { }
#pragma warning restore CA1031
            } // lock
        }

        internal static IReadOnlyDictionary<string, string> ConvertFromProperties(IReadOnlyDictionary<TelemetryProperty, string> properties)
        {
            if (properties == null || !properties.Any())
                return null;

            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (KeyValuePair<TelemetryProperty, string> pair in properties)
            {
                output.Add(pair.Key.ToString(), pair.Value);
            }

            return output;
        }
    } // class
} // namespace
