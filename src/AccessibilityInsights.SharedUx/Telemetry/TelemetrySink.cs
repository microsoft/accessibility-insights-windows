// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// This is the lowest sink for telemetry in Accessibility Insights.
    /// All telemetry comes through this class.
    /// It should not be accessed directly, but only through the higher-level telemetry classes
    /// such as Logger and TelemetryController.
    /// This class should not throw any exceptions.
    /// </summary>
    internal static class TelemetrySink
    {
        private static ITelemetry Telemetry => Container.GetDefaultInstance()?.Telemetry;

        /// <summary>
        /// Whether or not telemetry toggle button is enabled in the settings.
        /// </summary>
        public static bool IsTelemetryAllowed { get; internal set; } = false;

        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        public static bool IsEnabled => IsTelemetryAllowed && Telemetry != null;

        /// <summary>
        /// Publishes event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void PublishTelemetryEvent(string eventName, string property, string value)
        {
            PublishTelemetryEvent(eventName, new Dictionary<string, string>
            {
                { property, value }
            });
        }

        /// <summary>
        /// Publishes event to the current telemetry pipeline
        /// </summary>
        /// <param name="eventName">The event being recorded</param>
        /// <param name="propertyBag">Associated property bag--this may be null</param>
        public static void PublishTelemetryEvent(string eventName, IReadOnlyDictionary<string, string> propertyBag = null)
        {
            if (!IsEnabled) return;

            try
            {
                Telemetry.PublishEvent(eventName, propertyBag);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                ReportException(e);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Explicitly updates context properties to be appended to future calls to the current telemetry pipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void AddOrUpdateContextProperty(string property, string value)
        {
            if (!IsEnabled) return;

            try
            {
                Telemetry.AddOrUpdateContextProperty(property, value);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                ReportException(e);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public static void ReportException(Exception e)
        {
            if (!IsEnabled) return;
            if (e == null) return;

            try
            {
                Telemetry.ReportException(e);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception) { }  // Silently eat this exception (nothing we could do about it anyway)
#pragma warning restore CA1031 // Do not catch general exception types
        }
    } // class
} // namespace
