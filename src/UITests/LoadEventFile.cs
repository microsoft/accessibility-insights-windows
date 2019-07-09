﻿// Copyright (c) Microsoft. All rights reserved.
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
        const string EventFileName = "WildlifeManagerTest.a11yevent";
        static readonly string TestFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestFiles");

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
            ScanWindow();
        }

        private void ScanWindow()
        {
            var issueCount = driver.ScanAIWin(TestContext, "EventPage");
            Assert.AreEqual(0, issueCount);
        }

        /// <summary>
        /// Click the first row in the event log
        /// </summary>
        private void CheckEventLog()
        {
            var eventGrid = driver.FindElementByAccessibilityId(AccessibilityInsights.SharedUx.Properties.AutomationIDs.EventModeControl);
            var rows = eventGrid.FindElementsByClassName("DataGridRow");

            rows[0].Click();

            // Text is populated after the cell above is selected (blank otherwise)
            Assert.AreEqual("09:58:37.859, EventRecorderNotification, Event Recorder", rows[0].Text);

            // 10 rows from the event log and 3 from the event details
            int numRows = GetNumEventDataRows();
            Assert.AreEqual(13, numRows);
        }

        /// <summary>
        /// Validate the event properties from the 3rd event (list item Owl)
        /// </summary>
        private void CheckPropertyView()
        {
            // Click on 3rd event to see its properties
            var eventGrid = driver.FindElementByAccessibilityId(AccessibilityInsights.SharedUx.Properties.AutomationIDs.EventsDataGrid);
            var eventRows = eventGrid.FindElementsByClassName("DataGridRow");
            eventRows[2].Click();

            // 10 rows from the event log and 10 from the event properties
            var rowCount = GetNumEventDataRows();
            Assert.AreEqual(20, rowCount);
        }

        private int GetNumEventDataRows()
        {
            var control = driver.FindElementByAccessibilityId(AccessibilityInsights.SharedUx.Properties.AutomationIDs.EventModeControl);
            var rows = control.FindElementsByClassName("DataGridRow");
            return rows.Count;
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