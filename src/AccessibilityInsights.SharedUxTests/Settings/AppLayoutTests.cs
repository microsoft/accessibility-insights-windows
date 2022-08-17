// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.SharedUx.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.IO;

namespace AccessibilityInsights.SharedUxTests.Settings
{
    [TestClass()]
    public class AppLayoutTests
    {
        const string TestLayoutFile = "LayoutTest.Json";
        private static readonly FixedConfigSettingsProvider provider = FixedConfigSettingsProvider.CreateDefaultSettingsProvider();
        public static string folderPath = Path.Combine(provider.UserDataFolderPath, "AppLayoutTests");

        [ClassInitialize()]
        public static void ClassInit()
        {
            Directory.CreateDirectory(folderPath);
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            Directory.Delete(folderPath);
        }

        /// <summary>
        /// Tests LoadAppLayout and SerializeInJSON by creating a new AppLayout, serializing
        /// and saving it, and then reading it back into a new AppLayout. Values are then
        /// compared to original values.
        /// </summary>
        [TestMethod()]
        public void LoadDefaultAppLayoutTest()
        {
            string path = Path.Combine(provider.UserDataFolderPath, TestLayoutFile);
            AppLayout al = new AppLayout(10, 11);
            al.LayoutLive.Height = 800;
            al.LayoutSnapshot.RowPropertiesHeight = 500;

            al.SerializeInJSON(path);

            AppLayout loadedAl = AppLayout.LoadFromJSON<AppLayout>(path);
            loadedAl.LoadLayoutIfPrevVersion(2, 2);

            System.IO.File.Delete(path);

            Assert.AreEqual(460, loadedAl.LayoutLive.Width);
            Assert.AreEqual(800, loadedAl.LayoutLive.Height);
            Assert.AreEqual(46 * 7, loadedAl.LayoutLive.RowPropertiesHeight);
            Assert.AreEqual(0, loadedAl.LayoutLive.RowTestHeight);

            Assert.AreEqual(870, loadedAl.LayoutSnapshot.Width);
            Assert.AreEqual(720, loadedAl.LayoutSnapshot.Height);
            Assert.AreEqual(410, loadedAl.LayoutSnapshot.ColumnSnapWidth);
            Assert.AreEqual(500, loadedAl.LayoutSnapshot.RowPropertiesHeight);
            Assert.AreEqual(260, loadedAl.LayoutSnapshot.RowTestHeight);

            Assert.AreEqual(AppLayout.CurrentVersion, loadedAl.Version);
            Assert.AreEqual(10, loadedAl.Top);
            Assert.AreEqual(11, loadedAl.Left);
        }

        /// <summary>
        /// Tests AppLayout constructor against given values and expected
        /// default values.
        /// </summary>
        [TestMethod()]
        public void AppLayoutTest()
        {
            AppLayout al = new AppLayout(10, 11);

            Assert.AreEqual(460, al.LayoutLive.Width);
            Assert.AreEqual(500, al.LayoutLive.Height);
            Assert.AreEqual(410, al.LayoutLive.ColumnSnapWidth);
            Assert.AreEqual(0, al.LayoutLive.RowTestHeight);
            Assert.AreEqual(46 * 7, al.LayoutLive.RowPropertiesHeight);

            Assert.AreEqual(870, al.LayoutSnapshot.Width);
            Assert.AreEqual(720, al.LayoutSnapshot.Height);
            Assert.AreEqual(410, al.LayoutSnapshot.ColumnSnapWidth);
            Assert.AreEqual(260, al.LayoutSnapshot.RowTestHeight);
            Assert.AreEqual(46 * 7, al.LayoutLive.RowPropertiesHeight);

            Assert.AreEqual(10, al.Top);
            Assert.AreEqual(11, al.Left);
        }

        /// <summary>
        /// Tests RemoveConfigurationTest by removing any existing backups, writing
        /// a new AppLayout, and removing it. A backup is then verified to have
        /// been created.
        /// </summary>
        [TestMethod()]
        public void RemoveConfigurationTest()
        {
            string path = Path.Combine(provider.UserDataFolderPath, TestLayoutFile);
            foreach (string file in Directory.EnumerateFiles(provider.UserDataFolderPath))
            {
                if (file.IndexOf(path) == 0 && file.Contains(".bak"))
                {
                    File.Delete(file);
                }
            }
            AppLayout al = new AppLayout(10, 11);

            al.SerializeInJSON(path);
            ConfigurationBase.RemoveConfiguration(path);
            Assert.IsFalse(File.Exists(path));
            bool bakExists = false;
            foreach (string file in Directory.EnumerateFiles(provider.UserDataFolderPath))
            {
                if (file.IndexOf(path) == 0 && file.Contains(".bak"))
                {
                    bakExists = true;
                    File.Delete(file);
                    break;
                }
            }

            Assert.IsTrue(bakExists);
        }

        /// <summary>
        /// Test: replacing old layout with new one if version is change.
        /// </summary>
        [TestMethod()]
        public void LoadLayoutIfPrevVersionTest()
        {
            var al = FromJson("Resources/Layout_S122.json");

            Assert.AreEqual(0.0, al.LayoutLive.ColumnSnapWidth);
            Assert.AreEqual(1338.6666666666665, al.Left);
            Assert.AreEqual(160.66666666666666, al.Top);
            Assert.AreEqual(410.0, al.LayoutSnapshot.ColumnSnapWidth);
            Assert.AreEqual(0, al.LayoutSnapshot.RowPatternsHeight);
            Assert.AreEqual(0.0, al.LayoutLive.ColumnSnapWidth);
            Assert.AreEqual(0, al.LayoutLive.RowPatternsHeight);
            Assert.AreEqual("1.0.1", al.Version);

            al.LoadLayoutIfPrevVersion(0, 0);
            Assert.AreEqual(410.0, al.LayoutLive.ColumnSnapWidth);
            Assert.AreEqual(410.0, al.LayoutSnapshot.ColumnSnapWidth);
            Assert.AreEqual(138.0, al.LayoutSnapshot.RowPatternsHeight);
            Assert.AreEqual(410.0, al.LayoutLive.ColumnSnapWidth);
            Assert.AreEqual(138, al.LayoutLive.RowPatternsHeight);
            Assert.AreEqual(0, al.Left);
            Assert.AreEqual(0, al.Top);
            Assert.AreEqual(AppLayout.CurrentVersion, al.Version);
        }

        /// <summary>
        /// Deserialize saved A11yElement
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AppLayout FromJson(string path)
        {
            AppLayout element = null;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                element = JsonConvert.DeserializeObject<AppLayout>(json);
            }

            return element;
        }
    }
}
