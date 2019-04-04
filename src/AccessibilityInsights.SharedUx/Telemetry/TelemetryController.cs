// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    public static class TelemetryController
    {
        public static void EnableTelemetry()
        {
            // Open the telemetry sink
            TelemetrySink.IsTelemetryAllowed = true;

            // Begin listening for telemetry events
            // This must be done after the low-level sink is opened above
            // So that queued events get flushed to an open telemetry sink
            EventTelemetrySink.Enable();

            AxeWindowsTelemetrySink.Enable();
        }

        public static void DisableTelemetry()
        {
            // Close the telemetry sink
            TelemetrySink.IsTelemetryAllowed = false;
        }
    } // class
} // namespace
