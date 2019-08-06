// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests
{
    [TestClass]
    public class GettingStartedPage : AIWinSession
    {
        /// <summary>
        /// The entry point for this test scenario. Every TestMethod  will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void GettingStartedPageTests()
        {
            VerifyGettingStartedTitle();
            driver.VerifyAccessibility(TestContext, "GettingStarted", 0);
        }

        private void VerifyGettingStartedTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - ", driver.Title, "Getting Started title is incorrect");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();
            driver.GettingStarted.DismissTelemetry();
        }

        [TestCleanup]
        public void TestCleanup() => TearDown();
    }
}