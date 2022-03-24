// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests
{
    [TestClass]
    public class ConnectionInfoTests
    {
        private const string KnownConfigString = @"{""ServerUri"":""https://www.bing.com"",""Project"":{""Name"":""My Project"",""Id"":""71b41af3-4c55-44eb-a1d4-3b0973c07ea3""},""Team"":{""ParentProject"":{""Name"":""My Project"",""Id"":""71b41af3-4c55-44eb-a1d4-3b0973c07ea3""},""Name"":""My Team"",""Id"":""4a56a1a0-b99f-49ca-986d-9c6d7fa6137d""},""LastUsage"":""2018-12-23T16:19:30Z""}";
        private static readonly Guid TeamGuid = new Guid("{4A56A1A0-B99F-49CA-986D-9C6D7FA6137D}");
        private static readonly Guid ProjectGuid = new Guid("{71B41AF3-4C55-44EB-A1D4-3B0973C07EA3}");
        private static readonly TeamProject TestProject = new TeamProject("My Project", ProjectGuid);
        private static readonly AdoTeam TestTeam = new AdoTeam("My Team", TeamGuid, TestProject);
        private static readonly Uri TestUri = new Uri("https://www.bing.com");
        private static readonly DateTime LastUsage = new DateTime(2018, 12, 23, 16, 19, 30, DateTimeKind.Utc);

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_Uri_FieldsAreCorrect()
        {
            ConnectionInfo info = new ConnectionInfo(TestUri, null, null);
            Assert.AreEqual(TestUri, info.ServerUri);
            Assert.IsNull(info.Project);
            Assert.IsNull(info.Team);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_UriAndProject_FieldsAreCorrect()
        {
            ConnectionInfo info = new ConnectionInfo(TestUri, TestProject, null);
            Assert.AreEqual(TestUri, info.ServerUri);
            Assert.AreEqual(TestProject, info.Project);
            Assert.IsNull(info.Team);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_UriAndTeam_FieldsAreCorrect()
        {
            ConnectionInfo info = new ConnectionInfo(TestUri, null, TestTeam);
            Assert.AreEqual(TestUri, info.ServerUri);
            Assert.IsNull(info.Project);
            Assert.AreEqual(TestTeam, info.Team);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Ctor_KnownConfigString_MatchesExpectations()
        {
            ConnectionInfo info = new ConnectionInfo(KnownConfigString);
            Assert.AreEqual(TestUri, info.ServerUri);
            Assert.AreEqual(TestProject.Name, info.Project.Name);
            Assert.AreEqual(TestProject.Id, info.Project.Id);
            Assert.AreEqual(TestTeam.Name, info.Team.Name);
            Assert.AreEqual(TestTeam.Id, info.Team.Id);
            Assert.AreEqual(TestProject.Name, info.Team.ParentProject.Name);
            Assert.AreEqual(LastUsage, info.LastUsage);
        }

        [TestMethod]
        [Timeout(1000)]
        public void ToConfigString_MatchesKnownConfigString()
        {
            // Note: This test might break if the serialization order changes
            ConnectionInfo info = new ConnectionInfo(TestUri, TestProject, TestTeam);
            info.SetLastUsage(LastUsage);
            string configString = info.ToConfigString();
            Assert.AreEqual(KnownConfigString, configString);
        }
    }
}
