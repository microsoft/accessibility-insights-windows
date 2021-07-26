// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SharedUx.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUXTests.Telemetry
{
    [TestClass]
    public class TelemetryBufferUnitTests
    {
        private TelemetryBuffer _testSubject;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _testSubject = new TelemetryBuffer();
        }

        [TestMethod]
        [Timeout(2000)]
        public void AddEventFactory_FactoryIsNull_ThrowsArgumentNullException()
        {
            ArgumentNullException e = Assert.ThrowsException<ArgumentNullException>(() => _testSubject.AddEventFactory(null));
            Assert.AreEqual("eventFactory", e.ParamName);
        }

        [TestMethod]
        [Timeout(2000)]
        public void AddEventFactory_FactoryIsNotNull_SavesFactoryWithoutInvokingIt()
        {
            _testSubject.AddEventFactory(() => { Assert.Fail("Factory should not be invoked"); return null; });
        }

        [TestMethod]
        [Timeout(2000)]
        public void ProcessEventFactories_ProcessorIsNull_ThrowsArgumentNullException()
        {
            ArgumentNullException e = Assert.ThrowsException<ArgumentNullException>(() => _testSubject.ProcessEventFactories(null));
            Assert.AreEqual("processor", e.ParamName);
        }

        [TestMethod]
        [Timeout(2000)]
        public void ProcessEventFactories_ProcessorIsNotNull_NoEvents_ProcessorIsNotInvoked()
        {
            _testSubject.ProcessEventFactories((_) => Assert.Fail("Processor should not be invoked"));
        }

        [TestMethod]
        [Timeout(2000)]
        public void ProcessEventFactories_ProcessorIsNotNull_HasEvents_ProcessorIsInvokedInOrder()
        {
            const TelemetryAction action1 = TelemetryAction.Event_Load;
            const TelemetryAction action2 = TelemetryAction.Event_Save;
            List<TelemetryEvent> receivedTelemetryEvents = new List<TelemetryEvent>();

            _testSubject.AddEventFactory(() => new TelemetryEvent(action1, new Dictionary<TelemetryProperty, string>()));
            _testSubject.AddEventFactory(() => new TelemetryEvent(action2, new Dictionary<TelemetryProperty, string>()));

            _testSubject.ProcessEventFactories((telemetryEvent) => receivedTelemetryEvents.Add(telemetryEvent));

            Assert.AreEqual(2, receivedTelemetryEvents.Count);
            Assert.AreEqual(action1, receivedTelemetryEvents[0].Action);
            Assert.AreEqual(action2, receivedTelemetryEvents[1].Action);
        }
    }
}
