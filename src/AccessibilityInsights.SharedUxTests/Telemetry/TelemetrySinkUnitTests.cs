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

        private Mock<ITelemetry> _telemetryMock1;
        private Mock<ITelemetry> _telemetryMock2;
        private TelemetrySink _telemetrySink;

        // This is just a wrapper to improve readability
        private void SetupMultipleTelemetryClasses()
        {
            SetupMocksAndSink(true);
        }

        private void SetupMocksAndSink(bool multipleMockTargets)
        {
            _telemetryMock1 = new Mock<ITelemetry>(MockBehavior.Strict);
            _telemetryMock2 = new Mock<ITelemetry>(MockBehavior.Strict);

            if (multipleMockTargets)
            {
                _telemetrySink = new TelemetrySink(new[] { _telemetryMock1.Object, _telemetryMock2.Object }, true);
            }
            else
            {
                _telemetrySink = new TelemetrySink(new[] { _telemetryMock1.Object }, true);
            }
        }

        [TestInitialize]
        public void BeforeEachTest()
        {
            SetupMocksAndSink(false);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_NoTelemetryClassesExist_ReturnsFalse()
        {
            TelemetrySink sink = new TelemetrySink(new List<ITelemetry>(), true);
            Assert.IsFalse(sink.IsEnabled);
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_TelemetryIsDisabledByGroupPolicy_ReturnsFalse()
        {
            TelemetrySink sink = new TelemetrySink(new[] { _telemetryMock1.Object }, false);
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
            _telemetryMock1.Setup(x => x.PublishEvent(EventName1, It.IsAny<PropertyBag>()))
                .Callback<string, PropertyBag>((_, p) => actualPropertyBag = p);

            _telemetrySink.PublishTelemetryEvent(EventName1, PropertyName1, Value1);

            Assert.AreEqual(1, actualPropertyBag.Count);
            Assert.AreEqual(Value1, actualPropertyBag[PropertyName1]);
            _telemetryMock1.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_TelemetryAllowed_ThrowsOnPublish_ReportsException()
        {
            Exception expectedExpection = new OutOfMemoryException();
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.PublishEvent(EventName1, It.IsAny<PropertyBag>()))
                .Throws(expectedExpection);
            _telemetryMock1.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.PublishTelemetryEvent(EventName1, PropertyName1, Value1);

            _telemetryMock1.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultipleClasses_SingleProperty_TelemetryAllowed_PublishesCorrectEventToAll()
        {
            SetupMultipleTelemetryClasses();

            PropertyBag actualPropertyBag = null;
            PropertyBag actualPropertyBag2 = null;

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.PublishEvent(EventName1, It.IsAny<PropertyBag>()))
                .Callback<string, PropertyBag>((_, p) => actualPropertyBag = p);
            _telemetryMock2.Setup(x => x.PublishEvent(EventName1, It.IsAny<PropertyBag>()))
                .Callback<string, PropertyBag>((_, p) => actualPropertyBag2 = p);

            _telemetrySink.PublishTelemetryEvent(EventName1, PropertyName1, Value1);

            Assert.AreEqual(1, actualPropertyBag.Count);
            Assert.AreEqual(1, actualPropertyBag2.Count);
            Assert.AreEqual(Value1, actualPropertyBag[PropertyName1]);
            Assert.AreEqual(Value1, actualPropertyBag2[PropertyName1]);
            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
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
            _telemetryMock1.Setup(x => x.PublishEvent(EventName2, expectedPropertyBag));
            _telemetrySink.PublishTelemetryEvent(EventName2, expectedPropertyBag);

            _telemetryMock1.VerifyAll();
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
            _telemetryMock1.Setup(x => x.PublishEvent(EventName2, expectedPropertyBag))
                .Throws(expectedExpection);
            _telemetryMock1.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.PublishTelemetryEvent(EventName2, expectedPropertyBag);

            _telemetryMock1.VerifyAll();
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
            _telemetryMock1.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2));

            _telemetrySink.AddOrUpdateContextProperty(PropertyName2, Value2);

            _telemetryMock1.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_MultipleClasses_TelemetryIsAllowed_ChainsSameDataToAll()
        {
            SetupMultipleTelemetryClasses();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2));
            _telemetryMock2.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2));

            _telemetrySink.AddOrUpdateContextProperty(PropertyName2, Value2);

            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_TelemetryIsAllowed_TelemetryThrowsException_ReportsException()
        {
            Exception expectedExpection = new OutOfMemoryException();
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2))
                .Throws(expectedExpection);
            _telemetryMock1.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.AddOrUpdateContextProperty(PropertyName2, Value2);

            _telemetryMock1.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_MultipleClasses_TelemetryIsAllowed_TelemetryThrowsException_ReportsExceptionToAll()
        {
            SetupMultipleTelemetryClasses();

            Exception expectedExpection = new OutOfMemoryException();
            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2))
                .Throws(expectedExpection);
            _telemetryMock2.Setup(x => x.AddOrUpdateContextProperty(PropertyName2, Value2));

            _telemetryMock1.Setup(x => x.ReportException(expectedExpection));
            _telemetryMock2.Setup(x => x.ReportException(expectedExpection));

            _telemetrySink.AddOrUpdateContextProperty(PropertyName2, Value2);

            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
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
            _telemetryMock1.Setup(x => x.ReportException(expectedException));

            _telemetrySink.ReportException(expectedException);

            _telemetryMock1.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_MultipleClasses_TelemetryIsAllowed_ChainsSameDataToAll()
        {
            SetupMultipleTelemetryClasses();

            Exception expectedException = new OutOfMemoryException();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.ReportException(expectedException));
            _telemetryMock2.Setup(x => x.ReportException(expectedException));

            _telemetrySink.ReportException(expectedException);

            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_TelemetryIsAllowed_TelemetryThrowsException_DoesNotReportSecondException()
        {
            Exception expectedException = new OutOfMemoryException();
            Exception unexpectedException = new InvalidOperationException();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.ReportException(expectedException))
                .Throws(unexpectedException);

            _telemetrySink.ReportException(expectedException);

            _telemetryMock1.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_TelemetryIsAllowed_MultipleClasses_TelemetryThrowsException_DoesNotReportSecondException()
        {
            SetupMultipleTelemetryClasses();

            Exception expectedException = new OutOfMemoryException();
            Exception unexpectedException = new InvalidOperationException();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.ReportException(expectedException))
                .Throws(unexpectedException);
            _telemetryMock2.Setup(x => x.ReportException(expectedException));

            _telemetrySink.ReportException(expectedException);

            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FlushAndShutDown_TelemetryIsNotAllowed_DoesNotChain()
        {
            _telemetrySink.FlushAndShutDown();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FlushAndShutDown_MultipleClasses_ChainsToAll()
        {
            SetupMultipleTelemetryClasses();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.FlushAndShutDown());
            _telemetryMock2.Setup(x => x.FlushAndShutDown());

            _telemetrySink.FlushAndShutDown();

            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FlushAndShutDown_MultipleClasses_FirstMockThrows_SecondMockIsStillCalled()
        {
            SetupMultipleTelemetryClasses();

            _telemetrySink.HasUserOptedIntoTelemetry = true;
            _telemetryMock1.Setup(x => x.FlushAndShutDown())
                .Throws(new OutOfMemoryException());
            _telemetryMock2.Setup(x => x.FlushAndShutDown());

            _telemetrySink.FlushAndShutDown();

            _telemetryMock1.VerifyAll();
            _telemetryMock2.VerifyAll();
        }
    }
}
