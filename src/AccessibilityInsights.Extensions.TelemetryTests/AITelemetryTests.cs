// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Telemetry;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AccessibilityInsights.Extensions.TelemetryTests
{
    [TestClass]
    public class AITelemetryTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void Ctor_WrapperIsNull_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AITelemetry(null));
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_WrapperIsNotNull_DoesNotThrow()
        {
            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);
            new AITelemetry(wrapperMock.Object);
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishEvent_NonContextPropertiesAreTransient()
        {
            List<EventTelemetry> testEvents = new List<EventTelemetry>();

            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);
            wrapperMock.Setup(x => x.TrackEvent(It.IsAny<EventTelemetry>()))
                .Callback<EventTelemetry>((e) => testEvents.Add(e));

            var aiTelemetry = new AITelemetry(wrapperMock.Object);

            const string action1 = "Action 1";
            const string action2 = "Action 2";
            const string property = "Transient Property";
            aiTelemetry.PublishEvent(action1, new Dictionary<string, string>
            {
                { property, "SomeValue" }
            });
            aiTelemetry.PublishEvent(action2);

            var eventWithProperty = testEvents[0];
            Assert.IsTrue(eventWithProperty.Properties.ContainsKey(property), "The event property must exist on the first event");

            var eventWithoutProperty = testEvents[1];
            Assert.IsFalse(eventWithoutProperty.Properties.ContainsKey(property), "The event property from the first event must not exist on the second event");

            wrapperMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void PublishEvent_ContextPropertiesArePersistent()
        {
            List<EventTelemetry> testEvents = new List<EventTelemetry>();

            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);
            wrapperMock.Setup(x => x.TrackEvent(It.IsAny<EventTelemetry>()))
                .Callback<EventTelemetry>((e) => testEvents.Add(e));

            var aiTelemetry = new AITelemetry(wrapperMock.Object);

            const string action1 = "Action 1";
            const string action2 = "Action 2";
            const string property = "Context Property";
            const string value = "First";
            aiTelemetry.AddOrUpdateContextProperty(property, value);
            aiTelemetry.PublishEvent(action1);
            aiTelemetry.PublishEvent(action2);

            for (int loop = 0; loop < 2; loop++)
            {
                var eventWithContextProperty = testEvents.Find(e => e.Properties.ContainsKey(property));
                Assert.IsNotNull(eventWithContextProperty);
                Assert.IsTrue(eventWithContextProperty.Properties[property].Equals(value));
            }

            wrapperMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_ExceptionIsNull_DoesNotCallWrapper()
        {
            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);

            var aiTelemetry = new AITelemetry(wrapperMock.Object);

            aiTelemetry.ReportException(null);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_ExceptionIsNotNull_CallsWrapper()
        {
            Exception expectedException = new InvalidTimeZoneException("blah");
            Dictionary<string, string> expectedContextProperties = new Dictionary<string, string>
            {
                {"key 1", "value 1" },
                {"key 2", "value 2" },
            };

            Dictionary<string, string> actualContextProperties = null;

            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);
            wrapperMock.Setup(x => x.TrackException(expectedException, It.IsAny<Dictionary<string, string>>()))
                .Callback<Exception, Dictionary<string, string>>((e, d) => actualContextProperties = d);

            var aiTelemetry = new AITelemetry(wrapperMock.Object);
            foreach (KeyValuePair<string, string> pair in expectedContextProperties)
            {
                aiTelemetry.AddOrUpdateContextProperty(pair.Key, pair.Value);
            }

            aiTelemetry.ReportException(expectedException);

            Assert.IsNotNull(actualContextProperties);
            Assert.IsTrue(expectedContextProperties.SequenceEqual(actualContextProperties));

            wrapperMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void ReportException_ThrowsInWrapper_EatsException()
        {
            Exception expectedException = new InvalidTimeZoneException("blah");

            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);
            wrapperMock.Setup(x => x.TrackException(expectedException, It.IsAny<Dictionary<string, string>>()))
                .Throws(new OutOfMemoryException());

            var aiTelemetry = new AITelemetry(wrapperMock.Object);

            aiTelemetry.ReportException(expectedException);

            wrapperMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FlushAndShutDown_CallsWrapper()
        {
            Mock<ITelemetryClientWrapper> wrapperMock = new Mock<ITelemetryClientWrapper>(MockBehavior.Strict);
            wrapperMock.Setup(x => x.FlushAndShutDown());

            var aiTelemetry = new AITelemetry(wrapperMock.Object);

            aiTelemetry.FlushAndShutDown();

            wrapperMock.VerifyAll();
        }
    }
}
