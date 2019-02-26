// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Desktop.UIAutomation;
using AccessibilityInsights.DesktopUI.Enums;
using AccessibilityInsights.RuleSelection;
using AccessibilityInsights.SharedUx.Enums;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public static string folderPath = Path.Combine(DirectoryManagement.sUserDataFolderPath, "ConfigurationTests");

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            Directory.CreateDirectory(folderPath);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Directory.Delete(folderPath);
        }

        /// <summary>
        /// Check serialization of selected properties
        /// </summary>
        [TestMethod()]
        public void ConfigurationModelTest()
        {
            string path = Path.Combine(DirectoryManagement.sUserDataFolderPath,"config.test");

            var coreProps = DesktopElementHelper.GetDefaultCoreProperties();

            ConfigurationModel config = new ConfigurationModel
            {
                CoreProperties = coreProps,
                Version = ConfigurationModel.CurrentVersion
            };

            config.SerializeInJSON(path);

            var newConfig = ConfigurationModel.LoadFromJSON(path);
            File.Delete(path);

            Assert.IsTrue(coreProps.SequenceEqual(newConfig.CoreProperties));
        }

        [TestMethod()]
        public void GetCurrentConfigurationTest()
        {
            const string expectedHotKeyForRecord = "Recording HotKey";
            const string expectedMainWindowActivation = "Main Window Activation HotKey";
            string path = Path.Combine(DirectoryManagement.sUserDataFolderPath, "config.test2");

            ConfigurationModel config = new ConfigurationModel
            {
                HotKeyForRecord = expectedHotKeyForRecord,
                HotKeyForActivatingMainWindow = expectedMainWindowActivation,
                Version = "1.0"
            };

            config.SerializeInJSON(path);

            var nc = ConfigurationModel.LoadFromJSON(path);
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
            Assert.IsNull(config.CachedConnections);
            ConfirmEnumerablesMatchExpectations(
                new int[] { 30005, 30003, 30004, 30009, 30001, 30007, 30006, 30013, 30102, 30101 },
                config.CoreProperties.ToArray());
            ConfirmEnumerablesMatchExpectations(new int[] { }, config.CoreTPAttributes.ToArray());
            Assert.IsFalse(config.DisableTestsInSnapMode);
            Assert.IsTrue(config.EnableTelemetry);
            Assert.IsTrue(config.EventRecordPath.EndsWith(@"\AccessibilityInsights"), config.EventRecordPath);
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
            Assert.IsTrue(config.IsUnderElementScope);
            Assert.AreEqual(100, config.MouseSelectionDelayMilliSeconds);
            Assert.IsFalse(config.PlayScanningSound);
            Assert.IsNull(config.SavedConnection);
            Assert.IsTrue(config.SelectionByFocus);
            Assert.IsTrue(config.SelectionByMouse);
            Assert.IsNull(config.SerializedCachedConnections);
            Assert.IsNull(config.SerializedSavedConnection);
            Assert.IsFalse(config.ShowAllProperties);
            Assert.IsTrue(config.ShowAncestry);
            Assert.IsTrue(config.ShowTelemetryDialog);
            Assert.IsFalse(config.ShowUncertain);
            Assert.IsTrue(config.ShowWelcomeScreenOnLaunch);
            Assert.IsFalse(config.ShowWhitespaceInTextPatternViewer);
            Assert.AreEqual(SuiteConfigurationType.Default, config.TestConfig);
            Assert.IsTrue(config.TestReportPath.EndsWith(@"\AccessibilityInsights"), config.TestReportPath);
            Assert.AreEqual(TreeViewMode.Control, config.TreeViewMode);
            Assert.AreEqual("1.1.10", config.Version);
            Assert.AreEqual(100, config.ZoomLevel);

            Assert.AreEqual(39, typeof(ConfigurationModel).GetProperties().Length, "Count of ConfigurationModel properties has changed! Please ensure that you are testing the default value for all properties, then update the expected value");
        }

        [TestMethod]
        public void LoadFronJSON_LegacyFormat_DataIsCorrect()
        {
            ConfigurationModel config = ConfigurationModel.LoadFromJSON(@"..\..\Resources\LegacyConfigSettings.json");

            ConfirmSharedOverrideConfigMatchesExpectation(config); 
        }

        [TestMethod]
        public void LoadFronJSON_CurrentFormat_DataIsCorrect()
        {
            ConfigurationModel config = ConfigurationModel.LoadFromJSON(@"..\..\Resources\ConfigSettings.json");

            ConfirmSharedOverrideConfigMatchesExpectation(config);
        }

        private static ConfigurationModel GetDefaultConfig()
        {
            return ConfigurationModel.LoadFromJSON(@"..\..\Resources\ThisFileDoesNotExist.json");
        }

        private static void ConfirmSharedOverrideConfigMatchesExpectation(ConfigurationModel config)
        {
            Assert.IsFalse(config.AlwaysOnTop);
            Assert.AreEqual("1.1.", config.AppVersion.Substring(0, 4));
            Assert.AreNotEqual("1.1.700.1", config.AppVersion);
            Assert.IsNull(config.CachedConnections);
            ConfirmEnumerablesMatchExpectations(
                new int[] { 30005, 30003, 30004, 30009, 30001, 30007, 30006, 30013, 30102, 30101 },
                config.CoreProperties.ToArray());
            ConfirmEnumerablesMatchExpectations( new int[] { }, config.CoreTPAttributes.ToArray());
            Assert.IsFalse(config.DisableTestsInSnapMode);
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
            Assert.IsTrue(config.IsUnderElementScope);
            Assert.AreEqual(200, config.MouseSelectionDelayMilliSeconds);
            Assert.IsFalse(config.PlayScanningSound);
            Assert.IsNull(config.SavedConnection);
            Assert.IsTrue(config.SelectionByFocus);
            Assert.IsTrue(config.SelectionByMouse);
            Assert.AreEqual("[]", config.SerializedCachedConnections);
            Assert.AreEqual("", config.SerializedSavedConnection);
            Assert.IsFalse(config.ShowAllProperties);
            Assert.IsTrue(config.ShowAncestry);
            Assert.IsFalse(config.ShowTelemetryDialog);
            Assert.IsFalse(config.ShowUncertain);
            Assert.IsTrue(config.ShowWelcomeScreenOnLaunch);
            Assert.IsFalse(config.ShowWhitespaceInTextPatternViewer);
            Assert.AreEqual(SuiteConfigurationType.MicrosoftStandard, config.TestConfig);
            Assert.AreEqual(@"C:\blah\AccessibilityInsightsTestFiles", config.TestReportPath);
            Assert.AreEqual(TreeViewMode.Content, config.TreeViewMode);
            Assert.AreEqual("1.1.10", config.Version);
            Assert.AreEqual(350, config.ZoomLevel);
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
