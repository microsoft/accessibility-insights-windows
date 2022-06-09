// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace UITests
{
    /// <summary>
    /// Tests around file load. Note that every TestMethod will restart ai-win, so use them sparingly.
    /// </summary>
    [TestClass]
    public class LoadTestFile : AIWinSession
    {
        private struct TestCase
        {
            public string TestFile;
            public int? AutomatedChecks_NonFrameworkErrorCount;
            public int? AutomatedChecks_FrameworkErrorCount;
            public int UIATree_FailedResultsCount;
            public int UIATree_AllResultsCount;
        }
        readonly string TestFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFiles");

        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void LoadFileWithNoFrameworkIssues()
        {
            LoadTestEngine(new TestCase
            {
                TestFile = "WildlifeManagerTest.a11ytest",
                AutomatedChecks_NonFrameworkErrorCount = 12,
                AutomatedChecks_FrameworkErrorCount = null,
                UIATree_FailedResultsCount = 2,
                UIATree_AllResultsCount = 28,
            });
        }

        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void LoadFileWithFrameworkIssues()
        {
            LoadTestEngine(new TestCase
            {
                TestFile = "WildlifeManagerTestWithFrameworkIssues.a11ytest",
                AutomatedChecks_NonFrameworkErrorCount = 12,
                AutomatedChecks_FrameworkErrorCount = 3,
                UIATree_FailedResultsCount = 2,
                UIATree_AllResultsCount = 30,
            });
        }

        private void LoadTestEngine(TestCase testCase)
        /// <summary>
        /// Start the app, open an A11yTest file, navigate to 2 views (automated checks and UIA hierarchy), spot check loaded data, exit app
        /// </summary>
        {
            Setup();

            try
            {
                driver.GettingStarted.DismissTelemetry();
                driver.GettingStarted.DismissStartupPage();
                driver.ToggleHighlighter();

                driver.LiveMode.OpenFile(TestFilePath, testCase.TestFile);
                driver.VerifyAccessibility(TestContext, "AutomatedChecks", 0);
                ScanResultsInUIATreePage();
                driver.TestMode.AutomatedChecks.ValidateAutomatedChecks(testCase.AutomatedChecks_NonFrameworkErrorCount, testCase.AutomatedChecks_FrameworkErrorCount);
                driver.TestMode.AutomatedChecks.GoToAutomatedChecksElementDetails(0);
                ValidateResultsInUIATree(testCase);
            }
            finally
            {
                TearDown();
            }
        }

        private void ScanResultsInUIATreePage()
        {
            driver.TestMode.AutomatedChecks.ViewInUIATree();

            driver.VerifyAccessibility(TestContext, "UIATree", 0);
            driver.TestMode.ResultsInUIATree.BackToAutomatedChecks();
        }

        private void ValidateFirstSelectedElement(TestCase testCase)
        {
            driver.TestMode.ResultsInUIATree.ValidateResults(testCase.UIATree_FailedResultsCount, testCase.UIATree_AllResultsCount);
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

        private void ValidateResultsInUIATree(TestCase testCase)
        {
            ValidateFirstSelectedElement(testCase);
            driver.TestMode.ResultsInUIATree.SelectElementInTree(0);
            ValidateRootElement();
        }
    }
}