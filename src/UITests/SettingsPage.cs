// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Properties;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using GitHubAutomationIDs = AccessibilityInsights.Extensions.GitHub.Properties.AutomationIDs;

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
        [TestCategory(TestCategory.NoStrongName), TestCategory(TestCategory.Integration)]
        public void SettingsPageTests()
        {
            CheckApplicationTab();
            CheckShortcut();
            CheckConnectionTab();
            CheckAboutTab();
        }

        /// <summary>
        /// Changes the shortcut for pause/resume and validates the shortcut works
        /// </summary>
        private void CheckShortcut()
        {
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsApplicationTabItem).Click();
            var pauseHotkey = driver.FindElementByAccessibilityId(AutomationIDs.SettingsHotkeyPauseButton);
            pauseHotkey.Click();

            var hotkeyDialog = driver.FindElementByAccessibilityId(AutomationIDs.SettingsHotkeyGrabDialog);
            hotkeyDialog.SendKeys(Keys.Shift);
            hotkeyDialog.SendKeys(Keys.F6);

            var saveAndClose = driver.FindElementByAccessibilityId(AutomationIDs.SettingsSaveAndCloseButton);
            Assert.IsTrue(saveAndClose.Enabled, "Save and close should be enabled");
            saveAndClose.Click();

            var pauseButton = driver.FindElementByAccessibilityId(AutomationIDs.MainWinPauseButton);
            Assert.IsTrue(pauseButton.Text.Contains("Pause"), "Live mode should be running, but the pause button doesn't say 'Pause'");
            var mainWindow = driver.FindElementByAccessibilityId(AutomationIDs.MainWindow);
            mainWindow.SendKeys(Keys.Shift + Keys.F6);
            Assert.IsTrue(pauseButton.Text.Contains("Resume"), "Live mode should be paused, but the pause button doesn't say 'Resume'");
        }

        /// <summary>
        /// Spot check the application tab and do an accessibility check
        /// </summary>
        private void CheckApplicationTab()
        {
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsApplicationTabItem).Click();

            var saveAndClose = driver.FindElementByAccessibilityId(AutomationIDs.SettingsSaveAndCloseButton);
            Assert.IsFalse(saveAndClose.Enabled, "Save and close should be disabled");

            var channelOptions = driver.FindElementByAccessibilityId(AutomationIDs.SettingsChannelComboBox);
            Assert.AreEqual("Production", channelOptions.Text, "Default channel should be production");
            channelOptions.SendKeys(Keys.ArrowDown);
            Assert.AreEqual("Insider", channelOptions.Text, "Insider should be the second channel in the combobox");
            Assert.IsTrue(saveAndClose.Enabled, "Save and close should be enabled after changing to insider");
            channelOptions.SendKeys(Keys.ArrowUp);
            Assert.AreEqual("Production", channelOptions.Text, "Insider should change to production after navigating up with the arrow key");
            Assert.IsFalse(saveAndClose.Enabled, "Save and close should be disabled after changing back to default production channel");
            driver.VerifyAccessibility(TestContext, "ApplicationTab", 0);
        }


        /// <summary>
        /// Spot check the connection tab and do an accessibility check
        /// </summary>
        private void CheckConnectionTab()
        {
            driver.GoToSettings();
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsConnectionTabItem).Click();

            var saveAndClose = driver.FindElementByAccessibilityId(AutomationIDs.SettingsSaveAndCloseButton);
            var connectionControl = driver.FindElementByAccessibilityId(AutomationIDs.ConnectionControl);
            var radioButtons = connectionControl.FindElementsByClassName("RadioButton");

            Assert.IsFalse(saveAndClose.Enabled, "Save and close should be disabled");
            Assert.AreEqual(2, radioButtons.Count, "There should be two connection extensions");
            Assert.IsTrue(radioButtons[0].Text.Contains("Azure Boards"), "The first connection should be Azure Boards");
            Assert.IsTrue(radioButtons[1].Text.Contains("GitHub"), "The second connection should be GitHub");

            radioButtons[1].Click();
            var urlTb = connectionControl.FindElementByAccessibilityId(GitHubAutomationIDs.IssueConfigurationUrlTextBox);
            urlTb.SendKeys("https://github.com/microsoft/accessibility-insights-windows");
            Assert.IsTrue(saveAndClose.Enabled, "The save and close button should be enabled after configuring GitHub");

            driver.VerifyAccessibility(TestContext, "ConnectionTab", 0);
        }

        /// <summary>
        /// Spot check the about tab and do an accessibility check
        /// </summary>
        private void CheckAboutTab()
        {
            driver.GoToSettings();
            driver.FindElementByAccessibilityId(AutomationIDs.SettingsAboutTabItem).Click();
            var noticesLink = driver.FindElementByAccessibilityId(AutomationIDs.SettingsThirdPartyNoticesHyperlink);
            Assert.AreEqual("Third party notices", noticesLink.Text, "The notices link text is incorrect");
            driver.VerifyAccessibility(TestContext, "AboutTab", 0);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Setup();
            driver.GettingStarted.DismissTelemetry();
            driver.GettingStarted.DismissStartupPage();
            driver.GoToSettings();
            driver.Maximize();
        }

        [TestCleanup]
        public void TestCleanup() => TearDown();
    }
}