// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.Desktop.Telemetry
{
    /// <summary>
    /// Shim to make the telemetry extension loosely coupled
    /// </summary>
    public static class Logger
    {
        // Use this within the class to get the ITelemetry interface
        private static ITelemetry Telemetry => Container.GetDefaultInstance()?.Telemetry;

        /// <summary>
        /// Whether or not the extension is available
        /// </summary>
        private static bool IsTelemetryAvailable => Telemetry != null;

        // Fields used for ReportException plumbing
        private static bool IsTelemetryAllowedBackingValue = false;
        private static bool IsReportExceptionHandlerAttached = false;
        private readonly static object LockObject = new object();
        private readonly static ReportExceptionBuffer ReportExceptionBuffer = new ReportExceptionBuffer(ReportException);

        /// <summary>
        /// Whether or not telemetry toggle button is enabled in the settings.
        /// </summary>
        public static bool IsTelemetryAllowed
        {
            get => IsTelemetryAllowedBackingValue;
            set
            {
                IsTelemetryAllowedBackingValue = value;
                AttachReportExceptionHandler();
                ReportExceptionBuffer.EnableForwarding();
            }
        }

        /// <summary>
        /// Whether or not telemetry is enabled. Exposed to allow callers who do lots of
        /// work to short-circuit their processing when telemetry is disabled
        /// </summary>
        public static bool IsEnabled => IsTelemetryAvailable && IsTelemetryAllowed;

        /// <summary>
        /// Publishes event with single property/value pair to the current telemetry pipeline
        /// </summary>
        /// <param name="action"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void PublishTelemetryEvent(TelemetryAction action, TelemetryProperty property, string value)
        {
            if (IsEnabled)
            {
                try
                {
                    Telemetry.PublishEvent(action.ToString(), new Dictionary<string, string>
                    {
                        { property.ToString(), value }
                    });
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Publishes event to the current telemetry pipeline
        /// </summary>
        /// <param name="action">The action being recorded</param>
        /// <param name="propertyBag">Associated property bag--this may be null</param>
        public static void PublishTelemetryEvent(TelemetryAction action, IReadOnlyDictionary<TelemetryProperty, string> propertyBag = null)
        {
            if (IsEnabled)
            {
                try
                {
                    Telemetry.PublishEvent(action.ToString(), ConvertFromProperties(propertyBag));
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// Explicitly updates context properties to be appended to future calls to the current telemetry pipeline
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void AddOrUpdateContextProperty(TelemetryProperty property, string value)
        {
            if (IsEnabled)
            {
                Telemetry.AddOrUpdateContextProperty(property.ToString(), value);
            }
        }

        /// <summary>
        /// Report an Exception into the pipeline
        /// </summary>
        /// <param name="e">The Exception to report</param>
        public static void ReportException(this Exception e)
        {
            if (IsEnabled && e != null)
            {
                Telemetry.ReportException(e);
            }
        }

        internal static IReadOnlyDictionary<string, string> ConvertFromProperties(IReadOnlyDictionary<TelemetryProperty, string> properties)
        {
            if (properties == null || !properties.Any())
                return null;

            Dictionary<string, string> output = new Dictionary<string, string>();

            foreach (KeyValuePair<TelemetryProperty, string> pair in properties)
            {
                output.Add(pair.Key.ToString(), pair.Value);
            }

            return output;
        }

        /// <summary>
        /// Forward exceptions from the event handler to the logger
        /// </summary>
        private static void OnReportedException(object sender, ReportExceptionEventArgs args)
        {
            ReportExceptionBuffer.ReportException(args.ReportedException);
        }

        /// <summary>
        /// Attach the handler only once, no matter how many times this method gets called
        /// </summary>
        public static void AttachReportExceptionHandler()
        {
            if (!IsReportExceptionHandlerAttached)
            {
                lock (LockObject)
                {
                    if (!IsReportExceptionHandlerAttached)
                    {
                        Container.ReportedExceptionEvent += OnReportedException;
                        IsReportExceptionHandlerAttached = true;
                    }
                }
            }
        }
    }
}
