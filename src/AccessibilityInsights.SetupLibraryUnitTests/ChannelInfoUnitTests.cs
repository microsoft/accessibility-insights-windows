// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace SetupLibraryUnitTests
{
    [TestClass]
    public class ChannelInfoUnitTests
    {
        private class TestExceptionReporter : IExceptionReporter
        {
            public Exception ActualException { get; private set; }
            public void ReportException(Exception e)
            {
                Assert.IsNotNull(e);
                Assert.IsNull(ActualException);
                ActualException = e;
            }
        }

        private const string DefaultChannel = "default";
        private const string Path1 = "www.somehost.com/somepath/1";
        private const string Path2 = "www.somehost.com/somepath/2";
        private static readonly Version VersionLow = new Version(1, 1);
        private static readonly Version VersionHigh = new Version(1, 2);
        private static readonly Version Version1000 = new Version(1, 1, 1000);
        private static readonly Version Version1234 = new Version(1, 1, 1234);
        private static readonly Version Version1300 = new Version(1, 1, 1300);
        private static readonly Version Version1330 = new Version(1, 1, 1330);

        private const string TestCaseContents =
@"{
  ""default"": {
    ""current_version"": ""1.1.1234"",
    ""minimum_version"": ""1.1.1000"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1234/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1234/releasenotes.html""
  },
  ""Insider"": {
    ""current_version"": ""1.1.1330"",
    ""minimum_version"": ""1.1.1300"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1330/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1330/releasenotes.html""
    },
  ""invalid"": {
    ""minimum_version"": ""1.1.1300"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1330/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1330/releasenotes.html""
    }
}";

        private string ExpectedInstaller(Version version)
        {
            return "https://somehost.com/somepath/" + version.ToString() + "/installer.msi";
        }

        private string ExpectedReleaseNotes(Version version)
        {
            return "https://somehost.com/somepath/" + version.ToString() + "/releasenotes.html";
        }

        private static Stream PopulateStream(string contents)
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(contents);
            writer.Flush();
            return stream;
        }

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

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsEmpty_ThrowsNullReferenceException()
        {
            using (Stream stream = PopulateStream(string.Empty))
            {
                ChannelInfo.GetChannelFromStream(DefaultChannel, stream);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamDoesNotContainChannel_ThrowsInvalidDataException()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfo.GetChannelFromStream("does not exist", stream);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetChannelFromStream_DefaultChannel_ReturnsCorrectData()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfo channelInfo = ChannelInfo.GetChannelFromStream(DefaultChannel, stream);

                Assert.IsNotNull(channelInfo);
                Assert.IsTrue(channelInfo.IsValid);
                Assert.AreEqual(Version1234, channelInfo.CurrentVersion);
                Assert.AreEqual(Version1000, channelInfo.MinimumVersion);
                Assert.AreEqual(ExpectedInstaller(Version1234), channelInfo.InstallAsset);
                Assert.AreEqual(ExpectedReleaseNotes(Version1234), channelInfo.ReleaseNotesAsset);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetChannelFromStream_InsiderChannel_ReturnsCorrectData()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfo channelInfo = ChannelInfo.GetChannelFromStream("Insider", stream);

                Assert.IsNotNull(channelInfo);
                Assert.IsTrue(channelInfo.IsValid);
                Assert.AreEqual(Version1330, channelInfo.CurrentVersion);
                Assert.AreEqual(Version1300, channelInfo.MinimumVersion);
                Assert.AreEqual(ExpectedInstaller(Version1330), channelInfo.InstallAsset);
                Assert.AreEqual(ExpectedReleaseNotes(Version1330), channelInfo.ReleaseNotesAsset);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsInvalid_ThrowsInvalidDataException()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfo.GetChannelFromStream("invalid", stream);
            }
        }
    }
}
