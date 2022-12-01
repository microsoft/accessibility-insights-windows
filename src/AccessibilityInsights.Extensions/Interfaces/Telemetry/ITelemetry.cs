// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.Interfaces.Telemetry
{
    /// <summary>
    /// Abstraction for telemetry extension
    /// </summary>
    public interface ITelemetry
    {
        /// <summary>
        /// Add or update a context property that will be included with every event going forward.
        /// If the same property exists in both the context and the specific call to PublishEvent,
        /// the property used in PublishEvent will be used
        /// </summary>
        /// <param name="propertyName">The name of the property. Will be ignored if trivial</param>
        /// <param name="propertyValue">The value for the property. Will be ignored if trivial</param>
        void AddOrUpdateContextProperty(string propertyName, string propertyValue);

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

        /// <summary>
        /// Application is shutting down, so flush any pending telemetry
        /// </summary>
        void FlushAndShutDown();
    }
}
