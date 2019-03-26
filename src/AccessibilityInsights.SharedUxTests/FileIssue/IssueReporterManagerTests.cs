using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUxTests.FileIssue
{
    [TestClass]
    public class IssueReporterManagerTests
    {
        private const string TestGuidString = "84553fe4-0e1c-4ef8-bacc-a51dc47ec59e";
        private const string TestReporterConfigs = "hello world";
        private const string RandomTestGuid = "cfef03dc-c4ed-4c09-b7ac-d556148e15a4";
        private string TestSerializedConfigs = "{\"" + TestGuidString + "\":\"" + TestReporterConfigs + "\"}";
        private Guid TestGuid = Guid.Parse(TestGuidString);

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_Normal_DictionaryPopulated()
        {
            ConfigurationModel configs = new ConfigurationModel();
            configs.IssueReporterSerializedConfigs = TestSerializedConfigs;

            Mock<IIssueReporting> issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Once);
            Assert.IsTrue(repManager.GetIssueFilingOptionsDict().ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_NullConfigs_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel(null);

            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Never);
            Assert.IsTrue(repManager.GetIssueFilingOptionsDict().ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_EmptyConfigs_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel(string.Empty);

            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Never);
            Assert.IsTrue(repManager.GetIssueFilingOptionsDict().ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_WhitespaceConfigs_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel(" ");

            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(TestReporterConfigs), Times.Never);
            Assert.IsTrue(repManager.GetIssueFilingOptionsDict().ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_EmptyReporterConfig_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel("{\"" + TestGuidString + "\":\"\"}");

            var issueReporterMock = GetIssueReporterMock();
            issueReporterMock.Setup(p => p.RestoreConfigurationAsync(string.Empty)).Returns(new Task<IIssueResult>(() => { return null; }));
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(string.Empty), Times.Never);
            Assert.IsTrue(repManager.GetIssueFilingOptionsDict().ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void Constructor_NullReporterConfig_NoRestore()
        {
            ConfigurationModel configs = GetConfigurationModel("{\"" + RandomTestGuid + "\":\"\"}");

            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };

            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

            issueReporterMock.Verify(p => p.RestoreConfigurationAsync(null), Times.Never);
            Assert.IsTrue(repManager.GetIssueFilingOptionsDict().ContainsKey(TestGuid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void SetIssueReporter_Valid_ReporterSet()
        {
            ConfigurationModel configs = GetConfigurationModel(null);

            var issueReporterMock = GetIssueReporterMock();
            List<IIssueReporting> issueReportingOptions = new List<IIssueReporting>() { issueReporterMock.Object };
            IssueReporterManager repManager = new IssueReporterManager(configs, issueReportingOptions);

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

        private Mock<IIssueReporting> GetIssueReporterMock()
        {
            var issueReporterMock = new Mock<IIssueReporting>();
            issueReporterMock.Setup(p => p.StableIdentifier).Returns(TestGuid);
            issueReporterMock.Setup(p => p.RestoreConfigurationAsync(TestReporterConfigs)).Returns(new Task<IIssueResult>(() => { return null; }));
            return issueReporterMock;
        }

        private static ConfigurationModel GetConfigurationModel(string config)
        {
            ConfigurationModel configs = new ConfigurationModel();
            configs.IssueReporterSerializedConfigs = config;
            return configs;
        }
    }
}
