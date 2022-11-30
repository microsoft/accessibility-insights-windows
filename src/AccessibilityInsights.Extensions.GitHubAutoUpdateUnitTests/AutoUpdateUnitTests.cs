// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.GitHubAutoUpdate;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;

namespace Extensions.GitHubAutoUpdateUnitTests
{
    [TestClass]
    public class AutoUpdateUnitTests
    {
        private const string TestInstalledVersion = "1.1.1234";
        private const string UplevelVersion = "1.1.1260";
        private const string DownlevelVersion = "1.1.1200";
        private const ReleaseChannel DefaultReleaseChannel = ReleaseChannel.Production;
        private const int TestManifestFileSize = 123456;
        private const string TestRequestUrl = "https://request-host/request-path";
        private const string TestResponseUrl = "https://response-host/response-path";

        private static readonly IChannelInfoProvider InertChannelInfoProvider =
            BuildChannelInfoProvider().Object;

        private static readonly StreamMetadata TestManifestStreamData = new StreamMetadata(
            new Uri(TestRequestUrl), new Uri(TestResponseUrl), TestManifestFileSize);

        private static readonly EnrichedChannelInfo NoUpgradeChannelInfo = new EnrichedChannelInfo
        {
            CurrentVersion = new Version(TestInstalledVersion),
            MinimumVersion = new Version(TestInstalledVersion),
            InstallAsset = GetInstallerPath(TestInstalledVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(TestInstalledVersion),
            Metadata = TestManifestStreamData,
        };

        private static readonly EnrichedChannelInfo OptionalUpgradeChannelInfo = new EnrichedChannelInfo
        {
            CurrentVersion = new Version(UplevelVersion),
            MinimumVersion = new Version(TestInstalledVersion),
            InstallAsset = GetInstallerPath(UplevelVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(UplevelVersion),
            Metadata = TestManifestStreamData,
        };

        private static readonly EnrichedChannelInfo RequiredUpgradeChannelInfo = new EnrichedChannelInfo
        {
            CurrentVersion = new Version(UplevelVersion),
            MinimumVersion = new Version(UplevelVersion),
            InstallAsset = GetInstallerPath(UplevelVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(UplevelVersion),
            Metadata = TestManifestStreamData,
        };

        private static readonly EnrichedChannelInfo PreReleaseChannelInfo = new EnrichedChannelInfo
        {
            CurrentVersion = new Version(DownlevelVersion),
            MinimumVersion = new Version(DownlevelVersion),
            InstallAsset = GetInstallerPath(DownlevelVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(DownlevelVersion),
            Metadata = TestManifestStreamData,
        };

        private static string GetInstallerPath(string version)
        {
            return "https://www.mywebsite.com/" + version + "/installer.msi";
        }

        private static string GetReleaseNotesPath(string version)
        {
            return "https://www.mywebsite.com/" + version + "/release_notes.md";
        }

        private static Mock<IChannelInfoProvider> BuildChannelInfoProvider(ReleaseChannel? expectedChannel = null,
            EnrichedChannelInfo enrichedChannelInfo = null)
        {
            Mock<IChannelInfoProvider> providerMock =
                new Mock<IChannelInfoProvider>(MockBehavior.Strict);

            if (expectedChannel.HasValue)
            {
                providerMock.Setup(x => x.TryGetChannelInfo(expectedChannel.Value, out enrichedChannelInfo)).Returns(enrichedChannelInfo != null);
            }

            return providerMock;
        }

        // Wrap the constructor to give us a single place for adding dependency injection parameters
        private static AutoUpdate BuildAutoUpdate(ReleaseChannel releaseChannel = ReleaseChannel.Production, string testInstalledVersion = null, IChannelInfoProvider channelProvider = null)
        {
            return new AutoUpdate(() => releaseChannel, () => testInstalledVersion ?? TestInstalledVersion, channelProvider ?? InertChannelInfoProvider);
        }

        private static void AssertSuccessfulManifestInformation(IAutoUpdate update)
        {
            Assert.AreEqual(TestRequestUrl, update.ManifestRequestUri.ToString());
            Assert.AreEqual(TestResponseUrl, update.ManifestResponseUri.ToString());
            Assert.AreEqual(TestManifestFileSize, update.ManifestSizeInBytes);
        }

        private static void AssertDefaultManifestInformation(IAutoUpdate update)
        {
            Assert.IsNull(update.ManifestRequestUri);
            Assert.IsNull(update.ManifestResponseUri);
            Assert.AreEqual(0, update.ManifestSizeInBytes);
        }

        [TestMethod]
        [Timeout(2000)]
        public void ReleaseChannel_DefaultsToExpectedValue()
        {
            IAutoUpdate update = BuildAutoUpdate();
            Assert.AreEqual(DefaultReleaseChannel.ToString(), update.ReleaseChannel);
            AssertDefaultManifestInformation(update);
        }

        [TestMethod]
        [Timeout(2000)]
        public void ReleaseChannel_CanBeOverriddenInConstructor()
        {
            const ReleaseChannel testReleaseChannel = ReleaseChannel.Insider;
            IAutoUpdate update = BuildAutoUpdate(releaseChannel: testReleaseChannel);
            Assert.AreEqual(testReleaseChannel.ToString(), update.ReleaseChannel);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_UnableToGetInstalledVersion_ReturnsUnknown_FieldsAreNull()
        {
            AutoUpdate update = BuildAutoUpdate(testInstalledVersion: "blah");
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.IsNull(update.InstalledVersion);
            Assert.IsNull(update.CurrentChannelVersion);
            Assert.IsNull(update.MinimumChannelVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            AssertDefaultManifestInformation(update);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_UnableToGetConfig_ReturnsUnknown_FieldsAreNull()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.CurrentChannelVersion);
            Assert.IsNull(update.MinimumChannelVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            AssertDefaultManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigIsInvalid_ReturnsUnknown_FieldsAreNull()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel,
                new EnrichedChannelInfo
                {
                    CurrentVersion = new Version(UplevelVersion)
                });  // Config is only partially set, so it's invalid
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.CurrentChannelVersion);
            Assert.IsNull(update.MinimumChannelVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            AssertDefaultManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsNoUpgrade_ReturnsCurrent_ReturnsNoUpgrade()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.Current, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(NoUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(NoUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(NoUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            AssertSuccessfulManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsOptionalUpgrade_ReturnsOptionalUpgrade()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, OptionalUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.OptionalUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(OptionalUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(OptionalUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(OptionalUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            AssertSuccessfulManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsRequiredUpgrade_ReturnsRequiredUpgrade()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, RequiredUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.RequiredUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(RequiredUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(RequiredUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(RequiredUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            AssertSuccessfulManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsPrereleaseBuild_ReturnsNewerThanCurrent()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, PreReleaseChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.NewerThanCurrent, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(PreReleaseChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(PreReleaseChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(PreReleaseChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            AssertSuccessfulManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ReleaseChannelIsOverridden_UsesCorrectChannel()
        {
            const ReleaseChannel testReleaseChannel = ReleaseChannel.Canary;
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(testReleaseChannel, OptionalUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(releaseChannel: testReleaseChannel, channelProvider: providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.OptionalUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(OptionalUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(OptionalUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(OptionalUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            AssertSuccessfulManifestInformation(update);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateAsync_ConfigNotAvailable_ReturnsNoUpgradeAvailable()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(UpdateResult.NoUpdateAvailable, update.UpdateAsync().Result);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateAsync_ConfigShowsNoUpgrade_ReturnsNoUpgradeAvailable()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            Assert.AreEqual(UpdateResult.NoUpdateAvailable, update.UpdateAsync().Result);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_InitializationTimeIsNotZero()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);
            // We have to wait for initialization to complete
            update.UpdateOptionAsync.Wait();
            Assert.IsTrue(update.GetInitializationTime().HasValue);
            Assert.AreNotEqual(TimeSpan.Zero, update.GetInitializationTime().Value);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateAsync_UpdateTimeIsNotZero()
        {
            Mock<IChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = BuildAutoUpdate(channelProvider: providerMock.Object);

            // ensure that the UpdateTime is 0 before calling UpdateAsync
            TimeSpan? updateTime = update.GetUpdateTime();
            Assert.IsTrue(updateTime.HasValue);
            Assert.AreEqual(TimeSpan.Zero, updateTime.Value);

            update.UpdateAsync().Wait();

            // ensure that the UpdateTime is not 0 after calling UpdateAsync
            updateTime = update.GetUpdateTime();
            Assert.IsTrue(updateTime.HasValue);
            Assert.AreNotEqual(TimeSpan.Zero, updateTime.Value);
            providerMock.VerifyAll();
        }
    }
}
