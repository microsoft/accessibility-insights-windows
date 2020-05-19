// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Telemetry;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Acts as a sink for telemetry events from Axe.Windows.
    /// This class is not intended to be instantiated or called by any other class.
    /// </summary>
    class AxeWindowsTelemetrySink : IAxeWindowsTelemetry
    {
        private readonly ITelemetrySink _telemetrySink;

        public static void Enable()
        {
            Axe.Windows.Telemetry.Logger.SetTelemetrySink(new AxeWindowsTelemetrySink(TelemetrySink.DefaultTelemetrySink));
        }

        /// <summary>
        /// Private constructor so the class cannot be instantiated by any other class
        /// </summary>
        private AxeWindowsTelemetrySink(ITelemetrySink telemetrySink)
        {
            _telemetrySink = telemetrySink;
        }

        public void PublishEvent(string eventName, IReadOnlyDictionary<string, string> propertyBag)
        {
            _telemetrySink.PublishTelemetryEvent(eventName, propertyBag);
        }

        public void ReportException(Exception e)
        {
            _telemetrySink.ReportException(e);
        }

        public bool IsEnabled => _telemetrySink.IsEnabled;
    } // class
} // namespace
