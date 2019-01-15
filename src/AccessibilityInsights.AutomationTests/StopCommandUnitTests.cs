// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Automation;
using AccessibilityInsights.Desktop.Telemetry;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Automation.Fakes;

namespace AccessibilityInsights.AutomationTests
{
    [TestClass]
    public class StopCommandUnitTests
    {
        [TestMethod]
        [Timeout (1000)]
        public void Execute_ClearInstanceSucceeds_ReturnsSuccessfulResult()
        {
            using (ShimsContext.Create())
            {
                int callsToClearInstance = 0;

                ShimAutomationSession.ClearInstance = () =>
                {
                    callsToClearInstance++;
                };

                TelemetryAction? actualAction = null;

                ShimAutomationLogger.LogActionTelemetryActionIReadOnlyDictionaryOfStringString = (a, d) =>
                {
                    actualAction = a;
                };

                StopCommandResult result = StopCommand.Execute();

                Assert.IsTrue(actualAction.HasValue);
                Assert.AreEqual(TelemetryAction.Automation_Stop_Session, actualAction.Value);
                Assert.AreEqual(1, callsToClearInstance);
                Assert.AreEqual(true, result.Completed);
                Assert.AreEqual(true, result.Succeeded);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.SummaryMessage));
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Execute_ClearInstanceThrowsAutomationException_ReturnsFailedResult()
        {
            const string exceptionMessage = "Hello from your local exception!";

            using (ShimsContext.Create())
            {
                int callsToClearInstance = 0;

                ShimAutomationSession.ClearInstance = () =>
                {
                    callsToClearInstance++;
                    throw new A11yAutomationException(exceptionMessage);
                };

                ShimAutomationLogger.LogActionTelemetryActionIReadOnlyDictionaryOfStringString = (_, __) => { };

                StopCommandResult result = StopCommand.Execute();

                Assert.AreEqual(1, callsToClearInstance);
                Assert.AreEqual(false, result.Completed);
                Assert.AreEqual(false, result.Succeeded);
                Assert.AreEqual(exceptionMessage, result.SummaryMessage);
            }
        }
    }
}
