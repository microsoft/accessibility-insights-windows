// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
#if FAKES_SUPPORTED
using Axe.Windows.Automation.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
#endif

namespace Axe.Windows.AutomationTests
{
    [TestClass]
    public class StartCommandUnitTests
    {
#if FAKES_SUPPORTED
        [TestMethod]
        [Timeout(1000)]
        public void Execute_NewInstanceSucceeds_ReturnsSuccessfulResult()
        {
            using (ShimsContext.Create())
            {
                int callsToNewInstance = 0;

                ShimAutomationSession.NewInstanceCommandParametersIDisposable = (_, __) =>
                {
                    callsToNewInstance++;
                    return new ShimAutomationSession();
                };
                StartCommandResult result = StartCommand.Execute(new Dictionary<string, string>(), string.Empty);

                Assert.AreEqual(1, callsToNewInstance);
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
#endif
    }
}
