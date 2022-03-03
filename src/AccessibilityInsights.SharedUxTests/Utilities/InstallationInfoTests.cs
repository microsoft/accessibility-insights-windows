// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AccessibilityInsights.SharedUxTests.Utilities
{
    [TestClass]
    public class InstallationInfoTests
    {
        private static readonly Guid InstallationGuid = new Guid("FF812E13-1061-4AA9-B236-F1628EB0D311");
        private static readonly Guid InstallationGuid2 = new Guid("8E2668D8-4036-455E-B7C2-BEFFE446A1F7");
        private static readonly Guid InstallationGuid3 = new Guid("34462CAE-4C96-47F5-9DED-520FEA62ADA6");
        private static readonly DateTime January2015 = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime February2015 = new DateTime(2015, 2, 1, 0, 0, 0, DateTimeKind.Utc);
        private static readonly DateTime January2016 = new DateTime(2016, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private string _testFolder;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _testFolder = Path.Combine(Path.GetTempPath(), "InstallationInfoTest_" + Guid.NewGuid().ToString());
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            if (Directory.Exists(_testFolder))
            {
                Directory.Delete(_testFolder, /*recursive*/true);
            }

            InstallationInfo.ReadFromDiskOverride = null;
            InstallationInfo.WriteToDiskOverride = null;
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_InstallationIdAndLastResetAreCorrectlySet()
        {
            DateTime[] dates = { January2015, February2015, January2016 };
            Guid[] guids = { InstallationGuid, InstallationGuid2, InstallationGuid3 };
            foreach (DateTime date in dates)
            {
                foreach (Guid guid in guids)
                {
                    InstallationInfo info = new InstallationInfo(guid, date);
                    Assert.AreEqual(date, info.LastReset);
                    Assert.AreEqual(guid, info.InstallationGuid);
                }
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadFromPath_FileDoesNotExist_LastResetIsProvidedTime()
        {
            DateTime now = January2015;
            bool wasRefreshed = false;
            InstallationInfo.WriteToDiskOverride = (_, __) => wasRefreshed = true;

            InstallationInfo info = InstallationInfo.LoadFromPath(_testFolder, now);

            Assert.IsTrue(wasRefreshed);
            Assert.AreEqual(now, info.LastReset);
            Assert.AreNotEqual(InstallationGuid, info.InstallationGuid);
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadFromPath_FileIsCurrent_FileIsNotRefreshed()
        {
            DateTime now = January2015;
            bool wasRefreshed = false;
            InstallationInfo.WriteToDiskOverride = (_, __) => wasRefreshed = true;
            InstallationInfo.ReadFromDiskOverride = (_) => new InstallationInfo(InstallationGuid, now);
            InstallationInfo info = InstallationInfo.LoadFromPath(_testFolder, now);

            Assert.IsFalse(wasRefreshed);
            Assert.AreEqual(now, info.LastReset);
            Assert.AreEqual(InstallationGuid, info.InstallationGuid);
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadFromPath_FileIsFromDifferentMonth_FileIsRefreshedWithProvidedTime()
        {
            DateTime now = February2015;
            bool wasRefreshed = false;
            InstallationInfo.WriteToDiskOverride = (_, __) => wasRefreshed = true;
            InstallationInfo.ReadFromDiskOverride = (_) => new InstallationInfo(InstallationGuid, January2015);

            InstallationInfo info = InstallationInfo.LoadFromPath(_testFolder, now);

            Assert.IsTrue(wasRefreshed);
            Assert.AreEqual(now, info.LastReset);
            Assert.AreNotEqual(InstallationGuid, info.InstallationGuid);
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadFromPath_FileIsFromDifferentYear_FileIsRefreshedWithProvidedTime()
        {
            DateTime now = January2016;
            bool wasRefreshed = false;
            InstallationInfo.WriteToDiskOverride = (_, __) => wasRefreshed = true;
            InstallationInfo.ReadFromDiskOverride = (_) => new InstallationInfo(InstallationGuid, January2015);

            InstallationInfo info = InstallationInfo.LoadFromPath(_testFolder, now);

            Assert.IsTrue(wasRefreshed);
            Assert.AreEqual(now, info.LastReset);
            Assert.AreNotEqual(InstallationGuid, info.InstallationGuid);
        }

        [TestMethod]
        [Timeout(1000)]
        public void LoadFromPath_FileIsFromDifferentMonth_UnableToWrite_ReturnsRefreshedDataEachTime()
        {
            // Intentionally don't set WriteToDiskOverride so that we try to write to disk.
            // Since we haven't created a folder for _testFolder, the attempt to write to
            // disk will fail. This simulates the case where we're unable to update the
            // configuration file for whatever reason (file is locked, disk is full, etc.)
            DateTime now = February2015;
            InstallationInfo.ReadFromDiskOverride = (_) => new InstallationInfo(InstallationGuid, January2015);

            InstallationInfo info1 = InstallationInfo.LoadFromPath(_testFolder, now);
            InstallationInfo info2 = InstallationInfo.LoadFromPath(_testFolder, now);

            Assert.AreEqual(now, info1.LastReset);
            Assert.AreEqual(now, info2.LastReset);
            Assert.AreNotEqual(InstallationGuid, info1.InstallationGuid);
            Assert.AreNotEqual(InstallationGuid, info2.InstallationGuid);
            Assert.AreNotEqual(info1.InstallationGuid, info2.InstallationGuid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void LoadFromPath_RoundTripThroughDisk_FileIsNotRefreshed()
        {
            // Note: Not strictly a unit test since it pushes data through the disk
            DateTime now = DateTime.UtcNow;
            Directory.CreateDirectory(_testFolder);

            InstallationInfo info1 = InstallationInfo.LoadFromPath(_testFolder, now);
            InstallationInfo info2 = InstallationInfo.LoadFromPath(_testFolder, now);

            Assert.AreEqual(now, info1.LastReset);
            Assert.AreEqual(info1.LastReset, info2.LastReset);

            Assert.AreNotEqual(InstallationGuid, info1.InstallationGuid);
            Assert.AreEqual(info1.InstallationGuid, info2.InstallationGuid);
        }
    }
}
