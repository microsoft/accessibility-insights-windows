// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UITests.UILibrary
{
    public class TestMode
    {
        public AutomatedChecks AutomatedChecks { get; }
        public ResultsInUIATree ResultsInUIATree { get; }
        public TestMode(WindowsDriver<WindowsElement> session)
        {
            AutomatedChecks = new AutomatedChecks(session);
            ResultsInUIATree = new ResultsInUIATree(session);
        }
    }

    public class AutomatedChecks
    {
        readonly WindowsDriver<WindowsElement> Session;
        public AutomatedChecks(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }

        public void ViewInUIATree() => Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksUIATreeButton).Click();

        public void GoToAutomatedChecksElementDetails(int element)
        {
            var resultsGrid = Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksResultsListView);
            var results = resultsGrid.FindElementsByClassName("ListViewItem");

            results[element].FindElementByClassName("Button").Click();
        }

        public void ValidateAutomatedChecks(int? nonFrameworkErrorCount, int? frameworkErrorCount)
        {
            ValidateResultCountForSet(AutomationIDs.AutomatedChecksResultsListView, AutomationIDs.AutomatedChecksExpandAllButton, nonFrameworkErrorCount);
            ValidateResultCountForSet(AutomationIDs.AutomatedChecksFrameworkResultsListView, AutomationIDs.AutomatedChecksFrameworkExpandAllButton, frameworkErrorCount);
            var resultsText = Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksResultsTextBlock).Text;
            var resultTextCount = int.Parse(resultsText.Split()[0]);

            int expectedTotalResultCount = (nonFrameworkErrorCount ?? 0) + (frameworkErrorCount ?? 0);
            Assert.AreEqual(resultTextCount, expectedTotalResultCount);
        }

        private void ValidateResultCountForSet(string selector, string expandAllSelector, int? expectedErrorCount)
        {
            if (expectedErrorCount.HasValue)
            {
                var resultsGrid = Session.FindElementByAccessibilityId(selector);
                Session.FindElementByAccessibilityId(expandAllSelector).Click();
                var results = resultsGrid.FindElementsByClassName("ListViewItem");
                Assert.AreEqual(expectedErrorCount, results.Count);
            }
            else
            {
                Assert.ThrowsException<WebDriverException>(() => Session.FindElementByAccessibilityId(selector));
                Assert.ThrowsException<WebDriverException>(() => Session.FindElementByAccessibilityId(expandAllSelector));
            }
        }
    }

    public class ResultsInUIATree
    {
        readonly WindowsDriver<WindowsElement> Session;
        public ResultsInUIATree(WindowsDriver<WindowsElement> session)
        {
            Session = session;
        }
        public void BackToAutomatedChecks() => Session.FindElementByAccessibilityId(AutomationIDs.MainWinBreadCrumbTwoButton).Click();

        public void SwitchToResultsTab()
        {
            var tree = Session.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            var patterns = GetPatternsNodes(AutomationIDs.SnapshotModeControl, nodes);

            patterns.Last().SendKeys(Keys.Control + Keys.Tab);
        }

        public void SwitchToDetailsTab()
        {
            var resultsList = Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsListView);
            var resultsAll = resultsList.FindElementsByClassName("ListViewItem");

            resultsAll.First().SendKeys(Keys.Control + Keys.Tab);
        }

        public void SelectElementInTree(int element)
        {
            var tree = Session.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            nodes[element].SendKeys(Keys.Enter);
        }

        public void ValidateDetails(string firstPattern, string firstProperty, int patternCount, int propCount)
        {
            var snapshotModeControl = Session.FindElementByAccessibilityId(AutomationIDs.SnapshotModeControl);
            var props = snapshotModeControl.FindElementsByClassName("ListViewItem");
            props[0].Click();
            var tree = snapshotModeControl.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            var patterns = GetPatternsNodes(AutomationIDs.SnapshotModeControl, nodes);

            Assert.AreEqual(firstPattern, patterns.First().Text);
            Assert.AreEqual(firstProperty, props[0].Text);
            Assert.AreEqual(patternCount, patterns.Count());
            Assert.AreEqual(propCount, props.Count);
        }

        public void ValidateTree(string firstNodeText, int nodeCount)
        {
            var tree = Session.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");

            Assert.AreEqual(nodeCount, nodes.Count);

            // We're seeing the Text property here return different values on different versions of .NET framework.
            // As such, we only check for Contains (not Equals) here to make the test more flexible.
            Assert.IsTrue(nodes.First().Text.Contains(firstNodeText));
        }

        public void ValidateResults(int nonExpandedNonFrameworkResultsCount, int expandedNonFrameworkResultsCount,
            int nonExpandedFrameworkResultsCount, int expandedFrameworkResultsCount)
        {
            ValidateCurrentResultCount(AutomationIDs.ScannerResultsListView, nonExpandedNonFrameworkResultsCount);
            ValidateCurrentResultCount(AutomationIDs.ScannerResultsFrameworkResultsListView, nonExpandedFrameworkResultsCount);

            if (expandedNonFrameworkResultsCount > 0 || expandedFrameworkResultsCount > 0)
            {
                Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsShowAllButton).Click();
                var fixFollowTb = Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsFixFollowingTextBox);
                Assert.IsFalse(string.IsNullOrEmpty(fixFollowTb.Text));

                ValidateCurrentResultCount(AutomationIDs.ScannerResultsListView, expandedNonFrameworkResultsCount);
                ValidateCurrentResultCount(AutomationIDs.ScannerResultsFrameworkResultsListView, expandedFrameworkResultsCount);
            }
        }

        private void ValidateResultListIsCollapsed(string automationId)
        {
            Assert.ThrowsException<WebDriverException>(() => Session.FindElementByAccessibilityId(automationId));
        }

        private void ValidateCurrentResultCount(string automationId, int expectedResultsCount)
        {
            if (expectedResultsCount > 0)
            {
                var resultsList = Session.FindElementByAccessibilityId(automationId);
                var results = resultsList.FindElementsByClassName("ListViewItem");
                Assert.AreEqual(expectedResultsCount, results.Count);
            }
            else
            {
                ValidateResultListIsCollapsed(automationId);
            }
        }

        private IEnumerable<AppiumWebElement> GetPatternsNodes(string parentId, ReadOnlyCollection<AppiumWebElement> nonPatternNodes)
        {
            var parent = Session.FindElementByAccessibilityId(parentId);
            var allnodes = parent.FindElementsByClassName("TreeViewItem");
            var patterns = allnodes.Except(nonPatternNodes);

            foreach (var pattern in patterns)
            {
                pattern.SendKeys(Keys.Right);
            }

            allnodes = parent.FindElementsByClassName("TreeViewItem");
            return allnodes.Except(nonPatternNodes);
        }
    }
}
