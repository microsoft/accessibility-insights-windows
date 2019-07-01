// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UITests
{
    /// <summary>
    /// Start the app, open an A11yTest file, navigate to 2 views (automated checks and UIA hierarchy), spot check loaded data, exit app
    /// </summary>
    [TestClass]
    public class LoadTestFile : AIWinSession
    {
        [TestMethod]
        [TestCategory("NoStrongName")]
        public void VerifyResultsInUIATreePage()
        {
            driver.TestMode.AutomatedChecks.ViewInUIATree();
            // do appropriate testing
            driver.TestMode.UIATree.BackToAutomatedChecks();
        }

        [TestMethod]
        [TestCategory("NoStrongName")]
        public void VerifyGridContents()
        {
            driver.TestMode.AutomatedChecks.ToggleAllExpanders();
            // do appropriate testing
            driver.TestMode.AutomatedChecks.ToggleAllExpanders();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Setup(context);

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissPage();
            driver.LiveMode.OpenFile("folder", "file");
        }

        [ClassCleanup]
        public static void ClassCleanup() => TearDown();
    }
}