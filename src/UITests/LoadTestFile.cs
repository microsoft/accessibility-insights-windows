﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
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
        [TestCategory("NoStrongName")]
        [TestCategory("UITest")]
        public void LoadTestFileTests()
        {
            ScanAutomatedChecks();
            ScanResultsInUIATreePage();
            ValidateAutomatedChecks();
            ValidateResultsInUIATree();
        }

        private void ScanResultsInUIATreePage()
        {
            driver.TestMode.AutomatedChecks.ViewInUIATree();

            var issueCount = driver.ScanAIWin(TestContext, "UIATree");

            Assert.AreEqual(0, issueCount);
            driver.TestMode.ResultsInUIATree.BackToAutomatedChecks();
        }

        private void ScanAutomatedChecks()
        {
            var issueCount = driver.ScanAIWin(TestContext, "AutomatedChecks");
            Assert.AreEqual(0, issueCount);
        }

        private void ValidateFirstSelectedElement()
        {
            driver.TestMode.ResultsInUIATree.ValidateResults(false, 2, 28);
            driver.TestMode.ResultsInUIATree.SwitchToDetailsTab();
            driver.TestMode.ResultsInUIATree.ValidateDetails("InvokePattern", "Name, Ok", 3, 10);
            driver.TestMode.ResultsInUIATree.ValidateTree("pane 'Desktop 1' has failed test results in descendants.", 16);
        }

        private void ValidateRootElement()
        {
            driver.TestMode.ResultsInUIATree.ValidateDetails("LegacyIAccessiblePattern", "Name, Desktop 1", 10, 10);
            driver.TestMode.ResultsInUIATree.SwitchToResultsTab();
            driver.TestMode.ResultsInUIATree.ValidateResults(true, 0, 0);
        }

        private void ValidateResultsInUIATree()
        {
            ValidateFirstSelectedElement();
            driver.TestMode.ResultsInUIATree.SelectElementInTree(0);
            ValidateRootElement();
        }

        private void ValidateAutomatedChecks()
        {
            driver.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksExpandAllButton).Click();
            var resultsGrid = driver.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksResultsListView);
            var results = resultsGrid.FindElementsByClassName("ListViewItem");
            var resultsText = driver.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksResultsTextBlock).Text;
            var resultCount = int.Parse(resultsText.Split()[0]);

            Assert.AreEqual(12, resultCount);
            Assert.AreEqual(resultCount, results.Count);

            results[0].FindElementByClassName("Button").Click();
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