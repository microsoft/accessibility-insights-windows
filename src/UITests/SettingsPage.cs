﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace UITests
{
    [TestClass]
    public class SettingsPage : AIWinSession
    {
        /// <summary>
        /// The entry point for this test scenario. Every TestMethod  will restart ai-win, so
        /// we want to use them sparingly.
        /// </summary>
        [TestMethod]
        [TestCategory("NoStrongName")]
        [TestCategory("UITest")]
        public void SettingsPageTests()
        {
            CheckApplicationTab();
            CheckShortcut();
            CheckAboutTab();
        }

        /// <summary>
        /// Changes the shortcut for pause/resume and validates the shortcut works
        /// </summary>
        private void CheckShortcut()
        {
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsApplicationTabItem).Click();
            var pauseHotkey = driver.FindElementByAccessibilityId(AutomationIDs.HotkeyPauseButton);
            pauseHotkey.Click();

            var hotkeyDialog = driver.FindElementByAccessibilityId(AutomationIDs.HotkeyGrabDialog);
            hotkeyDialog.SendKeys(Keys.Shift);
            hotkeyDialog.SendKeys(Keys.F6);

            var saveAndClose = driver.FindElementByAccessibilityId(AutomationIDs.SaveAndCloseButton);
            Assert.IsTrue(saveAndClose.Enabled);
            saveAndClose.Click();

            var pauseButton = driver.FindElementByAccessibilityId(AutomationIDs.PauseButton);
            Assert.IsTrue(pauseButton.Text.Contains("Pause"));
            var mainWindow = driver.FindElementByAccessibilityId(AutomationIDs.MainWindow);
            mainWindow.SendKeys(Keys.Shift + Keys.F6);
            Assert.IsTrue(pauseButton.Text.Contains("Resume"));
        }

        /// <summary>
        /// Spot check the application tab and do an accessibility check
        /// </summary>
        private void CheckApplicationTab()
        {
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsApplicationTabItem).Click();

            var saveAndClose = driver.FindElementByAccessibilityId(AutomationIDs.SaveAndCloseButton);
            Assert.IsFalse(saveAndClose.Enabled);

            var channelOptions = driver.FindElementByAccessibilityId(AutomationIDs.ChannelComboBox);
            Assert.AreEqual("Production", channelOptions.Text);
            channelOptions.SendKeys(Keys.ArrowDown);
            Assert.AreEqual("Insider", channelOptions.Text);
            Assert.IsTrue(saveAndClose.Enabled);
            channelOptions.SendKeys(Keys.ArrowUp);
            Assert.AreEqual("Production", channelOptions.Text);
            Assert.IsFalse(saveAndClose.Enabled);
            CheckAccessibility("ApplicationTab");
        }

        /// <summary>
        /// Spot check the about tab and do an accessibility check
        /// </summary>
        private void CheckAboutTab()
        {
            driver.GoToSettings();
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsAboutTabItem).Click();
            var noticesLink = driver.FindElementByAccessibilityId(AutomationIDs.ThirdPartyNoticesHyperlink);
            Assert.AreEqual("Third party notices", noticesLink.Text);
            CheckAccessibility("AboutTab");
        }

        private void CheckAccessibility(string name)
        {
            var issueCount = driver.ScanAIWin(TestContext, name);
            Assert.AreEqual(0, issueCount);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();
            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();
            driver.ToggleHighlighter();
            driver.GoToSettings();
            driver.Maximize();
        }

        [TestCleanup]
        public void TestCleanup() => TearDown();
    }
}