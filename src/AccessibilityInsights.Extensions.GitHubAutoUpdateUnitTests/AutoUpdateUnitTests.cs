// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.GitHubAutoUpdate;
using AccessibilityInsights.Extensions.Interfaces.Upgrades;
using AccessibilityInsights.SetupLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Threading;

namespace Extensions.GitHubAutoUpdateUnitTests
{
    [TestClass]
    public class AutoUpdateUnitTests
    {
        private const string TestInstalledVersion = "1.1.1234";
        private const string InsiderReleaseChannel = "insider";

        private static readonly IGitHubWrapper MinimalGitHubWrapper = BuildGitHubWrapper().Object;

        private const string UptionalUpgradeDataForDefaultRequiredUpdateForInsider =
@"{
  ""insider"": {
    ""current_version"": ""1.1.1330"",
    ""minimum_version"": ""1.1.1300"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1330/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1330/releasenotes.html""
    },
  ""default"": {
    ""current_version"": ""1.1.1250"",
    ""minimum_version"": ""1.1.1000"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1250/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1250/releasenotes.html""
  }
}";

        private const string RequiredUpgradeDataForDefaultAndInsider =
@"{
  ""insider"": {
    ""current_version"": ""1.1.1330"",
    ""minimum_version"": ""1.1.1000"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1300/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1300/releasenotes.html""
    },
  ""default"": {
    ""current_version"": ""1.1.1250"",
    ""minimum_version"": ""1.1.1240"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1250/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1250/releasenotes.html""
  }
}";

        private const string CurrentUpgradeDataForDefaultOptionalUpgradeForInsider =
