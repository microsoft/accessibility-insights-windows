// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests
{
    [TestClass]
    public class TelemetryDialog : AIWinSession
    {
        /// <summary>
        /// The entry point for this test scenario. Every TestMethod  will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void TelemetryDialogTests()
        {
            VerifyTelemetryDialog();
        }

        private void VerifyTelemetryDialog()
        {
            driver.VerifyAccessibility(TestContext, "TelemetryDialog", 0);
            driver.GettingStarted.DismissTelemetry();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();
        }

        [TestCleanup]
        public void TestCleanup() => TearDown();
    }
}