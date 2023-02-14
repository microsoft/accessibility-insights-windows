// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;

namespace AccessibilityInsights.SetupLibraryUnitTests
{
    [TestClass]
    public class ChannelInfoUtilitiesUnitTests
    {
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
        [ExpectedException(typeof(InvalidDataException))]
        [Timeout(2000)]
        public void GetChannelFromStream_StreamIsEmpty_ThrowsInvalidDataException()
        {
            using (Stream stream = PopulateStream(string.Empty))
            {
                ChannelInfoUtilities.GetChannelFromStream(stream);
            }
        }

        private static void AssertValidUri(string url)
        {
            Uri uri = new Uri(url);
            Assert.IsFalse(uri.IsUnc);
            Assert.IsFalse(uri.IsFile);
            Assert.IsTrue(uri.IsAbsoluteUri);
        }

        [TestMethod]
        [Timeout(5000)]
        public void TryGetChannelInfo_CanaryChannel_ReturnsReasonableData()
        {
            List<Exception> exceptions = new List<Exception>();
            Mock<IExceptionReporter> reporterMock = new Mock<IExceptionReporter>(MockBehavior.Strict);
            reporterMock.Setup(x => x.ReportException(It.IsAny<Exception>()))
                .Callback<Exception>((e) => exceptions.Add(e));

            bool succeeded = ChannelInfoUtilities.TryGetChannelInfo(ReleaseChannel.Canary,
                out EnrichedChannelInfo info, null, reporterMock.Object);

            if (succeeded)
            {
                const int SHA_512_Length = 128;
                Assert.AreEqual(SHA_512_Length, info.MsiSha512.Length);
                Assert.AreNotEqual(0, info.MsiSizeInBytes);
                Assert.AreEqual(1, info.CurrentVersion.Major);
                Assert.AreEqual(1, info.CurrentVersion.Minor);
                Assert.AreEqual(1, info.ProductionMinimumVersion.Major);
                Assert.AreEqual(1, info.ProductionMinimumVersion.Minor);
                Assert.IsTrue(info.MinimumVersion == info.CurrentVersion);
                Assert.IsTrue(info.CurrentVersion >= info.ProductionMinimumVersion, "Canary should never be behind Production");
                Assert.IsTrue(info.IsValid);
                AssertValidUri(info.InstallAsset);
                AssertValidUri(info.ReleaseNotesAsset);
                Assert.AreEqual(0, exceptions.Count);
            }
            else
            {
                Assert.IsNull(info);
                Assert.AreNotEqual(0, exceptions.Count);
                Assert.Inconclusive(string.Join(Environment.NewLine, exceptions));  
            }
        }
    }
}
