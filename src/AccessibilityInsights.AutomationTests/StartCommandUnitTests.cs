// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Automation;
using AccessibilityInsights.Automation.Fakes;
using AccessibilityInsights.Desktop.Telemetry;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AccessibilityInsights.AutomationTests
{
    [TestClass]
    public class StartCommandUnitTests
    {
        [TestMethod]
        [Timeout(1000)]
        public void Execute_NewInstanceSucceeds_ReturnsSuccessfulResult()
        {
            using (ShimsContext.Create())
            {
                int callsToNewInstance = 0;
                int callsToDeclareSource = 0;
                bool emptyTeamName = false;

                TelemetryAction? actualAction = null;

                ShimAutomationLogger.DeclareSourceString = (name) =>
                {
                    emptyTeamName = string.IsNullOrEmpty(name);

                    callsToDeclareSource++;
                };
                ShimAutomationLogger.LogActionTelemetryActionIReadOnlyDictionaryOfStringString = (a, d) =>
                {
                    actualAction = a;
                };

                ShimAutomationSession.NewInstanceCommandParametersIDisposable = (_, __) =>
                {
                    callsToNewInstance++;
                    return new ShimAutomationSession();
                };
                StartCommandResult result = StartCommand.Execute(new Dictionary<string, string>(), string.Empty);

                Assert.IsTrue(actualAction.HasValue);
                Assert.AreEqual(TelemetryAction.Automation_Start_Session, actualAction.Value);
                Assert.IsTrue(emptyTeamName);
                Assert.AreEqual(1, callsToNewInstance);
                Assert.AreEqual(1, callsToDeclareSource);
                Assert.AreEqual(true, result.Completed);
                Assert.AreEqual(true, result.Succeeded);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.SummaryMessage));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void Execute_NewInstanceSucceedsWithTeamName_ReturnsSuccessfulResult()
        {
            using (ShimsContext.Create())
            {
                int callsToNewInstance = 0;
                int callsToDeclareSource = 0;
                bool matchedTeamName = false;
                string teamname = "team1";

                TelemetryAction? actualAction = null;

                ShimAutomationLogger.DeclareSourceString = (name) =>
                {
                    if (name == teamname) matchedTeamName = true;
                    callsToDeclareSource++;
                };
                ShimAutomationLogger.LogActionTelemetryActionIReadOnlyDictionaryOfStringString = (a, d) =>
                {
                    actualAction = a;
                };

                ShimAutomationSession.NewInstanceCommandParametersIDisposable = (_, __) =>
                {
                    callsToNewInstance++;
                    return new ShimAutomationSession();
                };
                var parameters = new Dictionary<string, string>() { { CommandConstStrings.TeamName, teamname } };

                StartCommandResult result = StartCommand.Execute(parameters, string.Empty);

                Assert.IsTrue(actualAction.HasValue);
                Assert.AreEqual(TelemetryAction.Automation_Start_Session, actualAction.Value);
                Assert.IsTrue(matchedTeamName);
                Assert.AreEqual(1, callsToNewInstance);
                Assert.AreEqual(1, callsToDeclareSource);
                Assert.AreEqual(true, result.Completed);
                Assert.AreEqual(true, result.Succeeded);
                Assert.IsFalse(string.IsNullOrWhiteSpace(result.SummaryMessage));
            }
        }

        [TestMethod]
        [Timeout (1000)]
        public void Execute_NewInstanceThrowsAutomationException_ReturnsFailedResult()
        {
            const string exceptionMessage = "Hello from your local exception!";

            using (ShimsContext.Create())
            {
                int callsToNewInstance = 0;

                ShimAutomationLogger.DeclareSourceString = (_) => {};
                ShimAutomationLogger.LogActionTelemetryActionIReadOnlyDictionaryOfStringString = (_, __) => {};

                ShimAutomationSession.NewInstanceCommandParametersIDisposable = (_, __) =>
                {
                    callsToNewInstance++;
                    throw new A11yAutomationException(exceptionMessage);
                };

                StartCommandResult result = StartCommand.Execute(new Dictionary<string, string>(), string.Empty);

                Assert.AreEqual(1, callsToNewInstance);
                Assert.AreEqual(false, result.Completed);
                Assert.AreEqual(false, result.Succeeded);
                Assert.AreEqual(exceptionMessage, result.SummaryMessage);
            }
        }
    }
}
