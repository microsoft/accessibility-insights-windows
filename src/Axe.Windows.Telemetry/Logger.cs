// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Axe.Windows.Telemetry
{
    /// <summary>
    /// Used for logging telemetry events
    /// </summary>
    public static class Logger
    {
        // Use this within the class to get the IAxeWindowsTelemetry interface
        private static IAxeWindowsTelemetry Telemetry => null;

        /// <summary>
        /// Whether or not the extension is available
        /// </summary>
        private static bool IsTelemetryAvailable => Telemetry != null;

        // Fields used for ReportException plumbing
        private static bool IsTelemetryAllowedBackingValue = false;
        // private readonly static object LockObject = new object();
        private readonly static ReportExceptionBuffer ReportExceptionBuffer = new ReportExceptionBuffer(ReportException);

        /// <summary>
        /// Whether or not telemetry toggle button is enabled in the settings.
        /// </summary>
        public static bool IsTelemetryAllowed
        {
            get => IsTelemetryAllowedBackingValue;
            set
            {
                IsTelemetryAllowedBackingValue = value;
                ReportExceptionBuffer.EnableForwarding();
            }
        }

        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        public static bool IsEnabled => IsTelemetryAvailable && IsTelemetryAllowed;

        /// <summary>
        /// Publishes event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void PublishTelemetryEvent(TelemetryAction action, TelemetryProperty property, string value)
        {
            if (IsEnabled)
            {
                try
                {
                    Telemetry.PublishEvent(action.ToString(), new Dictionary<string, string>
                    {
                        { property.ToString(), value }
                    });
                }
#pragma warning disable CA1031
                catch { }
#pragma warning restore CA1020
            }
        }

        /// <summary>
        /// Publishes event to the current telemetry pipeline
        /// </summary>
        /// <param name="action">The action being recorded</param>
        /// <param name="propertyBag">Associated property bag--this may be null</param>
        public static void PublishTelemetryEvent(TelemetryAction action, IReadOnlyDictionary<TelemetryProperty, string> propertyBag = null)
        {
            if (IsEnabled)
            {
                try
                {
                    Telemetry.PublishEvent(action.ToString(), ConvertFromProperties(propertyBag));
                }
#pragma warning disable CA1031
                catch { }
#pragma warning restore CA1031
            }
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public static void ReportException(this Exception e)
        {
            if (IsEnabled && e != null)
            {
                Telemetry.ReportException(e);
            }
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
