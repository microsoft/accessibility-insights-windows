// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests
{
    [TestClass]
    public class GettingStartedPage : AIWinSession
    {
        [TestMethod]
        [TestCategory("NoStrongName")]
        public void GettingStartedPageTests()
        {
            VerifyGettingStartedTitle();
            VerifyAccessibility();
        }

        private void VerifyGettingStartedTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - ", driver.Title);
        }

        private void VerifyAccessibility()
        {
            var result = driver.ScanAIWin(TestContext.ResultsDirectory);
            Assert.AreEqual(0, result.errors);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            // Close telemetry dialog if open
            driver.GettingStarted.DismissTelemetry();
        }

        [TestCleanup]
        public void TestCleanup() => TearDown();
    }
}