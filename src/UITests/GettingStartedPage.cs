// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UITests
{
    [TestClass]
    public class GettingStartedPage : AIWinSession
    {
        [TestMethod]
        [TestCategory("NoStrongName")]
        public void VerifyGettingStartedTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - ", driver.Title);
        }

        [TestMethod]
        [TestCategory("NoStrongName")]
        public void VerifyAccessibility()
        {
            var result = driver.ScanAIWin();
            Assert.AreEqual(0, result.errors);
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

            // Close telemetry dialog if open
            driver.GettingStarted.DismissTelemetry();
        }

        [ClassCleanup]
        public static void ClassCleanup() => TearDown();
    }
}