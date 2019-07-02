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

        [TestMethod]
        [TestCategory("NoStrongName")]
        public void LoadTestFileTests()
        {
            ScanResultsInUIATreePage();
            ScanAutomatedChecks();
        }

        private void ScanResultsInUIATreePage()
        {
            Assert.IsTrue(driver.TestMode.AutomatedChecks.ViewInUIATree());

            var result = driver.ScanAIWin(TestContext.ResultsDirectory);

            Assert.AreEqual(0, result.errors);
            Assert.IsTrue(driver.TestMode.ResultsInUIATree.BackToAutomatedChecks());
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
            Assert.IsTrue(driver.GettingStarted.DismissStartupPage());
            Assert.IsTrue(driver.ToggleHighlighter());
            Assert.IsTrue(driver.LiveMode.OpenFile(TestFilePath, TestFileName));
        }

        [TestCleanup]
        public void ClassCleanup() => TearDown();
    }
}