// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace AccessibilityInsights.ExtensionsTests.DummyClasses
{
    /// <summary>
    ///  Dummy Telemetry implementation for container. No need to
    ///  implement the methods other than the boilerplate
    /// </summary>
    [Export(typeof(ITelemetry))]
    public class DummyTelemetry : ITelemetry
    {
        public void AddOrUpdateContextProperty(string propertyName, string propertyValue)
        {
            throw new NotImplementedException();
        }

        public void FlushAndShutDown()
        {
            throw new NotImplementedException();
        }

        public void PublishEvent(string eventName, IReadOnlyDictionary<string, string> properties)
        {
            throw new NotImplementedException();
        }

        public void ReportException(Exception e)
        {
            throw new NotImplementedException();
        }
    }
}
