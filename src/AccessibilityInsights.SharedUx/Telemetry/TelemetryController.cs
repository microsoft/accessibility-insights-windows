// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    public static class TelemetryController
    {
        private static readonly ITelemetrySink Sink = TelemetrySink.DefaultTelemetrySink;

        public static void OptIntoTelemetry()
        {
            if (!DoesGroupPolicyAllowTelemetry)
                return;

            // Open the telemetry sink
            Sink.HasUserOptedIntoTelemetry = true;

            // Begin listening for telemetry events
            // This must be done after the low-level sink is opened above
            // So that queued events get flushed to an open telemetry sink
            EventTelemetrySink.Enable();

            AxeWindowsTelemetrySink.Enable();
        }

        public static void OptOutOfTelemetry()
        {
            if (!DoesGroupPolicyAllowTelemetry)
                return;

            // Close the telemetry sink
            Sink.HasUserOptedIntoTelemetry = false;
        }

        public static bool DoesGroupPolicyAllowTelemetry => Sink.DoesGroupPolicyAllowTelemetry;
    } // class
} // namespace
