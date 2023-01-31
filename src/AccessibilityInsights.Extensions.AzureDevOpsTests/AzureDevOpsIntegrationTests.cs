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

        [TestMethod]
        [Timeout(1000)]
        public void EscapeForUrl_InputHasNoSubstitutions_ReturnsExpectedUri()
        {
            const string inputUri = "https://www.github.com/accessibility-insights-windows/issues?text=This is some text";
            const string expectedUri = "https%3A%2F%2Fwww.github.com%2Faccessibility-insights-windows%2Fissues%3Ftext%3DThis%20is%20some%20text";

            string outputUri = AzureDevOpsIntegration.EscapeForUrl(inputUri);

            Assert.AreEqual(expectedUri, outputUri);
        }

        [TestMethod]
        [Timeout(1000)]
        public void EscapeForUrl_InputHasMiddleDots_ReturnsExpectedUri()
        {
            const string inputUri = "https://www.github.com/accessibility-insights-windows/issues?text=This·has·middle·dots";
            const string expectedUri = "https%3A%2F%2Fwww.github.com%2Faccessibility-insights-windows%2Fissues%3Ftext%3DThis-has-middle-dots";

            string outputUri = AzureDevOpsIntegration.EscapeForUrl(inputUri);

            Assert.AreEqual(expectedUri, outputUri);
        }

        [TestMethod]
        [Timeout(1000)]
        public void EscapeForUrl_InputHasNonBreakingSpaces_ReturnsExpectedUri()
        {
            const string inputUri = "https://www.github.com/accessibility-insights-windows/issues?text=This\u00a0has\u00a0non-breaking\u00a0spaces";
            const string expectedUri = "https%3A%2F%2Fwww.github.com%2Faccessibility-insights-windows%2Fissues%3Ftext%3DThis%20has%20non-breaking%20spaces";

            string outputUri = AzureDevOpsIntegration.EscapeForUrl(inputUri);

            Assert.AreEqual(expectedUri, outputUri);
        }

        [TestMethod]
        [Timeout(1000)]
        public void EscapeForUrl_InputHasHighCharacters_ReturnsExpectedUri()
        {
            const string inputUri = "https://www.github.com/accessibility-insights-windows/issues?text=This\u0080has\u0090high\u00b0characters";
            const string expectedUri = "https%3A%2F%2Fwww.github.com%2Faccessibility-insights-windows%2Fissues%3Ftext%3DThis%3Fhas%3Fhigh%3Fcharacters";

            string outputUri = AzureDevOpsIntegration.EscapeForUrl(inputUri);

            Assert.AreEqual(expectedUri, outputUri);
        }
    }
}
