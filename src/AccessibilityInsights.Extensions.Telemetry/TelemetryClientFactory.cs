// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace AccessibilityInsights.Extensions.Telemetry
{
    internal static class TelemetryClientFactory
    {
        internal static TelemetryClient GetClient(TelemetryConfiguration config)
        {
            var tc = new TelemetryClient(config);
            tc.Context.Device.OperatingSystem = OSHelpers.GetVersion();
            tc.Context.Cloud.RoleInstance = "undefined";
            return tc;
        }
    }
}
