// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    internal interface ITelemetrySink
    {
        /// <summary>
        /// We allow group policy to disable telemetry. This takes precedence over user opt-in
        /// </summary>
        bool DoesGroupPolicyAllowTelemetry { get; }

        /// <summary>
        /// Whether or not telemetry toggle button is enabled in the settings.
        /// </summary>
        bool HasUserOptedIntoTelemetry { get; set; }

        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Publishes event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="eventName">The event being recorded</param>
        /// <param name="property">Name of the property</param>
        /// <param name="value">Value associated with the property</param>
        void PublishTelemetryEvent(string eventName, string property, string value);

        /// <summary>
        /// Publishes event to the current telemetry pipeline
        /// </summary>
        /// <param name="eventName">The event being recorded</param>
        /// <param name="propertyBag">Associated property bag--this may be null</param>
        void PublishTelemetryEvent(string eventName, IReadOnlyDictionary<string, string> propertyBag = null);

        /// <summary>
        /// Explicitly updates context properties to be appended to future calls to the current telemetry pipeline
        /// </summary>
        /// <param name="property">The name of the context property</param>
        /// <param name="value">The value of the context property</param>
        void AddOrUpdateContextProperty(string property, string value);

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        void ReportException(Exception e);
    }
}
