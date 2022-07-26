// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace AccessibilityInsights.Extensions.Telemetry
{
    static internal class TelemetryClientFactory
    {
        static internal TelemetryClient GetClient(TelemetryConfiguration config)
        {
            var tc = new TelemetryClient(config);
            tc.Context.Device.OperatingSystem = OSHelpers.GetVersion();
            tc.Context.Cloud.RoleInstance = "undefined";
            return tc;
        }
    }
}
