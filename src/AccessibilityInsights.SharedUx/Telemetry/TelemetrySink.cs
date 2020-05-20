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
    internal class TelemetrySink : ITelemetrySink
    {
        /// <summary>
        /// Holds the production TelemetrySink object
        /// </summary>
        internal static ITelemetrySink DefaultTelemetrySink { get; } = new TelemetrySink(Container.GetDefaultInstance()?.Telemetry);

        private readonly ITelemetry _telemetry;

        internal TelemetrySink(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Implements <see cref="ITelemetrySink.IsTelemetryAllowed"/>
        /// </summary>
        public bool IsTelemetryAllowed { get; set; } = false;

        /// <summary>
        /// Implements <see cref="ITelemetrySink.IsEnabled"/>
        /// </summary>
        public bool IsEnabled => IsTelemetryAllowed && _telemetry != null;

        /// <summary>
        /// Implements <see cref="ITelemetrySink.PublishTelemetryEvent(string, string, string)"/>
        /// </summary>
        public void PublishTelemetryEvent(string eventName, string property, string value)
        {
            PublishTelemetryEvent(eventName, new Dictionary<string, string>
            {
                { property, value }
            });
        }

        /// <summary>
        /// Implements <see cref="ITelemetrySink.PublishTelemetryEvent(string, IReadOnlyDictionary{string, string})"/>
        /// </summary>
        public void PublishTelemetryEvent(string eventName, IReadOnlyDictionary<string, string> propertyBag = null)
        {
            if (!IsEnabled) return;

            try
            {
                _telemetry.PublishEvent(eventName, propertyBag);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                ReportException(e);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Implements <see cref="ITelemetrySink.AddOrUpdateContextProperty(string, string)"/>
        /// </summary>
        public void AddOrUpdateContextProperty(string property, string value)
        {
            if (!IsEnabled) return;

            try
            {
                _telemetry.AddOrUpdateContextProperty(property, value);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                ReportException(e);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Implements <see cref="ITelemetrySink.ReportException(Exception)"/>
        /// </summary>
        public void ReportException(Exception e)
        {
            if (!IsEnabled) return;
            if (e == null) return;

            try
            {
                _telemetry.ReportException(e);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception) { }  // Silently eat this exception (nothing we could do about it anyway)
#pragma warning restore CA1031 // Do not catch general exception types
        }
    } // class
} // namespace
