// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Win32;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace UITests
{
    [TestClass]
    public class LiveMode : AIWinSession
    {
        readonly string TestAppPath = Path.GetFullPath("../../../../../tools/WildlifeManager/WildlifeManager.exe");
        Process _wildlifeManager;

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration), TestCategory(TestCategory.LiveMode)]
        public void LiveModeTests()
        {
            TestLiveMode();
            TestTestResults();
        }

        private void TestTestResults()
        {
            RunTests();
            driver.VerifyAccessibility(TestContext, "LiveMode", 0);
            ValidateTestModeTitle();
            driver.TestMode.AutomatedChecks.ValidateAutomatedChecks(12);
            driver.TestMode.AutomatedChecks.GoToAutomatedChecksElementDetails(4);
            ValidateResultsInUIATree();
        }

        private void ValidateResultsInUIATree()
        {
            int expectedResultsCount = 13; // this number should only change if axe-windows or WildlifeManager are modified.

            driver.TestMode.ResultsInUIATree.ValidateResults(2, expectedResultsCount);
            driver.TestMode.ResultsInUIATree.SwitchToDetailsTab();
            driver.TestMode.ResultsInUIATree.ValidateDetails("SynchronizedInputPattern", "Name: Property does not exist", 1, 10);
            driver.TestMode.ResultsInUIATree.ValidateTree("pane 'Desktop 1'", 16);
        }

        private void RunTests()
        {
            driver.LiveMode.RunTests();
            var testsComplete = WaitFor(() => !driver.Title.Contains("(Scanning)"), new TimeSpan(0, 0, 0, 0, 500), 10);

            Assert.IsTrue(testsComplete, "Tests did not complete - (Scanning) title was never found");
        }

        private void TestLiveMode()
        {
            var appOpened = WaitFor(() => _wildlifeManager.MainWindowTitle == "Wildlife Manager 2.0", new TimeSpan(0, 0, 1), 10, _wildlifeManager.Refresh);

            // set focus on ai win and then WildlifeManager to make sure the app is selected.
            driver.FocusWindowByResizing();
            NativeMethods.SetForegroundWindow(_wildlifeManager.MainWindowHandle);

            var appSelected = WaitFor(() => driver.LiveMode.SelectedElementText == "window 'Wildlife Manager 2.0'", new TimeSpan(0, 0, 1), 5);
            driver.LiveMode.TogglePause();

            Assert.IsTrue(appOpened, "Wildlife Manager may not have opened, the window title was not found.");
            Assert.IsTrue(appSelected, "Wildlife Manager was not selected in Live Mode.");
            VerifyLiveModeTitle();
        }

        private void VerifyLiveModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Inspect - Live", driver.Title, "Live mode title is incorrect");
        }

        private void ValidateTestModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Test - Test results", driver.Title, "Test mode title is incorrect");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();

            _wildlifeManager = Process.Start(TestAppPath);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _wildlifeManager?.CloseMainWindow();
            _wildlifeManager?.Kill();
            TearDown();
        }
    }
}
