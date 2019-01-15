// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Desktop.UIAutomation;
using AccessibilityInsights.Desktop.Utility;
using AccessibilityInsights.SharedUx.Settings;
using AccessibilityInsights.SharedUx.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

            var newConfig = ConfigurationModel.LoadFromJSON<ConfigurationModel>(path);
            System.IO.File.Delete(path);

            Assert.IsTrue(coreProps.SequenceEqual(newConfig.CoreProperties));
        }

        [TestMethod()]
        public void GetCurrentConfigurationTest()
        {
            string path = Path.Combine(DirectoryManagement.sUserDataFolderPath, "config.test2");

            ConfigurationModel config = new ConfigurationModel
            {
                HotKeyForRecord = ConfigurationModel.OldHotKeyRecord,
                HotKeyForActivatingMainWindow = "Windows + 5",
                Version = "1.0"
            };

            config.SerializeInJSON(path);

            var nc = ConfigurationModel.LoadConfiguration(path);
            System.IO.File.Delete(path);

            Assert.AreEqual(ConfigurationModel.CurrentVersion, nc.Version);
            Assert.AreEqual(ConfigurationModel.DefaultHotKeyRecord, nc.HotKeyForRecord);
            Assert.AreEqual("Windows + 5", nc.HotKeyForActivatingMainWindow);
        }
    }
}
