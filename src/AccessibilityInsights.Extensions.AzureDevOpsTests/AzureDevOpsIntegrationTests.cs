// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests
{
    [TestClass]
    public class AzureDevOpsIntegrationTests
    {
        const string TestProject = "my project";
        const string TestTeam = "my team";
        static readonly Uri TestUri = new Uri("https://myaccount.visualstudio.com");
        static readonly Uri SlashTestUri = new Uri("https://myaccount.visualstudio.com/");

        [TestMethod]
        [Timeout(1000)]
        public void GetTeamProjectUri_ProjectNameIsNull_ReturnsNull()
        {
            Assert.IsNull(new AzureDevOpsIntegration().GetTeamProjectUri(null, TestTeam));
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetTeamProjectUri_NotConnected_ReturnsNull()
        {
            Assert.IsNull(new AzureDevOpsIntegration().GetTeamProjectUri(TestProject, TestTeam));
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetTeamProjectUriInternal_TeamIsNotNull_ReturnsExpectedUri()
        {
            const string expectedUri = "https://myaccount.visualstudio.com/my%20project/my%20team";

            Assert.AreEqual(expectedUri,
                AzureDevOpsIntegration.GetTeamProjectUriInternal(TestProject, TestTeam, TestUri).AbsoluteUri);

            Assert.AreEqual(expectedUri,
                AzureDevOpsIntegration.GetTeamProjectUriInternal(TestProject, TestTeam, SlashTestUri).AbsoluteUri);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TestGetTeamProjectUriInternal_TeamIsNull_ReturnsExpectedUri()
        {
            const string expectedUri = "https://myaccount.visualstudio.com/my%20project";

            Assert.AreEqual(expectedUri,
                AzureDevOpsIntegration.GetTeamProjectUriInternal(TestProject, null, TestUri).AbsoluteUri);

            Assert.AreEqual(expectedUri,
                AzureDevOpsIntegration.GetTeamProjectUriInternal(TestProject, null, SlashTestUri).AbsoluteUri);
        }
    }
}
