// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.Telemetry;
using AccessibilityInsights.SharedUx.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

using PropertyBag = System.Collections.Generic.IReadOnlyDictionary<string, string>;

namespace AccessibilityInsights.SharedUxTests.Telemetry
{
    [TestClass]
    public class TelemetrySinkUnitTests
    {
        const string EventName1 = "event 1";
        const string EventName2 = "event 2";
        const string PropertyName1 = "property 1";
        const string PropertyName2 = "property 2";
        const string Value1 = "value 1";
        const string Value2 = "value 2";

        private Mock<ITelemetry> _telemetryMock;
        private TelemetrySink _telemetrySink;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _telemetryMock = new Mock<ITelemetry>(MockBehavior.Strict);
            _telemetrySink = new TelemetrySink(_telemetryMock.Object, true);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_TelemetryIsNull_ReturnsFalse()
        {
            TelemetrySink sink = new TelemetrySink(null, true);
            Assert.IsFalse(sink.IsEnabled);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_TelemetryIsDisabledByGroupPolicy_ReturnsFalse()
        {
            TelemetrySink sink = new TelemetrySink(_telemetryMock.Object, false);
            Assert.IsFalse(sink.IsEnabled);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsTelemetryAllowed_DefaultToFalse()
        {
            Assert.IsFalse(_telemetrySink.HasUserOptedIntoTelemetry);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_IsTelemetryAllowedIsFalse_ReturnsFalse()
        {
            Assert.IsFalse(_telemetrySink.IsEnabled);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_IsTelemetryAllowedIsTrue_ReturnsTrue()
        {
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            Assert.IsTrue(_telemetrySink.IsEnabled);
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemetryNotAllowed_DoesNotPublish()
        {
            _telemetrySink.PublishTelemetryEvent(EventName1, PropertyName1, Value1);
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemetryAllowed_PublishesCorrectEvent()
        {
            PropertyBag actualPropertyBag = null;

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.PublishEvent(EventName1, It.IsAny<PropertyBag>()))
                .Callback<string, PropertyBag>((_, p) => actualPropertyBag = p);

            _telemetrySink.PublishTelemetryEvent(EventName1, PropertyName1, Value1);

            Assert.AreEqual(1, actualPropertyBag.Count);
            Assert.AreEqual(Value1, actualPropertyBag[PropertyName1]);
            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemetryAllowed_ThrowsOnPublish_ReportsException()
        {
            Exception expectedExpection = new OutOfMemoryException();
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.PublishEvent(EventName1, It.IsAny<PropertyBag>()))
                .Throws(expectedExpection);
            _telemetryMock.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.PublishTelemetryEvent(EventName1, PropertyName1, Value1);

            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_PropertyBag_TelemetryNotAllowed_DoesNotChain()
        {
            PropertyBag expectedPropertyBag = new Dictionary<string, string>
            {
                { PropertyName1, Value1 },
                { PropertyName2, Value2 },
            };

            _telemetrySink.PublishTelemetryEvent(EventName2, expectedPropertyBag);
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_PropertyBag_TelemetryAllowed_ChainsSameData()
        {
            PropertyBag expectedPropertyBag = new Dictionary<string, string>
            {
                { PropertyName1, Value1 },
                { PropertyName2, Value2 },
            };

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.PublishEvent(EventName2, expectedPropertyBag));
            _telemetrySink.PublishTelemetryEvent(EventName2, expectedPropertyBag);

            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_PropertyBag_TelemetryAllowed_ThrowsOnPublish_ReportsException()
        {
            Exception expectedExpection = new OutOfMemoryException();
            PropertyBag expectedPropertyBag = new Dictionary<string, string>
            {
                { PropertyName1, Value2 },
                { PropertyName2, Value1 },
            };

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.PublishEvent(EventName2, expectedPropertyBag))
                .Throws(expectedExpection);
            _telemetryMock.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.PublishTelemetryEvent(EventName2, expectedPropertyBag);

            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryNotAllowed_DoesNotChain()
        {
            _telemetrySink.AddOrUpdateContextProperty(PropertyName1, Value1);
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryIsAllowed_ChainsSameData()
        {
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2));

            _telemetrySink.AddOrUpdateContextProperty(PropertyName2, Value2);

            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryIsAllowed_TelemetryThrowsException_ReportsException()
        {
            Exception expectedExpection = new OutOfMemoryException();
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2))
                .Throws(expectedExpection);
            _telemetryMock.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.AddOrUpdateContextProperty(PropertyName2, Value2);

            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_TelemetryNotAllowed_DoesNotChain()
        {
            _telemetrySink.ReportException(new OutOfMemoryException());
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_TelemetryIsAllowed_ExceptionIsNull_DoesNotChain()
        {
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetrySink.ReportException(null);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_TelemetryIsAllowed_ChainsSameData()
        {
            Exception expectedException = new OutOfMemoryException();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.ReportException(expectedException));

            _telemetrySink.ReportException(expectedException);

            _telemetryMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_TelemetryIsAllowed_TelemetryThrowsException_DoesNotReportSecondException()
        {
            Exception expectedException = new OutOfMemoryException();
            Exception unexpectedException = new InvalidOperationException();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock.Setup(x => x.ReportException(expectedException))
                .Throws(unexpectedException);

            _telemetrySink.ReportException(expectedException);

            _telemetryMock.VerifyAll();
        }
    }
}
