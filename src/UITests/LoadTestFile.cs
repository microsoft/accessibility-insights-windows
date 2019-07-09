// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
            ValidateFirstResults();
            ValidateFirstDetails();
            ValidateFirstTree();
        }

        private void ValidateFirstResults()
        {
            var resultsList = driver.FindElementByAccessibilityId(AutomationIDs.ScannerResultsDetailsListView);
            var resultsFailedOnly = resultsList.FindElementsByClassName("ListViewItem");
            driver.FindElementByAccessibilityId(AutomationIDs.ScannerResultsShowAllButton).Click();
            var resultsAll = resultsList.FindElementsByClassName("ListViewItem");
            var fixFollowTb = driver.FindElementByAccessibilityId(AutomationIDs.ScannerResultsFixFollowingTextBox);

            Assert.IsFalse(string.IsNullOrEmpty(fixFollowTb.Text));
            Assert.AreEqual(2, resultsFailedOnly.Count);
            Assert.AreEqual(28, resultsAll.Count);

            resultsAll[0].SendKeys(Keys.Control + Keys.Tab);
        }

        private void ValidateFirstDetails()
        {
            var tree = driver.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            var props = driver.FindElementByAccessibilityId(AutomationIDs.SnapshotModeControl).FindElementsByClassName("DataGridRow");
            props[0].Click();
            var patterns = GetPatternsNodes(AutomationIDs.SnapshotModeControl, nodes);

            Assert.AreEqual("InvokePattern", patterns.First().Text);
            Assert.AreEqual("Name, Ok", props[0].Text);
            Assert.AreEqual(3, patterns.Count());
            Assert.AreEqual(10, props.Count);
        }

        private void ValidateFirstTree()
        {
            var tree = driver.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            nodes.First().SendKeys(Keys.Left);

            Assert.AreEqual(16, nodes.Count);
            Assert.AreEqual("pane 'Desktop 1' has failed test results in descendants.", nodes.First().Text);
        }

        private void ValidateRootElement()
        {
            ValidateRootDetails();
            ValidateRootResults();
        }

        private void ValidateRootDetails()
        {
            var props = driver.FindElementByAccessibilityId(AutomationIDs.SnapshotModeControl).FindElementsByClassName("DataGridRow");
            props[0].Click();
            var text = props[0].Text;
            var tree = driver.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            var patterns = GetPatternsNodes(AutomationIDs.SnapshotModeControl, nodes);

            Assert.AreEqual("Name, Desktop 1", text);
            Assert.AreEqual(10, patterns.Count());
            Assert.AreEqual("LegacyIAccessiblePattern", patterns.First().Text);
            Assert.AreEqual(10, props.Count);

            patterns.Last().SendKeys(Keys.Control + Keys.Tab);
        }

        private void ValidateRootResults()
        {
            var resultsList = driver.FindElementByAccessibilityId(AutomationIDs.ScannerResultsDetailsListView);
            var results = resultsList.FindElementsByClassName("ListViewItem");
            var fixFollowTb = driver.FindElementByAccessibilityId(AutomationIDs.ScannerResultsFixFollowingTextBox);

            Assert.IsTrue(string.IsNullOrEmpty(fixFollowTb.Text));
            Assert.AreEqual(0, results.Count);
        }

        private void ValidateResultsInUIATree()
        {
            ValidateFirstSelectedElement();
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

        private IEnumerable<AppiumWebElement> GetPatternsNodes(string parentId, ReadOnlyCollection<AppiumWebElement> nonPatternNodes)
        {
            var parent = driver.FindElementByAccessibilityId(parentId);
            var allnodes = parent.FindElementsByClassName("TreeViewItem");
            var patterns = allnodes.Except(nonPatternNodes);

            foreach (var pattern in patterns)
            {
                pattern.SendKeys(Keys.Right);
            }

            allnodes = parent.FindElementsByClassName("TreeViewItem");
            return allnodes.Except(nonPatternNodes);
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