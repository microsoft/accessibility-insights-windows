// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UITests
{
    [TestClass]
    public class LiveMode : AIWinSession
    {
        readonly string TestAppPath = Path.GetFullPath("../../../../tools/WildlifeManager/WildlifeManager.exe");
        Process _wildlifeManager;

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory("NoStrongName")]
        [TestCategory("UITest")]
        public void LiveModeTests()
        {
            TestLiveMode();
            TestTestResults();
        }

        private void TestTestResults()
        {
            driver.LiveMode.RunTests();
            Thread.Sleep(2000); // let tests run

            VerifyTestModeTitle();
            ScanAccessibility();
        }

        private void TestLiveMode()
        {
            driver.LiveMode.TogglePause();

            VerifyLiveModeTitle();
            ScanAccessibility();
        }

        private void VerifyLiveModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Inspect - Live", driver.Title);
        }

        private void VerifyTestModeTitle()
        {
            Assert.AreEqual("Accessibility Insights for Windows - Test - Test results", driver.Title);
        }

        private void ScanAccessibility()
        {
            var issueCount = driver.ScanAIWin(TestContext, "EventPage");
            Assert.AreEqual(0, issueCount);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();

            _wildlifeManager = Process.Start(TestAppPath);
            RetryUntilSuccess(() => _wildlifeManager.MainWindowTitle == "Wildlife Manager 2.0", 1500, 10, _wildlifeManager.Refresh);
            Thread.Sleep(2000); // give ai-win a chance to find the app
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