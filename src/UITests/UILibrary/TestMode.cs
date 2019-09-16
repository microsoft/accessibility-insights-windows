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
        WindowsDriver<WindowsElement> Session;
        public AutomatedChecks AutomatedChecks { get; }
        public ResultsInUIATree ResultsInUIATree { get; }
        public TestMode(WindowsDriver<WindowsElement> session)
        {
            Session = session;
            AutomatedChecks = new AutomatedChecks(session);
            ResultsInUIATree = new ResultsInUIATree(session);
        }
    }

    public class AutomatedChecks
    {
        WindowsDriver<WindowsElement> Session;
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

        public void ValidateAutomatedChecks(int resultCount)
        {
            Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksExpandAllButton).Click();
            var resultsGrid = Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksResultsListView);
            var results = resultsGrid.FindElementsByClassName("ListViewItem");
            var resultsText = Session.FindElementByAccessibilityId(AutomationIDs.AutomatedChecksResultsTextBlock).Text;
            var resultTextCount = int.Parse(resultsText.Split()[0]);

            Assert.AreEqual(resultCount, resultTextCount);
            Assert.AreEqual(resultTextCount, results.Count);
        }
    }

    public class ResultsInUIATree
    {
        WindowsDriver<WindowsElement> Session;
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
            var resultsList = Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsDetailsListView);
            var resultsAll = resultsList.FindElementsByClassName("ListViewItem");

            resultsAll.First().SendKeys(Keys.Control + Keys.Tab);
        }

        public void SelectElementInTree(int element)
        {
            var tree = Session.FindElementByAccessibilityId(AutomationIDs.HierarchyControlUIATreeView);
            var nodes = tree.FindElementsByClassName("TreeViewItem");
            nodes[0].SendKeys(Keys.Enter);
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

        public void ValidateResults(int failedResultsCount, int allResultsCount)
        {
            var resultsList = Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsDetailsListView);
            var resultsFailedOnly = resultsList.FindElementsByClassName("ListViewItem");

            if (allResultsCount > 0)
            {
                Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsShowAllButton).Click();
                var fixFollowTb = Session.FindElementByAccessibilityId(AutomationIDs.ScannerResultsFixFollowingTextBox);
                Assert.IsFalse(string.IsNullOrEmpty(fixFollowTb.Text));
            }

            var resultsAll = resultsList.FindElementsByClassName("ListViewItem");
            Assert.AreEqual(failedResultsCount, resultsFailedOnly.Count);
            Assert.AreEqual(allResultsCount, resultsAll.Count);
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