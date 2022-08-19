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
            public int? AutomatedChecks_FrameworkErrorCount;
            public int UIATree_AllFrameworkResultsCount;
        }

        readonly string TestFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFiles");

        // These values should only change in response to a change in Axe.Windows or WildlifeManager
        const int AutomatedChecks_NonFrameworkErrorCount = 12;
        const int AutomatedChecks_FrameworkErrorCount = 3;
        const int UIATree_FailedResultsCount = 2;
        const int UIATree_AllResultsCount = 28;
        const int UIATree_FailedFrameworksResultsCount = 0;
        const int UIATree_AllFrameworkResultsCount = 2;

        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void LoadFileWithNoFrameworkIssues()
        {
            LoadTestEngine(new TestCase
            {
                TestFile = "WildlifeManagerTest.a11ytest"
            });
        }

        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void LoadFileWithFrameworkIssues()
        {
            LoadTestEngine(new TestCase
            {
                TestFile = "WildlifeManagerTestWithFrameworkIssues.a11ytest",
                AutomatedChecks_FrameworkErrorCount = AutomatedChecks_FrameworkErrorCount,
                UIATree_AllFrameworkResultsCount = UIATree_AllFrameworkResultsCount,
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
                driver.TestMode.AutomatedChecks.ValidateAutomatedChecks(AutomatedChecks_NonFrameworkErrorCount, testCase.AutomatedChecks_FrameworkErrorCount);
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
            driver.TestMode.ResultsInUIATree.ValidateResults(UIATree_FailedResultsCount, UIATree_AllResultsCount, UIATree_FailedFrameworksResultsCount, testCase.UIATree_AllFrameworkResultsCount);
            driver.TestMode.ResultsInUIATree.SwitchToDetailsTab();
            driver.TestMode.ResultsInUIATree.ValidateDetails("InvokePattern", "Name: Ok", 3, 10);
            driver.TestMode.ResultsInUIATree.ValidateTree("pane 'Desktop 1' has failed test results in descendants.", 16);
        }

        private void ValidateRootElement()
        {
            driver.TestMode.ResultsInUIATree.ValidateDetails("LegacyIAccessiblePattern", "Name: Desktop 1", 10, 10);
            driver.TestMode.ResultsInUIATree.SwitchToResultsTab();
            driver.TestMode.ResultsInUIATree.ValidateResults(0, 0, 0, 0);
        }

        private void ValidateResultsInUIATree(TestCase testCase)
        {
            ValidateFirstSelectedElement(testCase);
            driver.TestMode.ResultsInUIATree.SelectElementInTree(0);
            ValidateRootElement();
        }
    }
}
