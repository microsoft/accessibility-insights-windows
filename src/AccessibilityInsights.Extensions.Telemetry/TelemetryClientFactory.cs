// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace AccessibilityInsights.Extensions.Telemetry
{
    static internal class TelemetryClientFactory
    {
        static internal TelemetryClient GetClient(string connectionString)
        {
#pragma warning disable CA2000 // Dispose objects before losing scope
            var tc = new TelemetryClient(new TelemetryConfiguration
            {
                ConnectionString = connectionString,
            });
#pragma warning restore CA2000 // Dispose objects before losing scope
            tc.Context.Device.OperatingSystem = OSHelpers.GetVersion();
            tc.Context.Cloud.RoleInstance = "undefined";
            return tc;
        }
    }
}
