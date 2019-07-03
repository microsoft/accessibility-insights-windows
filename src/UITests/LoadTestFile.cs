// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace UITests
{
    /// <summary>
    /// Start the app, open an A11yTest file, navigate to 2 views (automated checks and UIA hierarchy), spot check loaded data, exit app
    /// </summary>
    [TestClass]
    public class LoadTestFile : AIWinSession
    {
        static readonly string TestFileName = "WildlifeManagerTest.a11ytest";
        static readonly string TestFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\TestFiles";

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod  will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory("NoStrongName")]
        public void LoadTestFileTests()
        {
            ScanResultsInUIATreePage();
            ScanAutomatedChecks();
        }

        private void ScanResultsInUIATreePage()
        {
            driver.TestMode.AutomatedChecks.ViewInUIATree();

            var result = driver.ScanAIWin(TestContext.ResultsDirectory);

            Assert.AreEqual(0, result.errors);
            driver.TestMode.ResultsInUIATree.BackToAutomatedChecks();
        }

        private void ScanAutomatedChecks()
        {
            var result = driver.ScanAIWin(TestContext.ResultsDirectory);
            Assert.AreEqual(0, result.errors);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();
            driver.ToggleHighlighter();
            driver.LiveMode.OpenFile(TestFilePath, TestFileName);
        }

        [TestCleanup]
        public void ClassCleanup() => TearDown();
    }
}