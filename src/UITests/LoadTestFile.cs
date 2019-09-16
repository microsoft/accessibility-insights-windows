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
        const string TestFileName = "WildlifeManagerTest.a11ytest";
        readonly string TestFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFiles");

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void LoadTestFileTests()
        {
            driver.VerifyAccessibility(TestContext, "AutomatedChecks", 0);
            ScanResultsInUIATreePage();
            driver.TestMode.AutomatedChecks.ValidateAutomatedChecks(12);
            driver.TestMode.AutomatedChecks.GoToAutomatedChecksElementDetails(0);
            ValidateResultsInUIATree();
        }

        private void ScanResultsInUIATreePage()
        {
            driver.TestMode.AutomatedChecks.ViewInUIATree();

            driver.VerifyAccessibility(TestContext, "UIATree", 0);
            driver.TestMode.ResultsInUIATree.BackToAutomatedChecks();
        }

        private void ValidateFirstSelectedElement()
        {
            driver.TestMode.ResultsInUIATree.ValidateResults(2, 28);
            driver.TestMode.ResultsInUIATree.SwitchToDetailsTab();
            driver.TestMode.ResultsInUIATree.ValidateDetails("InvokePattern", "Name: Ok", 3, 10);
            driver.TestMode.ResultsInUIATree.ValidateTree("pane 'Desktop 1' has failed test results in descendants.", 16);
        }

        private void ValidateRootElement()
        {
            driver.TestMode.ResultsInUIATree.ValidateDetails("LegacyIAccessiblePattern", "Name: Desktop 1", 10, 10);
            driver.TestMode.ResultsInUIATree.SwitchToResultsTab();
            driver.TestMode.ResultsInUIATree.ValidateResults(0, 0);
        }

        private void ValidateResultsInUIATree()
        {
            ValidateFirstSelectedElement();
            driver.TestMode.ResultsInUIATree.SelectElementInTree(0);
            ValidateRootElement();
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
        public void TestCleanup() => TearDown();
    }
}