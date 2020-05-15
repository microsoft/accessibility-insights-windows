// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.Extensions.TelemetryTests
{
    [TestClass]
    public class OSHelpersUnitTests
    {
        const string CurrentVersion = "CurrentVersion";
        const string CurrentBuild = "CurrentBuild";

        private Tuple<string, string, string> MakeVersionTuple(string valueName, string defaultValue)
        {
            const string VersionKey = @"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows NT\CurrentVersion";
            return new Tuple<string, string, string>(VersionKey, valueName, defaultValue);
        }

        [TestMethod]
        public void GetVersion_CurrentVersionDoesNotExist_ReturnsUnknown()
        {
            Tuple<string, string, string>[] expectedCalls =
            {
                MakeVersionTuple(CurrentVersion, ""),
            };
            TestRegistry registry = new TestRegistry(expectedCalls);

            Assert.AreEqual("unknown", OSHelpers.GetVersion(registry.GetStringValue));
            Assert.AreEqual(1, registry.QueriesMade);
        }

        [TestMethod]
        public void GetVersion_CurrentBuildDoesNotExist_ReturnsCurrentVersion()
        {
            Tuple<string, string, string>[] expectedCalls =
            {
                MakeVersionTuple(CurrentVersion, "99.88"),
                MakeVersionTuple(CurrentBuild, ""),
            };
            TestRegistry registry = new TestRegistry(expectedCalls);

            Assert.AreEqual("99.88", OSHelpers.GetVersion(registry.GetStringValue));
            Assert.AreEqual(2, registry.QueriesMade);
        }

        [TestMethod]
        public void GetVersion_AllDataExists_ReturnsCurrentVersionDotCurrentBuild()
        {
            Tuple<string, string, string>[] expectedCalls =
            {
                MakeVersionTuple(CurrentVersion, "99.88"),
                MakeVersionTuple(CurrentBuild, "77"),
            };
            TestRegistry registry = new TestRegistry(expectedCalls);

            Assert.AreEqual("99.88.77", OSHelpers.GetVersion(registry.GetStringValue));
            Assert.AreEqual(2, registry.QueriesMade);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetVersion_LiveData_ReturnsDataInCorrectFormat()
        {
            string version = OSHelpers.GetVersion();
            string[] pieces = version.Split('.');
            Assert.AreEqual(3, pieces.Length);
            int majorVersion = int.Parse(pieces[0]);
            int minorVersion = int.Parse(pieces[1]);
            int build = int.Parse(pieces[2]);

            // Validation here assumes a range of Windows OS versions.
            // Our lower bound is 6.1.7601  (Win 7 SP1)
            // Our upper bound is 6.3.99999
            // At current Win10 cadence, that will last until about 2040
            Assert.AreEqual(6, majorVersion);
            Assert.IsTrue(minorVersion >= 1 && minorVersion <= 3, "minorVersion is " + minorVersion);
            Assert.IsTrue(build >= 7601 && build <= 99999, "build is " + build);
        }
    }
}
