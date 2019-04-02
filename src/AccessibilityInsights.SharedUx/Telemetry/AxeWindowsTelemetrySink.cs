// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions;
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using Axe.Windows.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AccessibilityInsights.SharedUx.Telemetry
{
    /// <summary>
    /// Acts as a sink for telemetry events from Axe.Windows.
    /// This class is not intended to be instantiated or called by any other class.
    /// </summary>
    class AxeWindowsTelemetrySink : IAxeWindowsTelemetry
    {
#pragma warning disable CA1810
        private static readonly ITelemetry _telemetry;

        static AxeWindowsTelemetrySink()
        {
            _telemetry = Container.GetDefaultInstance()?.Telemetry;
            if (_telemetry == null) return;

            if (!Logger.IsEnabled)
                return;

            Axe.Windows.Telemetry.Logger.SetTelemetrySink(new AxeWindowsTelemetrySink());
        }
#pragma warning restore CA1810

        /// <summary>
        /// Private constructor so the class cannot be instantiated by any other class
        /// </summary>
        private AxeWindowsTelemetrySink()
        { }

        public void PublishEvent(string action, IReadOnlyDictionary<string, string> propertyBag)
        {
            if (!Logger.IsEnabled) return;

            try
            {
                _telemetry?.PublishEvent(action, propertyBag);
            }
            catch (Exception) { }
        }

        public void ReportException(Exception e)
        {
            Logger.ReportException(e);
        }

        public bool IsEnabled => Logger.IsEnabled;
    } // class
} // namespace
