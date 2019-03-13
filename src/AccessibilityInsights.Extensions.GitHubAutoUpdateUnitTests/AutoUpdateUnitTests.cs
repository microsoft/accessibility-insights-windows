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
        private const string DefaultReleaseChannel = "default";

        private static readonly IGitHubWrapper InertGitHubWrapper = new Mock<IGitHubWrapper>(MockBehavior.Strict).Object;
        private static readonly AutoUpdate.ChannelInfoProvider InertChannelInfoProvider =
            BuildChannelInfoProvider().Object;

        private static readonly ChannelInfo NoUpgradeChannelInfo = new ChannelInfo
        {
            CurrentVersion = new Version(TestInstalledVersion),
            MinimumVersion = new Version(TestInstalledVersion),
            InstallAsset = GetInstallerPath(TestInstalledVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(TestInstalledVersion)
        };

        private static readonly ChannelInfo OptionalUpgradeChannelInfo = new ChannelInfo
        {
            CurrentVersion = new Version(UplevelVersion),
            MinimumVersion = new Version(TestInstalledVersion),
            InstallAsset = GetInstallerPath(UplevelVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(UplevelVersion)
        };

        private static readonly ChannelInfo RequiredUpgradeChannelInfo = new ChannelInfo
        {
            CurrentVersion = new Version(UplevelVersion),
            MinimumVersion = new Version(UplevelVersion),
            InstallAsset = GetInstallerPath(UplevelVersion),
            ReleaseNotesAsset = GetReleaseNotesPath(UplevelVersion)
        };

        private static string GetInstallerPath(string version)
        {
            return "https://www.mywebsite.com/" + version + "/installer.msi";
        }

        private static string GetReleaseNotesPath(string version)
        {
            return "https://www.mywebsite.com/" + version + "/release_notes.md";
        }

        private static Mock<AutoUpdate.ChannelInfoProvider> BuildChannelInfoProvider(string expectedChannel = null,
            ChannelInfo channelInfo = null)
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock =
                new Mock<AutoUpdate.ChannelInfoProvider>(MockBehavior.Strict);

            if (expectedChannel != null)
            {
                providerMock.Setup(x => x(InertGitHubWrapper, expectedChannel, out channelInfo)).Returns(channelInfo != null);
            }

            return providerMock;
        }

        [TestMethod]
        [Timeout(2000)]
        public void ReleaseChannel_DefaultsToExpectedValue()
        {
            IAutoUpdate update = new AutoUpdate();
            Assert.AreEqual(DefaultReleaseChannel, update.ReleaseChannel);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_UnableToGetInstalledVersion_ReturnsUnknown_FieldsAreNull()
        {
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => "blah", InertChannelInfoProvider);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.IsNull(update.InstalledVersion);
            Assert.IsNull(update.CurrentChannelVersion);
            Assert.IsNull(update.MinimumChannelVersion);
            Assert.IsNull(update.ReleaseNotesUri);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_UnableToGetConfig_ReturnsUnknown_FieldsAreNull()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.CurrentChannelVersion);
            Assert.IsNull(update.MinimumChannelVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigIsInvalid_ReturnsUnknown_FieldsAreNull()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel,
                new ChannelInfo
                {
                    CurrentVersion = new Version(UplevelVersion)
                });  // Config is only partially set, so it's invalid
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.CurrentChannelVersion);
            Assert.IsNull(update.MinimumChannelVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsNoUpgrade_ReturnsCurrent_ReturnsNoUpgrade()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.Current, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(NoUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(NoUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(NoUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsOptionalUpgrade_ReturnsOptionalUpgrade()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, OptionalUpgradeChannelInfo);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.OptionalUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(OptionalUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(OptionalUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(OptionalUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsRequiredUpgrade_ReturnsRequiredUpgrade()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, RequiredUpgradeChannelInfo);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(AutoUpdateOption.RequiredUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(RequiredUpgradeChannelInfo.CurrentVersion, update.CurrentChannelVersion);
            Assert.AreEqual(RequiredUpgradeChannelInfo.MinimumVersion, update.MinimumChannelVersion);
            Assert.AreEqual(RequiredUpgradeChannelInfo.ReleaseNotesAsset, update.ReleaseNotesUri.ToString());
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateAsync_ConfigNotAvailable_ReturnsNoUpgradeAvailable()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(UpdateResult.NoUpdateAvailable, update.UpdateAsync().Result);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateAsync_ConfigShowsNoUpgrade_ReturnsNoUpgradeAvailable()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
            Assert.AreEqual(UpdateResult.NoUpdateAvailable, update.UpdateAsync().Result);
            providerMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_InitializationTimeIsNotZero()
        {
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);
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
            Mock<AutoUpdate.ChannelInfoProvider> providerMock = BuildChannelInfoProvider(DefaultReleaseChannel, NoUpgradeChannelInfo);
            AutoUpdate update = new AutoUpdate(InertGitHubWrapper, () => TestInstalledVersion, providerMock.Object);

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
