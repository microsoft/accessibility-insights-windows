// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.Telemetry
{
    /// <summary>
    /// Simple interface to wrap TelemetryClient (which is sealed) for unit testing
    /// </summary>
    internal interface ITelemetryClientWrapper
    {
        void TrackEvent(EventTelemetry data);
        void TrackException(Exception e, Dictionary<string, string> contextProperties);
        void FlushAndShutDown();
    }
}
