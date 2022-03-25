// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Class to buffer telemetry events prior to telemetry initialization, then play them back after
    /// initialization if telemetry is enabled.
    /// </summary>
    public class TelemetryBuffer
    {
        private readonly List<Func<TelemetryEvent>> _eventFactoryList = new List<Func<TelemetryEvent>>();

        public void AddEventFactory(Func<TelemetryEvent> eventFactory)
        {
            if (eventFactory == null)
                throw new ArgumentNullException(nameof(eventFactory));

            _eventFactoryList.Add(eventFactory);
        }

        public void ProcessEventFactories(Action<TelemetryEvent> processor)
        {
            if (processor == null)
                throw new ArgumentNullException(nameof(processor));

            foreach (Func<TelemetryEvent> eventFactory in _eventFactoryList)
            {
                processor(eventFactory());
            }
        }
    }
}
