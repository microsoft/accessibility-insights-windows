// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SharedUx.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
#if FAKES_SUPPORTED
using Microsoft.QualityTools.Testing.Fakes;
using System.Fakes;
using System.IO.Fakes;
#endif

namespace AccessibilityInsights.SharedUxTests.Settings
{
    [TestClass]
    public class InstallationInfoTests
    {
#if FAKES_SUPPORTED
        /// <summary>
        /// Checks whether the guid resets when the month changes
        /// </summary>
        [TestMethod]
        public void TestInstallationInfoGuidResets()
        {
            using (ShimsContext.Create())
            {
                // We shim the filesystem related methods. currentJson represents the current 
                // state of what would be saved on disk
                string fakePath = "fake\\";
                var currentJson = "";
                ShimFile.ReadAllTextStringEncoding = (_, __) => { return currentJson; };
                ShimFile.ExistsString = (_) => !string.IsNullOrEmpty(currentJson);
                ShimFile.WriteAllTextStringStringEncoding = (_, json, __) => { currentJson = json; };

                ShimDateTime.UtcNowGet = () => new DateTime(2015, 1, 1);
                InstallationInfo janInfo1 = InstallationInfo.LoadFromPath(fakePath);

                // The guid shouldn't reset if the month is the same
                InstallationInfo info2 = InstallationInfo.LoadFromPath(fakePath);
                Assert.AreEqual(janInfo1.InstallationGuid, info2.InstallationGuid);

                // A month has elapsed so the guid should reset
                DateTime februaryYearOne = new DateTime(2015, 2, 1);
                ShimDateTime.UtcNowGet = () => februaryYearOne;
                InstallationInfo febInfo = InstallationInfo.LoadFromPath(fakePath);
                Assert.AreNotEqual(janInfo1.InstallationGuid, febInfo.InstallationGuid);
                Assert.AreEqual(febInfo.LastReset, februaryYearOne);

                // although the old month (2) > current month(1), a year has elapsed
                //  so the guid should reset
                DateTime januaryYearTwo = new DateTime(2016, 1, 1);
                ShimDateTime.UtcNowGet = () => januaryYearTwo;
                InstallationInfo janInfo2 = InstallationInfo.LoadFromPath(fakePath);
                Assert.AreNotEqual(febInfo.InstallationGuid, janInfo2.InstallationGuid);
                Assert.AreEqual(janInfo2.LastReset, januaryYearTwo);
            }
        }
#endif
    }
}
