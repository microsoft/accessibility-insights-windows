// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace AccessibilityInsights.SetupLibraryUnitTests
{
    [TestClass]
    public class ChannelInfoUtilitiesUnitTests
    {
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
        [ExpectedException(typeof(ArgumentNullException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsNull_ThrowsArgumentNullException()
        {
            ChannelInfoUtilities.GetChannelFromStream(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsEmpty_ThrowsNullReferenceException()
        {
            using (Stream stream = PopulateStream(string.Empty))
            {
                ChannelInfoUtilities.GetChannelFromStream(stream);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamDoesNotContainChannel_ThrowsInvalidDataException()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfoUtilities.GetChannelFromStream(stream, "key does not exist");
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetChannelFromStream_DefaultChannel_ReturnsCorrectData()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfo channelInfo = ChannelInfoUtilities.GetChannelFromStream(stream);

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
                ChannelInfo channelInfo = ChannelInfoUtilities.GetChannelFromStream(stream, "Insider");

                Assert.IsNotNull(channelInfo);
                Assert.IsTrue(channelInfo.IsValid);
                Assert.AreEqual(Version1330, channelInfo.CurrentVersion);
                Assert.AreEqual(Version1300, channelInfo.MinimumVersion);
                Assert.AreEqual(ExpectedInstaller(Version1330), channelInfo.InstallAsset);
                Assert.AreEqual(ExpectedReleaseNotes(Version1330), channelInfo.ReleaseNotesAsset);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsValid_StreamIsNotClosed()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                long originalLength = stream.Length;
                ChannelInfoUtilities.GetChannelFromStream(stream);
                Assert.AreEqual(originalLength, stream.Length);
                Assert.AreNotEqual(0, originalLength);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsInvalid_ThrowsInvalidDataException()
        {
            using (Stream stream = PopulateStream(TestCaseContents))
            {
                ChannelInfoUtilities.GetChannelFromStream(stream, "invalid");
            }
        }
    }
}
