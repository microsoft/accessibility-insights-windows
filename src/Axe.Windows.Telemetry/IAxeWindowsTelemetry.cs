// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Axe.Windows.Telemetry
{
    public interface IAxeWindowsTelemetry
    {
        /// <summary>
        /// Publish a single event to the telemetry stream.
        /// </summary>
        /// <param name="eventName">The name of the event. Will be ignored if trivial</param>
        /// <param name="properties">The properties to include with the event. Will be ignored if trivial</param>
        void PublishEvent(string eventName, IReadOnlyDictionary<string, string> properties);

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        void ReportException(Exception e);
    } // interface
} // namespace
