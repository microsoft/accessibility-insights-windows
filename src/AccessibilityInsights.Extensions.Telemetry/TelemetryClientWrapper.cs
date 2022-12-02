// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.Telemetry
{
    internal class TelemetryClientWrapper : ITelemetryClientWrapper
    {
        private readonly TelemetryClient _client;

        internal TelemetryClientWrapper(TelemetryClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        async void ITelemetryClientWrapper.TrackEvent(EventTelemetry data)
        {
            await Task.Run(new Action(() => _client.TrackEvent(data))).ConfigureAwait(false);
        }

        async void ITelemetryClientWrapper.TrackException(Exception e, Dictionary<string, string> contextProperties)
        {
            await Task.Run(new Action(() => _client.TrackException(e, contextProperties))).ConfigureAwait(false);
        }

        public void FlushAndShutDown()
        {
            _client.Flush();
        }
    }
}
