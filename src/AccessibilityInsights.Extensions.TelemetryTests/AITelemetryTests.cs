// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Telemetry;
using AccessibilityInsights.Extensions.Telemetry.Fakes;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Extensions.TelemetryTests
{
    [TestClass]
    public class AITelemetryTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void NonContextPropertiesResetBetweenEventsTest()
        {
            using (ShimsContext.Create())
            {
                List<EventTelemetry> testEvents = new List<EventTelemetry>();
                ShimAITelemetry.AllInstances.ProcessEventEventTelemetry = (_, eve) => testEvents.Add(eve);
                var aiTelemetry = new AITelemetry();

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
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void ContextPropertiesRemainBetweenEventsTest()
        {
            using (ShimsContext.Create())
            {
                List<EventTelemetry> testEvents = new List<EventTelemetry>();
                ShimAITelemetry.AllInstances.ProcessEventEventTelemetry = (_, eve) => testEvents.Add(eve);
                var aiTelemetry = new AITelemetry();

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
            }
        }
    }
}
