// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.SetupLibrary;
using AccessibilityInsights.VersionSwitcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace ManifestTests
{
    [TestClass]
    public class IntegrationTests
    {
        private static readonly string RawManifestFilePath = Path.GetFullPath(Path.Combine(@"..\..\..\..", @"bld\ReleaseInfo.json"));
        private static readonly string SignedManifestFilePath = Path.GetFullPath(Path.Combine(@"..\..\..\..", @"Manifest\bin\Release\net48\AccessibilityInsights.Manifest.dll"));
        private static readonly string MsiFilePath = Path.GetFullPath(Path.Combine(@"..\..\..\..", @"MSI\bin\Release\AccessibilityInsights.msi"));
        private static readonly ChannelInfo rawInfo = FileHelpers.LoadDataFromJSON<ChannelInfo>(RawManifestFilePath);

        [TestMethod]
        public void MsiFileEists()
        {
            Assert.IsTrue(File.Exists(MsiFilePath), $"Can't find MSI file at {MsiFilePath}");
        }

        [TestMethod]
        public void RawManifestFileExists()
        {
            Assert.IsTrue(File.Exists(RawManifestFilePath), $"Can't find raw manifest file at {RawManifestFilePath}");
        }

        [TestMethod]
        public void EmbeddedManifestFileExists()
        {
            Assert.IsTrue(File.Exists(SignedManifestFilePath), $"Can't find signed manifest file at {SignedManifestFilePath}");
        }

        [TestMethod]
        public void MsiFileSizeIsCorrectInRawManifest()
        {
            Assert.AreEqual(new FileInfo(MsiFilePath).Length, rawInfo.MsiSizeInBytes);
        }

        [TestMethod]
        public void MsiSha512IsCorrectInRawManifest()
        {
            Assert.AreEqual(InstallationEngine.ComputeSha512(MsiFilePath), rawInfo.MsiSha512);
        }

        [TestMethod]
        public void MinimumVersionIsNullInRawManifest()
        {
            Assert.IsNull(rawInfo.MinimumVersion);
        }

        [TestMethod]
        public void EmbeddedManifestMatchesRawManifest()
        {
            using (Stream stream = new FileStream(SignedManifestFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                stream.Position = 0;

#if ENABLE_SIGNING
                Func<Stream, bool> signingOverride = null;          // Enforces signing requirement
#else
                Func<Stream, bool> signingOverride = (_) => true;   // Bypasses signing requirement
#endif
                ChannelInfo signedInfo = ChannelInfoUtilities.GetChannelInfoFromSignedManifest(stream, signingOverride);

                Assert.IsNotNull(signedInfo);
                Assert.AreEqual(rawInfo.MsiSizeInBytes, signedInfo.MsiSizeInBytes);
                Assert.AreEqual(rawInfo.MsiSha512, signedInfo.MsiSha512);
                Assert.AreEqual(rawInfo.InstallAsset, signedInfo.InstallAsset);
                Assert.AreEqual(rawInfo.ReleaseNotesAsset, signedInfo.ReleaseNotesAsset);
                Assert.AreEqual(rawInfo.CurrentVersion, signedInfo.CurrentVersion);
                Assert.AreEqual(rawInfo.ProductionMinimumVersion, signedInfo.ProductionMinimumVersion);
                Assert.AreEqual(rawInfo.MinimumVersion, signedInfo.MinimumVersion);
            }
        }
    }
}
