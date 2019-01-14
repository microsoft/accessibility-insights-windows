// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Automation;
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.Desktop.Telemetry.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.AutomationTests
{
    [TestClass]
    public class AutomationLoggerUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void LogAction_CalledWithAutomationSopSession_NoProperties_ChainsToLoggerWithCorrectParameter()
        {
            using (ShimsContext.Create())
            {
                const TelemetryAction expectedAction = TelemetryAction.Automation_Stop_Session;

                int callsToShim = 0;
                TelemetryAction? actualAction = null;
                TelemetryProperty? actualProperties = null;
                string actualValue = string.Empty;
                ShimLogger.PublishTelemetryEventTelemetryActionTelemetryPropertyString = (action, properties, value) =>
                {
                    callsToShim++;
                    actualAction = action;
                    actualProperties = properties;
                    actualValue = value;
                };
                AutomationLogger.LogAction(expectedAction);
                Assert.AreEqual(1, callsToShim);
                Assert.IsTrue(actualAction.HasValue);
                Assert.AreEqual(expectedAction, actualAction.Value);
                Assert.IsTrue(actualProperties.HasValue);
                Assert.AreEqual(TelemetryProperty.AutomationParametersSpecified, actualProperties.Value);
                Assert.AreEqual(string.Empty, actualValue);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void LogAction_CalledWithAutomationStartSession_HasProperties_ChainsToLoggerWithCorrectParameter()
        {
            using (ShimsContext.Create())
            {
                const TelemetryAction expectedAction = TelemetryAction.Automation_Start_Session;

                Dictionary<string, string> expectedDictionary = new Dictionary<string, string>
                {
                    { "North", "Carolina" },
                    { "West", "Virginia" },
                };

                int callsToShim = 0;
                TelemetryAction? actualAction = null;
                TelemetryProperty? actualProperties = null;
                string actualValue = string.Empty;
                ShimLogger.PublishTelemetryEventTelemetryActionTelemetryPropertyString = (action, properties, value) =>
                {
                    callsToShim++;
                    actualAction = action;
                    actualProperties = properties;
                    actualValue = value;
                };
                AutomationLogger.LogAction(expectedAction, expectedDictionary);
                Assert.AreEqual(1, callsToShim);
                Assert.IsTrue(actualAction.HasValue);
                Assert.AreEqual(expectedAction, actualAction.Value);
                Assert.IsTrue(actualProperties.HasValue);
                Assert.AreEqual(TelemetryProperty.AutomationParametersSpecified, actualProperties.Value);
                Assert.AreEqual("North;West", actualValue);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void LogAction_ExceptionThrownInLogger_EatsException()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.PublishTelemetryEventTelemetryActionTelemetryPropertyString = (_, __, ___) =>
                {
                    throw new InvalidOperationException("Oops");
                };
                AutomationLogger.LogAction(TelemetryAction.Automation_Invoke_Snapshot);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void DeclareSource_CalledWithAutomationStartSession_ChainsToLoggerWithCorrectParameter()
        {
            using (ShimsContext.Create())
            {
                List<Tuple<TelemetryProperty, string>> parameters = new List<Tuple<TelemetryProperty, string>>();
                ShimLogger.AddOrUpdateContextPropertyTelemetryPropertyString = (property, value) =>
                {
                    parameters.Add(new Tuple<TelemetryProperty, string>(property, value));
                };
                AutomationLogger.DeclareSource();
                Assert.AreEqual(2, parameters.Count);
                Assert.AreEqual(TelemetryProperty.SessionType, parameters[0].Item1);
                Assert.AreEqual("Automation", parameters[0].Item2);
                Assert.AreEqual(TelemetryProperty.Version, parameters[1].Item1);
                Assert.AreEqual(Core.Misc.Utility.GetAppVersion(), parameters[1].Item2);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void DeclareSource_CalledWithTeamName()
        {
            using (ShimsContext.Create())
            {
                List<Tuple<TelemetryProperty, string>> parameters = new List<Tuple<TelemetryProperty, string>>();
                ShimLogger.AddOrUpdateContextPropertyTelemetryPropertyString = (property, value) =>
                {
                    parameters.Add(new Tuple<TelemetryProperty, string>(property, value));
                };
                AutomationLogger.DeclareSource("team1");
                Assert.AreEqual(3, parameters.Count);
                Assert.AreEqual(TelemetryProperty.TeamID, parameters[0].Item1);
                Assert.AreEqual("team1", parameters[0].Item2);
                Assert.AreEqual(TelemetryProperty.SessionType, parameters[1].Item1);
                Assert.AreEqual("Automation", parameters[1].Item2);
                Assert.AreEqual(TelemetryProperty.Version, parameters[2].Item1);
                Assert.AreEqual(Core.Misc.Utility.GetAppVersion(), parameters[2].Item2);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void DeclareSource_ExceptionThrownInLogger_EatsException()
        {
            using (ShimsContext.Create())
            {
                ShimLogger.AddOrUpdateContextPropertyTelemetryPropertyString = (_, __) =>
                {
                    throw new InvalidOperationException("Oops");
                };
                AutomationLogger.DeclareSource();
            }
        }
    }
}
