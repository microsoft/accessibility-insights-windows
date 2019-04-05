// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.SetupLibraryUnitTests
{
    [TestClass]
    public class ChannelInfoUnitTests
    {
        private const string Path1 = "www.somehost.com/somepath/1";
        private const string Path2 = "www.somehost.com/somepath/2";
        private static readonly Version VersionLow = new Version(1, 1);
        private static readonly Version VersionHigh = new Version(1, 2);

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_EmptyObject_IsNotValid()
        {
            ChannelInfo channelInfo = new ChannelInfo();
            Assert.IsFalse(channelInfo.IsValid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_TypicalData_IsTrue()
        {
            ChannelInfo channelInfo = new ChannelInfo
            {
                CurrentVersion = VersionHigh,
                MinimumVersion = VersionLow,
                InstallAsset = Path1,
                ReleaseNotesAsset = Path2,
            };
            Assert.IsTrue(channelInfo.IsValid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_MissingCurrentVersion_IsFalse()
        {
            ChannelInfo channelInfo = new ChannelInfo
            {
                MinimumVersion = VersionLow,
                InstallAsset = Path1,
                ReleaseNotesAsset = Path2,
            };
            Assert.IsFalse(channelInfo.IsValid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_MissingMinimumVersion_IsFalse()
        {
            ChannelInfo channelInfo = new ChannelInfo
            {
                CurrentVersion = VersionHigh,
                InstallAsset = Path1,
                ReleaseNotesAsset = Path2,
            };
            Assert.IsFalse(channelInfo.IsValid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_MissingInstallAsset_IsFalse()
        {
            ChannelInfo channelInfo = new ChannelInfo
            {
                CurrentVersion = VersionHigh,
                MinimumVersion = VersionLow,
                ReleaseNotesAsset = Path2,
            };
            Assert.IsFalse(channelInfo.IsValid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_MissingReleaseNotesAsset_IsFalse()
        {
            ChannelInfo channelInfo = new ChannelInfo
            {
                CurrentVersion = VersionHigh,
                MinimumVersion = VersionLow,
                InstallAsset = Path1,
            };
            Assert.IsFalse(channelInfo.IsValid);
        }

        [TestMethod]
        [Timeout(2000)]
        public void IsValid_MinimumVersionExceedsCurrentVersion_IsFalse()
        {
            ChannelInfo channelInfo = new ChannelInfo
            {
                CurrentVersion = VersionLow,
                MinimumVersion = VersionHigh,
                InstallAsset = Path1,
                ReleaseNotesAsset = Path2
            };
            Assert.IsFalse(channelInfo.IsValid);
        }
    }
}
