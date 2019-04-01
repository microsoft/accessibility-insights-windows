// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Used for logging telemetry events
    /// </summary>
    public static class Logger
    {
        private static IAxeWindowsTelemetry _telemetry = null;

        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        public static bool IsEnabled => _telemetry != null && _telemetry.IsEnabled;

        /// <summary>
        /// Publishes event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void PublishTelemetryEvent(TelemetryAction action, TelemetryProperty property, string value)
        {
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
            try
            {
                _telemetry?.PublishEvent(action, propertyBag);
            }
#pragma warning disable CA1031
            catch { }
#pragma warning restore CA1031
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public static void ReportException(this Exception e)
        {
            if (e == null) return;

            _telemetry?.ReportException(e);
        }
    } // class
} // namespace
