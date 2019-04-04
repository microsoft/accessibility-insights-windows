// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using Axe.Windows.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Acts as a sink for telemetry events from Axe.Windows.
    /// This class is not intended to be instantiated or called by any other class.
    /// </summary>
    class AxeWindowsTelemetrySink : IAxeWindowsTelemetry
    {
        public static void Enable()
        {
            Axe.Windows.Telemetry.Logger.SetTelemetrySink(new AxeWindowsTelemetrySink());
        }

        /// <summary>
        /// Private constructor so the class cannot be instantiated by any other class
        /// </summary>
        private AxeWindowsTelemetrySink()
        { }

        public void PublishEvent(string eventName, IReadOnlyDictionary<string, string> propertyBag)
        {
            TelemetrySink.PublishTelemetryEvent(eventName, propertyBag);
        }

        public void ReportException(Exception e)
        {
            TelemetrySink.ReportException(e);
        }

        public bool IsEnabled => TelemetrySink.IsEnabled;
    } // class
} // namespace
