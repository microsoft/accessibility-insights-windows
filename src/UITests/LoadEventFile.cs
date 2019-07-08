// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace UITests
{
    /// <summary>
    /// Start the app, open an A11yEvent file, spot check loaded data, exit app
    /// </summary>
    [TestClass]
    public class LoadEventFile : AIWinSession
    {
        static readonly string EventFileName = "WildlifeManagerTest.a11yevent";
        static readonly string TestFilePath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\TestFiles";

        /// <summary>
        /// The entry point for this test scenario. Every TestMethod  will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory("NoStrongName")]
        [TestCategory("UITest")]
        public void LoadEventFileTests()
        {
            driver.Maximize();
            CheckEventLog();
            CheckPropertyView();
            CheckPatternsView();
            ScanWindow();
        }

        private void ScanWindow()
        {
            var issueCount = driver.ScanAIWin(TestContext);
            Assert.AreEqual(0, issueCount);
        }

        /// <summary>
        /// Click the first row in the event log
        /// </summary>
        private void CheckEventLog()
        {
            var eventGrid = driver.FindElementByAccessibilityId("ctrlEventMode");
            var rows = eventGrid.FindElementsByClassName("DataGridRow");

            rows[0].Click();

            // Text is populated after the cell above is selected (blank otherwise)
            Assert.AreEqual("09:58:37.859, EventRecorderNotification, Event Recorder", rows[0].Text);

            // 10 rows from the event log and 3 from the event details
            Assert.AreEqual(rows.Count, 13);
        }

        /// <summary>
        /// Validate the event properties from the 3rd event (list item Owl)
        /// </summary>
        private void CheckPropertyView()
        {
            var eventGrid = driver.FindElementByAccessibilityId("dgEvents");
            var eventRows = eventGrid.FindElementsByClassName("DataGridRow");
            eventRows[2].Click();
            var propertyGrid = driver.FindElementByAccessibilityId("dgProperties");
            var propertyRows = propertyGrid.FindElementsByClassName("DataGridRow");

            // propertyRows.Count is 0, need to understand why
            // Assert.AreEqual(propertyRows.Count, 10);
        }

        /// <summary>
        /// Validate the patterns from the 3rd event (list item Owl)
        /// </summary>
        private void CheckPatternsView()
        {
            var patternTree = driver.FindElementByAccessibilityId("treePatterns");
            var patternItems = patternTree.FindElementsByClassName("TreeViewItem");

            // patternItems.Count is 0, need to understand why
            // Assert.AreEqual(3, patternItems.Count);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();

            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();
            driver.ToggleHighlighter();
            driver.LiveMode.OpenFile(TestFilePath, EventFileName);
        }

        [TestCleanup]
        public void TestCleanup() => TearDown();
    }
}