// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUxTests.FileIssue
{
    [TestClass]
    public class IssueReporterManagerTests
    {
        private const string TestGuidString = "84553fe4-0e1c-4ef8-bacc-a51dc47ec59e";
        private const string TestReporterConfigs = "hello world";
        private const string RandomTestGuid = "cfef03dc-c4ed-4c09-b7ac-d556148e15a4";
        private readonly string TestSerializedConfigs = "{\"" + TestGuidString + "\":\"" + TestReporterConfigs + "\"}";
        private Guid TestGuid = Guid.Parse(TestGuidString);

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_AppConfigIsNull_ThrowsArgumentNullException()
        {
            new IssueReporterManager(null, Enumerable.Empty<IIssueReporting>());
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_EnumerbleIsNull_DoesNotThrow()
        {
            ConfigurationModel configs = new ConfigurationModel();

            new IssueReporterManager(configs, null);
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_NoNullInputs_DoesNotThrow()
        {
            ConfigurationModel configs = new ConfigurationModel();

            new IssueReporterManager(configs, Enumerable.Empty<IIssueReporting>());
        }

        [TestMethod]
        [Timeout(1000)]
        public void RestorePersistedConfigurations_Normal_DictionaryPopulated()
        {
            ConfigurationModel configs = new ConfigurationModel
            {
                IssueReporterSerializedConfigs = TestSerializedConfigs
            };

            Mock<IIssueReporting> issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.RestorePersistedConfigurations();

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Once);
            Assert.IsTrue(repManager.IssueFilingOptionsDict.ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void RestorePersistedConfigurations_NullConfigs_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel(null);

            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.RestorePersistedConfigurations();

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Never);
            Assert.IsTrue(repManager.IssueFilingOptionsDict.ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void RestorePersistedConfigurations_EmptyConfigs_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel(string.Empty);
            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.RestorePersistedConfigurations();

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Never);
            Assert.IsTrue(repManager.IssueFilingOptionsDict.ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void RestorePersistedConfigurations_WhitespaceConfigs_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel(" ");
            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.RestorePersistedConfigurations();

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Never);
            Assert.IsTrue(repManager.IssueFilingOptionsDict.ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void RestorePersistedConfigurations_EmptyReporterConfig_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel("{\"" + TestGuidString + "\":\"\"}");
            var issueReporterMock = GetIssueReporterMock();
            issueReporterMock.Setup(p => p.RestoreConfigurationAsync(string.Empty)).Returns(new Task<IIssueResult>(() => { return null; }));
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.RestorePersistedConfigurations();

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(string.Empty), Times.Never);
            Assert.IsTrue(repManager.IssueFilingOptionsDict.ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void RestorePersistedConfigurations_NullReporterConfig_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel("{\"" + RandomTestGuid + "\":\"\"}");
            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.RestorePersistedConfigurations();

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(null), Times.Never);
            Assert.IsTrue(repManager.IssueFilingOptionsDict.ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void SetIssueReporter_Valid_ReporterSet()
        {
            ConfigurationModel configs = GetConfigurationModel(null);
            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);
            repManager.RestorePersistedConfigurations();

            repManager.SetIssueReporter(TestGuid);

            Assert.AreEqual(TestGuid, IssueReporter.IssueReporting.StableIdentifier);
        }

        [TestMethod]
        [Timeout(1000)]
        public void SetIssueReporter_ReporterNonExistent_ReporterUnset()
        {
            ConfigurationModel configs = GetConfigurationModel(null);
            IssueReporter.IssueReporting = null;
            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            repManager.SetIssueReporter(Guid.Parse(RandomTestGuid));

            Assert.IsNull(IssueReporter.IssueReporting);
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UpdateIssueReporterSettings_InputIsNull_ThrowsArgumentNullException()
        {
            ConfigurationModel configs = GetConfigurationModel(null);
            IssueReporterManager repManager = new IssueReporterManager(configs, Enumerable.Empty<IIssueReporting>());

            repManager.UpdateIssueReporterSettings(null);
        }

        [TestMethod]
        [Timeout(1000)]
        public void UpdateIssueReporterSettings_ReporterDoesNotSupportGetSettings_DoesNothing()
        {
            ConfigurationModel configs = GetConfigurationModel(null);
            IssueReporterManager repManager = new IssueReporterManager(configs, Enumerable.Empty<IIssueReporting>());
            Mock<IIssueReporting> issueReportingMock = GetIssueReporterMock(
                setStableIdentifier: false, expectRestoreConfig: false, supportGetSerializedSettings: false);

            repManager.UpdateIssueReporterSettings(issueReportingMock.Object);

            issueReportingMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void UpdateIssueReporterSettings_SettingNotInOriginalConfig_AddsToConfig()
        {
            ConfigurationModel configs = GetConfigurationModel(null);
            IssueReporterManager repManager = new IssueReporterManager(configs, Enumerable.Empty<IIssueReporting>());
            Mock<IIssueReporting> issueReportingMock = GetIssueReporterMock(
                expectRestoreConfig: false, supportGetSerializedSettings: true);

            repManager.UpdateIssueReporterSettings(issueReportingMock.Object);

            issueReportingMock.VerifyAll();
            Assert.AreEqual(TestReporterConfigs, GetIssueReporterConfig(configs, TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void UpdateIssueReporterSettings_SettingInOriginalConfig_UpdatesConfig()
        {
            const string newSettings = "la di dah";
            ConfigurationModel configs = GetConfigurationModel(null);
            configs.IssueReporterSerializedConfigs = TestSerializedConfigs;
            IssueReporterManager repManager = new IssueReporterManager(configs, Enumerable.Empty<IIssueReporting>());
            Mock<IIssueReporting> issueReportingMock = GetIssueReporterMock(
                expectRestoreConfig: false, supportGetSerializedSettings: true,
                newSettings: newSettings);
            Assert.IsFalse(configs.IssueReporterSerializedConfigs.Contains(newSettings));
            Assert.AreEqual(TestReporterConfigs, GetIssueReporterConfig(configs, TestGuid));

            repManager.UpdateIssueReporterSettings(issueReportingMock.Object);

            issueReportingMock.VerifyAll();
            Assert.AreEqual(newSettings, GetIssueReporterConfig(configs, TestGuid));
        }

        private string GetIssueReporterConfig(ConfigurationModel config, Guid stableIdentifier)
        {
            string serializedData = config.IssueReporterSerializedConfigs;

            Dictionary<Guid, string> configsDictionary =
                JsonConvert.DeserializeObject<Dictionary<Guid, string>>(serializedData);

            return configsDictionary[stableIdentifier];
        }

        private Mock<IIssueReporting> GetIssueReporterMock(bool setStableIdentifier = true,
            bool expectRestoreConfig = true, bool? supportGetSerializedSettings = null,
            string newSettings = TestReporterConfigs)
        {
            var issueReporterMock = new Mock<IIssueReporting>(MockBehavior.Strict);

            if (setStableIdentifier)
            {
                issueReporterMock.Setup(p => p.StableIdentifier).Returns(TestGuid);
            }

            if (expectRestoreConfig)
            {
                issueReporterMock.Setup(p => p.RestoreConfigurationAsync(TestReporterConfigs)).Returns(new Task<IIssueResult>(() => { return null; }));
            }

            if (supportGetSerializedSettings.HasValue)
            {
                string settings = supportGetSerializedSettings.Value
                    ? newSettings : null;
                issueReporterMock.Setup(p => p.TryGetCurrentSerializedSettings(out settings))
                    .Returns(supportGetSerializedSettings.Value);
            }

            return issueReporterMock;
        }

        private static ConfigurationModel GetConfigurationModel(string config)
        {
            ConfigurationModel configs = new ConfigurationModel
            {
                IssueReporterSerializedConfigs = config
            };
            return configs;
        }
    }
}
