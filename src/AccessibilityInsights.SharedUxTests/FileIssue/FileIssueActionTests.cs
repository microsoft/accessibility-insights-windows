// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.Telemetry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

using PropertyBag = System.Collections.Generic.IReadOnlyDictionary<string, string>;

namespace AccessibilityInsights.SharedUxTests.FileIssue
{
    [TestClass]
    public class FileIssueActionTests
    {
        const string DISPLAY_NAME = "GitHub";

        private Mock<ITelemetrySink> _telemetrySinkMock;

        [TestInitialize]
        public void BeforeEachTest()
        {
            _telemetrySinkMock = new Mock<ITelemetrySink>(MockBehavior.Strict);
            Logger.SetTelemetrySink(_telemetrySinkMock.Object);
        }

        [TestCleanup]
        public void AfterEachTest()
        {
            Logger.SetTelemetrySink(null);
            IssueReporter.TestControlledIsEnabled = null;
            IssueReporter.TestControlledDisplayName = null;
            IssueReporter.TestControlledFileIssueAsync = null;
        }

        [TestMethod]
        [Timeout(1000)]
        public void FileNewBug_IsNotEnabled_ReturnsNull()
        {
            bool wasIssueFiled = false;

            IssueReporter.TestControlledIsEnabled = false;
            IssueReporter.TestControlledFileIssueAsync = (issueInformation) =>
            {
                wasIssueFiled = true;
                return null;
            };

            var issueInfo = new IssueInformation();
            var output = FileIssueAction.FileIssueAsync(issueInfo);

            Assert.IsNull(output);
            Assert.IsFalse(wasIssueFiled);
        }

        /// <summary>
        /// Tests the shape of the telemetry we send when we file a bug with no rule
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void FileIssueAsync_NoRuleIsSpecified_SetsCorrectTelemetry()
        {
            IssueInformation expectedIssueInformation = new IssueInformation();
            IssueInformation actualIssueInformation = null;

            List<PropertyBag> capturedTelemetry = CaptureTelemetryEvents(TelemetryAction.Issue_File_Attempt.ToString());

            Mock<IIssueResult> issueResultMock = new Mock<IIssueResult>(MockBehavior.Strict);
            issueResultMock.Setup(x => x.IssueLink).Returns<Uri>(null);

            IssueReporter.TestControlledIsEnabled = true;
            IssueReporter.TestControlledDisplayName = DISPLAY_NAME;
            IssueReporter.TestControlledFileIssueAsync = (issueInformation) =>
            {
                actualIssueInformation = issueInformation;
                return issueResultMock.Object;
            };

            IIssueResult result = FileIssueAction.FileIssueAsync(expectedIssueInformation);

            Assert.AreSame(issueResultMock.Object, result);
            Assert.AreSame(expectedIssueInformation, actualIssueInformation);
            Assert.AreEqual(1, capturedTelemetry.Count);
            Assert.AreEqual(DISPLAY_NAME, capturedTelemetry[0][TelemetryProperty.IssueReporter.ToString()]);

            _telemetrySinkMock.VerifyAll();
            issueResultMock.VerifyAll();
        }

        /// <summary>
        /// Tests the shape of the telemetry we send when there is a specific rule failure
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void FileIssueAsync_RuleIsSpecified_SetsCorrectTelemetry()
        {
            const string expectedRule = "An awesome rule";

            IssueInformation expectedIssueInformation = new IssueInformation(ruleForTelemetry: expectedRule);
            IssueInformation actualIssueInformation = null;

            List<PropertyBag> capturedTelemetry = CaptureTelemetryEvents(TelemetryAction.Issue_Save.ToString());

            Mock<IIssueResult> issueResultMock = new Mock<IIssueResult>(MockBehavior.Strict);
            issueResultMock.Setup(x => x.IssueLink).Returns(new Uri("https://AccessibilityInsights.io"));

            IssueReporter.TestControlledIsEnabled = true;
            IssueReporter.TestControlledDisplayName = DISPLAY_NAME;
            IssueReporter.TestControlledFileIssueAsync = (issueInformation) =>
            {
                actualIssueInformation = issueInformation;
                return issueResultMock.Object;
            };

            IIssueResult result = FileIssueAction.FileIssueAsync(expectedIssueInformation);

            Assert.AreEqual(3, capturedTelemetry[0].Count);
            Assert.AreEqual(expectedRule, capturedTelemetry[0][TelemetryProperty.RuleId.ToString()]);
            Assert.AreEqual("", capturedTelemetry[0][TelemetryProperty.UIFramework.ToString()]);
            Assert.AreEqual(DISPLAY_NAME, capturedTelemetry[0][TelemetryProperty.IssueReporter.ToString()]);

            _telemetrySinkMock.VerifyAll();
            issueResultMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FileIssueAsync_ExceptionIsThrown_IsReportedToTelemetry()
        {
            Exception actualException = null;
            IssueInformation expectedIssueInformation = new IssueInformation();

            _telemetrySinkMock.Setup(x => x.IsEnabled).Returns(true);
            _telemetrySinkMock.Setup(x => x.ReportException(It.IsAny<Exception>()))
                .Callback<Exception>((e) => actualException = e);

            IssueReporter.TestControlledIsEnabled = true;
            IssueReporter.TestControlledFileIssueAsync = (issueInformation) =>
            {
                throw new InvalidCastException();
            };

            Assert.IsNull(FileIssueAction.FileIssueAsync(expectedIssueInformation));

            Assert.IsNotNull(actualException);
            Assert.IsInstanceOfType(actualException, typeof(InvalidCastException));

            _telemetrySinkMock.VerifyAll();
        }

        private List<PropertyBag> CaptureTelemetryEvents(string eventName)
        {
            List<PropertyBag> capturedTelemetry = new List<PropertyBag>();

            _telemetrySinkMock.Setup(x => x.IsEnabled).Returns(true);
            _telemetrySinkMock.Setup(x => x.PublishTelemetryEvent(eventName, It.IsAny<PropertyBag>()))
                .Callback<string, PropertyBag>((_, p) => capturedTelemetry.Add(p));

            return capturedTelemetry;
        }
    }
}
