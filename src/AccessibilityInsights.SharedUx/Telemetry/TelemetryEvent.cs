// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Container for a telemetry action and its properties
    /// </summary>
    public class TelemetryEvent
    {
        public TelemetryAction Action { get; }

        public IReadOnlyDictionary<TelemetryProperty, string> Properties { get; }

        public TelemetryEvent(TelemetryAction action, IReadOnlyDictionary<TelemetryProperty, string> properties)
        {
            this.Action = action;
            this.Properties = properties;
        }
    }
}
