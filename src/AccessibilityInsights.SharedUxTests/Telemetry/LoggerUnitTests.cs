// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

using TelemetryPropertyBag = System.Collections.Generic.IReadOnlyDictionary<AccessibilityInsights.SharedUx.Telemetry.TelemetryProperty, string>;
using StringPropertyBag = System.Collections.Generic.IReadOnlyDictionary<string, string>;

namespace AccessibilityInsights.SharedUXTests.Telemetry
{
    [TestClass]
    public class LoggerUnitTests
    {
        const TelemetryAction Action1 = (TelemetryAction)1;
        const TelemetryAction Action2 = (TelemetryAction)2;
        const TelemetryProperty Property1 = (TelemetryProperty)1;
        const TelemetryProperty Property2 = (TelemetryProperty)2;
        const TelemetryProperty Property3 = (TelemetryProperty)3;
        const string Value1 = "January";
        const string Value2 = "February";
        const string Value3 = "March";

        private Mock<ITelemetrySink> _sinkMock;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _sinkMock = new Mock<ITelemetrySink>(MockBehavior.Strict);
            Logger.SetTelemetrySink(_sinkMock.Object);
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            Logger.SetTelemetrySink(null);  // Reset to default sink
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsEnabled_SinkIsNotEnabled_ReturnsFalse()
        {
            _sinkMock.Setup(x => x.IsEnabled).Returns(false);

            Assert.IsFalse(Logger.IsEnabled);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void IsEnabled_SinkIsEnabled_ReturnsTrue()
        {
            _sinkMock.Setup(x => x.IsEnabled).Returns(true);

            Assert.IsTrue(Logger.IsEnabled);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ConvertFromProperties_InputIsNull_OutputIsNull()
        {
            Assert.IsNull(Logger.ConvertFromProperties(null));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ConvertFromProperties_InputIsTrivial_OutputIsNull()
        {
            Assert.IsNull(Logger.ConvertFromProperties(new Dictionary<TelemetryProperty, string>()));
        }

        [TestMethod]
        [Timeout(1000)]
        public void ConvertFromProperties_InputIsNontrivial_OutputIsCorrect()
        {
            // Specific values of TelemetryProperty are unimportant for this test
            TelemetryPropertyBag expectedProperties = new Dictionary<TelemetryProperty, string>
            {
                { Property1, Value1 },
                { Property2, Value2 },
                { Property3, Value3 },
            };

            StringPropertyBag convertedProperties = Logger.ConvertFromProperties(expectedProperties);

            ValidatePropertyBag(expectedProperties, convertedProperties);
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_EventObject_EventIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => Logger.PublishTelemetryEvent(null));
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_EventObject_EventIsNotNull_SinkIsNotEnabled_DoesNotChainToSink()
        {
            TelemetryEvent expectedEvent = new TelemetryEvent(Action1, new Dictionary<TelemetryProperty, string>
            {
                { Property2, Value3 },
            });

            _sinkMock.Setup(x => x.IsEnabled).Returns(false);

            Logger.PublishTelemetryEvent(expectedEvent);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultiProperty_SinkIsNotEnabled_DoesNotChainToSink()
        {
            TelemetryPropertyBag expectedProperties = new Dictionary<TelemetryProperty, string>
            {
                { Property2, Value3 },
                { Property3, Value1 },
            };

            _sinkMock.Setup(x => x.IsEnabled).Returns(false);

            Logger.PublishTelemetryEvent(Action2, expectedProperties);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_MultiProperty_SinkIsEnabled_ChainsToSink()
        {
            StringPropertyBag actualProperties = null;
            TelemetryPropertyBag expectedProperties = new Dictionary<TelemetryProperty, string>
            {
                { Property2, Value3 },
                { Property3, Value1 },
            };

            _sinkMock.Setup(x => x.IsEnabled).Returns(true);
            _sinkMock.Setup(x => x.PublishTelemetryEvent(Action2.ToString(), It.IsAny<StringPropertyBag>()))
                .Callback<string, StringPropertyBag>((_, p) => actualProperties = p);

            Logger.PublishTelemetryEvent(Action2, expectedProperties);

            ValidatePropertyBag(expectedProperties, actualProperties);
            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_SinkIsNotEnabled_DoesNotChainToSink()
        {
            _sinkMock.Setup(x => x.IsEnabled).Returns(false);

            Logger.PublishTelemetryEvent(Action1, Property3, Value2);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishTelemetryEvent_SingleProperty_SinkIsEnabled_ChainsToSink()
        {
            _sinkMock.Setup(x => x.IsEnabled).Returns(true);
            _sinkMock.Setup(x => x.PublishTelemetryEvent(Action1.ToString(), Property3.ToString(), Value2));

            Logger.PublishTelemetryEvent(Action1, Property3, Value2);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_SinkIsNotEnabled_DoesNotChainToSink()
        {
            _sinkMock.Setup(x => x.IsEnabled).Returns(false);

            Logger.AddOrUpdateContextProperty(Property1, Value1);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void AddOrUpdateContextProperty_SinkIsEnabled_ChainsToSink()
        {
            _sinkMock.Setup(x => x.IsEnabled).Returns(true);
            _sinkMock.Setup(x => x.AddOrUpdateContextProperty(Property1.ToString(), Value1));

            Logger.AddOrUpdateContextProperty(Property1, Value1);

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_ExceptionIsNull_DoesNotChainToSink()
        {
            Logger.ReportException(null);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_SinkIsNotEnabled_DoesNotChainToSink()
        {
            Exception expectedException = new DivideByZeroException();

            _sinkMock.Setup(x => x.IsEnabled).Returns(false);

            expectedException.ReportException();

            _sinkMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_SinkIsEnabled_ChainsToSink()
        {
            Exception expectedException = new OutOfMemoryException();

            _sinkMock.Setup(x => x.IsEnabled).Returns(true);
            _sinkMock.Setup(x => x.ReportException(expectedException));

            expectedException.ReportException();

            _sinkMock.VerifyAll();
        }

        private void ValidatePropertyBag(TelemetryPropertyBag expected, StringPropertyBag actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            foreach (KeyValuePair<TelemetryProperty, string> pair in expected)
            {
                Assert.AreEqual(pair.Value, actual[pair.Key.ToString()]);
            }
        }
    }
}