@"{
  ""insider"": {
    ""current_version"": ""1.1.1330"",
    ""minimum_version"": ""1.1.1000"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1300/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1300/releasenotes.html""
    },
  ""default"": {
    ""current_version"": ""1.1.1234"",
    ""minimum_version"": ""1.1.1000"",
    ""installer_url"": ""https://somehost.com/somepath/1.1.1234/installer.msi"",
    ""release_notes_url"": ""https://somehost.com/somepath/1.1.1234/releasenotes.html""
  }
}";

        private static Mock<IGitHubWrapper> BuildGitHubWrapper(string channelInfoString = null)
        {
            Mock<IGitHubWrapper> wrapperMock = new Mock<IGitHubWrapper>(MockBehavior.Strict);

            if (channelInfoString == null)
            {
                wrapperMock.Setup(x => x.TryGetChannelInfo(It.IsAny<Stream>()))
                    .Returns(false);
            }
            else
            {
                wrapperMock.Setup(x => x.TryGetChannelInfo(It.IsAny<Stream>()))
                    .Callback<Stream>((stream) =>
                    {
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(channelInfoString);
                        writer.Flush();
                    })
                    .Returns(true);
            }

            return wrapperMock;
        }

        [TestMethod]
        [Timeout(2000)]
        public void ReleaseChannel_DefaultsToExpectedValue()
        {
            IAutoUpdate update = new AutoUpdate();
            Assert.AreEqual("default", update.ReleaseChannel);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_UnableToGetInstalledVersion_ReturnsUnknown_FieldsAreNull()
        {
            AutoUpdate update = new AutoUpdate(MinimalGitHubWrapper, () => "blah");
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.IsNull(update.InstalledVersion);
            Assert.IsNull(update.LatestVersion);
            Assert.IsNull(update.MinimumVersion);
            Assert.IsNull(update.ReleaseNotesUri);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_UnableToGetConfig_ReturnsUnknown_FieldsAreNull()
        {
            AutoUpdate update = new AutoUpdate(MinimalGitHubWrapper, () => TestInstalledVersion);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.LatestVersion);
            Assert.IsNull(update.MinimumVersion);
            Assert.IsNull(update.ReleaseNotesUri);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigIsInvalid_ReturnsUnknown_FieldsAreNull()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: "blah");
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.LatestVersion);
            Assert.IsNull(update.MinimumVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            githubMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ReleaseChannelIsNotFound_ReturnsUnknown_FieldsAreNull()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: CurrentUpgradeDataForDefaultOptionalUpgradeForInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            update.ReleaseChannel = "blah";
            Assert.AreEqual(AutoUpdateOption.Unknown, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.IsNull(update.LatestVersion);
            Assert.IsNull(update.MinimumVersion);
            Assert.IsNull(update.ReleaseNotesUri);
            githubMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_SearchWithReleaseChannel_FindsCorrectData()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: UptionalUpgradeDataForDefaultRequiredUpdateForInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            update.ReleaseChannel = InsiderReleaseChannel;
            Assert.AreEqual(AutoUpdateOption.RequiredUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(new Version(1, 1, 1330), update.LatestVersion);
            Assert.AreEqual(new Version(1, 1, 1300), update.MinimumVersion);
            Assert.AreEqual("https://somehost.com/somepath/1.1.1330/releasenotes.html", update.ReleaseNotesUri.ToString());
            githubMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsNoUpgrade_ReturnsCurrent_FieldsAreCorrect()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: CurrentUpgradeDataForDefaultOptionalUpgradeForInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            Assert.AreEqual(AutoUpdateOption.Current, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(new Version(1, 1, 1234), update.LatestVersion);
            Assert.AreEqual(new Version(1, 1, 1000), update.MinimumVersion);
            Assert.AreEqual("https://somehost.com/somepath/1.1.1234/releasenotes.html", update.ReleaseNotesUri.ToString());
            githubMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsOptionalUpgrade_ReturnsOptionalUpgrade()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: UptionalUpgradeDataForDefaultRequiredUpdateForInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            Assert.AreEqual(AutoUpdateOption.OptionalUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(new Version(1, 1, 1250), update.LatestVersion);
            Assert.AreEqual(new Version(1, 1, 1000), update.MinimumVersion);
            Assert.AreEqual("https://somehost.com/somepath/1.1.1250/releasenotes.html", update.ReleaseNotesUri.ToString());
            githubMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_ConfigShowsRequiredUpgrade_ReturnsRequiredUpgrade()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: RequiredUpgradeDataForDefaultAndInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            Assert.AreEqual(AutoUpdateOption.RequiredUpgrade, update.UpdateOptionAsync.Result);
            Assert.AreEqual(TestInstalledVersion, update.InstalledVersion.ToString());
            Assert.AreEqual(new Version(1, 1, 1250), update.LatestVersion);
            Assert.AreEqual(new Version(1, 1, 1240), update.MinimumVersion);
            Assert.AreEqual("https://somehost.com/somepath/1.1.1250/releasenotes.html", update.ReleaseNotesUri.ToString());
            githubMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateAsync_UnableToGetConfig_ReturnsDownloadFailed()
        {
            AutoUpdate update = new AutoUpdate(MinimalGitHubWrapper, () => TestInstalledVersion);
            Assert.AreEqual(UpdateResult.Unknown, update.UpdateAsync().Result);
        }

        [TestMethod]
        [Timeout(2000)]
        public void UpdateOptionAsync_InitializationTimeIsNotZero()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: CurrentUpgradeDataForDefaultOptionalUpgradeForInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);
            // We have to wait for initialization to complete
            update.UpdateOptionAsync.Wait();
            Assert.IsTrue(update.GetInitializationTime().HasValue);
            Assert.AreNotEqual(TimeSpan.Zero, update.GetInitializationTime().Value);
            githubMock.VerifyAll();
        }

        [TestMethod]
        //[Timeout(2000)]
        public void UpdateOptionAsync_UpdateTimeIsNotZero()
        {
            Mock<IGitHubWrapper> githubMock = BuildGitHubWrapper(channelInfoString: CurrentUpgradeDataForDefaultOptionalUpgradeForInsider);
            AutoUpdate update = new AutoUpdate(githubMock.Object, () => TestInstalledVersion);

            // ensure that the UpdateTime is 0 before calling UpdateAsync
            TimeSpan? updateTime = update.GetUpdateTime();
            Assert.IsTrue(updateTime.HasValue);
            Assert.AreEqual(TimeSpan.Zero, updateTime.Value);

            update.UpdateAsync().Wait();

            // ensure that the UpdateTime is not 0 after calling UpdateAsync
            updateTime = update.GetUpdateTime();
            Assert.IsTrue(updateTime.HasValue);
            Assert.AreNotEqual(TimeSpan.Zero, updateTime.Value);
            githubMock.VerifyAll();
        }
    }
}
