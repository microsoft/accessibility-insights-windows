// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Settings;
using Axe.Windows.Core.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AccessibilityInsights.SharedUxTests.Settings
{
    /// <summary>
    /// Tests for ConfigurationModel
    /// </summary>
    [TestClass()]
    public class ConfigurationModelTests
    {
        private static FixedConfigSettingsProvider testProvider;

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            string testBaseDirectory = Path.Combine(context.TestRunDirectory, context.FullyQualifiedTestClassName);

            var configDir = Path.Combine(testBaseDirectory, "config");
            var userDir = Path.Combine(testBaseDirectory, "user");
            testProvider = new FixedConfigSettingsProvider(configDir, userDir);
            Directory.CreateDirectory(testProvider.ConfigurationFolderPath);
        }

        /// <summary>
        /// Check serialization of selected properties
        /// </summary>
        [TestMethod()]
        public void ConfigurationModelTest()
        {
            string path = Path.Combine(testProvider.ConfigurationFolderPath, "config.test");

            var coreProps = PropertySettings.DefaultCoreProperties;

            ConfigurationModel config = new ConfigurationModel
            {
                CoreProperties = coreProps,
                Version = ConfigurationModel.CurrentVersion
            };

            config.SerializeInJSON(path);

            var newConfig = ConfigurationModel.LoadFromJSON(path, testProvider);
            File.Delete(path);

            Assert.IsTrue(coreProps.SequenceEqual(newConfig.CoreProperties));
        }

        [TestMethod()]
        public void GetCurrentConfigurationTest()
        {
            const string expectedHotKeyForRecord = "Recording HotKey";
            const string expectedMainWindowActivation = "Main Window Activation HotKey";
            string path = Path.Combine(testProvider.ConfigurationFolderPath, "config.test2");

            ConfigurationModel config = new ConfigurationModel
            {
                HotKeyForRecord = expectedHotKeyForRecord,
                HotKeyForActivatingMainWindow = expectedMainWindowActivation,
                Version = "1.0"
            };

            config.SerializeInJSON(path);

            var nc = ConfigurationModel.LoadFromJSON(path, testProvider);
            File.Delete(path);

            Assert.AreEqual(ConfigurationModel.CurrentVersion, nc.Version);
            Assert.AreEqual(expectedHotKeyForRecord, nc.HotKeyForRecord);
            Assert.AreEqual(expectedMainWindowActivation, nc.HotKeyForActivatingMainWindow);
        }

        [TestMethod]
        public void LoadFromJSON_FileDoesNotExist_DataIsCorrect()
        {
            ConfigurationModel config = GetDefaultConfig();

            Assert.IsTrue(config.AlwaysOnTop);
            Assert.AreEqual("1.1.", config.AppVersion.Substring(0, 4));
            ConfirmEnumerablesMatchExpectations(
                new int[] { 30005, 30003, 30004, 30009, 30001, 30007, 30006, 30013, 30102, 30101 },
                config.CoreProperties.ToArray());
            ConfirmEnumerablesMatchExpectations(new int[] { }, config.CoreTPAttributes.ToArray());
            Assert.IsFalse(config.DisableTestsInSnapMode);
            Assert.IsFalse(config.DisableDarkMode);
            Assert.IsFalse(config.EnableTelemetry);
            Assert.IsTrue(config.EventRecordPath.Equals(testProvider.UserDataFolderPath));
            Assert.AreEqual(FontSize.Standard, config.FontSize);
            Assert.AreEqual(HighlighterMode.HighlighterBeakerTooltip, config.HighlighterMode);
            Assert.AreEqual("Shift + F9", config.HotKeyForActivatingMainWindow);
            Assert.AreEqual("Control,Shift + F7", config.HotKeyForMoveToFirstChild);
            Assert.AreEqual("Control,Shift + F9", config.HotKeyForMoveToLastChild);
            Assert.AreEqual("Control,Shift + F8", config.HotKeyForMoveToNextSibling);
            Assert.AreEqual("Control,Shift + F6", config.HotKeyForMoveToParent);
            Assert.AreEqual("Control,Shift + F5", config.HotKeyForMoveToPreviousSibling);
            Assert.AreEqual("Shift + F5", config.HotKeyForPause);
            Assert.AreEqual("Shift + F7", config.HotKeyForRecord);
            Assert.AreEqual("Shift + F8", config.HotKeyForSnap);
            Assert.IsTrue(config.IsHighlighterOn);
            Assert.IsNull(config.IssueReporterSerializedConfigs);
            Assert.IsTrue(config.IsUnderElementScope);
            Assert.AreEqual(100, config.MouseSelectionDelayMilliSeconds);
            Assert.IsFalse(config.PlayScanningSound);
            Assert.AreEqual(ReleaseChannel.Production, config.ReleaseChannel);
            Assert.AreEqual(Guid.Empty, config.SelectedIssueReporter);
            Assert.IsTrue(config.SelectionByFocus);
            Assert.IsTrue(config.SelectionByMouse);
            Assert.IsFalse(config.ShowAllProperties);
            Assert.IsTrue(config.ShowAncestry);
            Assert.IsTrue(config.ShowTelemetryDialog);
            Assert.IsFalse(config.ShowUncertain);
            Assert.IsTrue(config.ShowWelcomeScreenOnLaunch);
            Assert.IsFalse(config.ShowWhitespaceInTextPatternViewer);
            Assert.IsTrue(config.TestReportPath.Equals(testProvider.UserDataFolderPath));
            Assert.AreEqual(TreeViewMode.Control, config.TreeViewMode);
            Assert.AreEqual(ReleaseChannel.Production, config.ReleaseChannel);
            Assert.AreEqual("1.1.10", config.Version);
            Assert.AreEqual(37, typeof(ConfigurationModel).GetProperties().Length, "Count of ConfigurationModel properties has changed! Please ensure that you are testing the default value for all properties, then update the expected value");
        }

        [TestMethod]
        public void LoadFronJSON_LegacyFormat_DataIsCorrect()
        {
            ConfigurationModel config = ConfigurationModel.LoadFromJSON(@".\Resources\LegacyConfigSettings.json", testProvider);

            ConfirmOverrideConfigMatchesExpectation(config);
        }

        [TestMethod]
        public void LoadFronJSON_CurrentFormat_DataIsCorrect()
        {
            ConfigurationModel config = ConfigurationModel.LoadFromJSON(@".\Resources\ConfigSettings.json", testProvider);

            ConfirmOverrideConfigMatchesExpectation(config,
                disableDarkMode: true,
                issueReporterSerializedConfigs: @"{""27f21dff-2fb3-4833-be55-25787fce3e17"":""hello world""}",
                selectedIssueReporter: new Guid("{27f21dff-2fb3-4833-be55-25787fce3e17}"),
                releaseChannel: ReleaseChannel.Canary
                );
        }

        private static ConfigurationModel GetDefaultConfig()
        {
            return ConfigurationModel.LoadFromJSON(null, testProvider);
        }

        private static void ConfirmOverrideConfigMatchesExpectation(ConfigurationModel config,
            Guid? selectedIssueReporter = null, string issueReporterSerializedConfigs = null,
            ReleaseChannel? releaseChannel = null, bool disableDarkMode = false)
        {
            Assert.IsFalse(config.AlwaysOnTop);
            Assert.AreEqual("1.1.", config.AppVersion.Substring(0, 4));
            Assert.AreNotEqual("1.1.700.1", config.AppVersion);

            ConfirmEnumerablesMatchExpectations(
                new int[] { 30005, 30003, 30004, 30009, 30001, 30007, 30006, 30013, 30102, 30101 },
                config.CoreProperties.ToArray());
            ConfirmEnumerablesMatchExpectations( new int[] { }, config.CoreTPAttributes.ToArray());
            Assert.IsFalse(config.DisableTestsInSnapMode);
            Assert.AreEqual(config.DisableDarkMode, disableDarkMode);
            Assert.IsFalse(config.EnableTelemetry);
            Assert.AreEqual(@"C:\blah\AccessibilityInsightsEventFiles", config.EventRecordPath);
            Assert.AreEqual(FontSize.Small, config.FontSize);
            Assert.AreEqual(HighlighterMode.HighlighterTooltip, config.HighlighterMode);
            Assert.AreEqual("Alt + F4", config.HotKeyForActivatingMainWindow);
            Assert.AreEqual("Alt + F6", config.HotKeyForMoveToFirstChild);
            Assert.AreEqual("Alt + F7", config.HotKeyForMoveToLastChild);
            Assert.AreEqual("Alt + F8", config.HotKeyForMoveToNextSibling);
            Assert.AreEqual("Alt + F5", config.HotKeyForMoveToParent);
            Assert.AreEqual("Alt + F9", config.HotKeyForMoveToPreviousSibling);
            Assert.AreEqual("Alt + F2", config.HotKeyForPause);
            Assert.AreEqual("Alt + F1", config.HotKeyForRecord);
            Assert.AreEqual("Alt + F3", config.HotKeyForSnap);
            Assert.IsTrue(config.IsHighlighterOn);
            Assert.AreEqual(issueReporterSerializedConfigs, config.IssueReporterSerializedConfigs);
            Assert.IsTrue(config.IsUnderElementScope);
            Assert.AreEqual(200, config.MouseSelectionDelayMilliSeconds);
            Assert.IsFalse(config.PlayScanningSound);
            Assert.AreEqual(releaseChannel ?? ReleaseChannel.Production, config.ReleaseChannel);
            Assert.AreEqual(selectedIssueReporter ?? Guid.Empty, config.SelectedIssueReporter);
            Assert.IsTrue(config.SelectionByFocus);
            Assert.IsTrue(config.SelectionByMouse);
            Assert.IsFalse(config.ShowAllProperties);
            Assert.IsTrue(config.ShowAncestry);
            Assert.IsFalse(config.ShowTelemetryDialog);
            Assert.IsFalse(config.ShowUncertain);
            Assert.IsTrue(config.ShowWelcomeScreenOnLaunch);
            Assert.IsFalse(config.ShowWhitespaceInTextPatternViewer);
            Assert.AreEqual(@"C:\blah\AccessibilityInsightsTestFiles", config.TestReportPath);
            Assert.AreEqual(TreeViewMode.Content, config.TreeViewMode);
            Assert.AreEqual("1.1.10", config.Version);
        }

        private static void ConfirmEnumerablesMatchExpectations(int[] expected, int[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int loop = 0; loop < expected.Length; loop++)
            {
                Assert.AreEqual(expected[loop], actual[loop], "Index = " + loop.ToString(CultureInfo.InvariantCulture));
            }
        }
    }
}
