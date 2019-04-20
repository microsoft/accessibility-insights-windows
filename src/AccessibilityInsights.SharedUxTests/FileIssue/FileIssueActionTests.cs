// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Axe.Windows.Actions.Fakes;
using Axe.Windows.Core.Enums;
using AccessibilityInsights.SharedUx.Telemetry;
using AccessibilityInsights.SharedUx.Telemetry.Fakes;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using AccessibilityInsights.SharedUx.FileIssue;
using AccessibilityInsights.SharedUx.FileIssue.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.SharedUxTests.FileIssue
{
    [TestClass]
    public class FileIssueActionTests
    {
#if FAKES_SUPPORTED
        static readonly string DISPLAY_NAME = "GitHub";

        [TestMethod]
        [Timeout(1000)]
        public void FileNewBug_IsNotEnabled_ReturnsNull()
        {
            using (ShimsContext.Create())
            {
                ShimIssueReporter.IsEnabledGet = () => false;
                var issueInfo = new IssueInformation();
                var output = FileIssueAction.FileIssueAsync(issueInfo);

                Assert.IsNull(output);
            }
        }

        /// <summary>
        /// Tests the shape of the telemetry we send when we file a bug with no rule
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void TestBugFilingTelemetryNoRule()
        {
            using (ShimsContext.Create())
            {
                SetUpShims();

                // Save telemetry locally
                List<Tuple<TelemetryAction, IReadOnlyDictionary<TelemetryProperty, string>>> telemetryLog = new List<Tuple<TelemetryAction, IReadOnlyDictionary<TelemetryProperty, string>>>();
                ShimLogger.PublishTelemetryEventTelemetryActionIReadOnlyDictionaryOfTelemetryPropertyString = (action, dict) =>
                {
                    telemetryLog.Add(new Tuple<TelemetryAction, IReadOnlyDictionary<TelemetryProperty, string>>(action, dict));
                };

                var issueInfo = new IssueInformation();
                var result = FileIssueAction.FileIssueAsync(issueInfo);

                Assert.AreEqual(1, telemetryLog.Count);
                Assert.AreEqual(DISPLAY_NAME, telemetryLog[0].Item2[TelemetryProperty.IssueReporter]);
            }
        }

        /// <summary>
        /// Tests the shape of the telemetry we send when there is a specific rule failure
        /// </summary>
        [TestMethod]
        [Timeout(1000)]
        public void TestBugFilingTelemetrySpecificRule()
        {
            using (ShimsContext.Create())
            {
                SetUpShims();

                // Save telemetry locally
                List<Tuple<TelemetryAction, IReadOnlyDictionary<TelemetryProperty, string>>> telemetryLog = new List<Tuple<TelemetryAction, IReadOnlyDictionary<TelemetryProperty, string>>>();
                ShimLogger.PublishTelemetryEventTelemetryActionIReadOnlyDictionaryOfTelemetryPropertyString = (action, dict) =>
                {
                    telemetryLog.Add(new Tuple<TelemetryAction, IReadOnlyDictionary<TelemetryProperty, string>>(action, dict));
                };

                var issueInfo = new IssueInformation(ruleForTelemetry: RuleId.BoundingRectangleContainedInParent.ToString());
                var result = FileIssueAction.FileIssueAsync(issueInfo);

                Assert.AreEqual(3, telemetryLog[0].Item2.Count);
                Assert.AreEqual(RuleId.BoundingRectangleContainedInParent.ToString(), telemetryLog[0].Item2[TelemetryProperty.RuleId]);
                Assert.AreEqual("", telemetryLog[0].Item2[TelemetryProperty.UIFramework]);
                Assert.AreEqual(DISPLAY_NAME, telemetryLog[0].Item2[TelemetryProperty.IssueReporter]);
            }
        }

        public static void SetUpShims()
        {
            ShimGetDataAction.GetElementDataContextGuid = (_) =>
            {
                return null;
            };
            ShimIssueReporter.IsEnabledGet = () => true;
            ShimIssueReporter.DisplayNameGet = () => DISPLAY_NAME;
            ShimIssueReporter.FileIssueAsyncIssueInformation = (_) =>
            {
                var mockIssueResult = new Mock<IIssueResult>();
                mockIssueResult.Setup(p => p.DisplayText).Returns("Issue Display text");
                mockIssueResult.Setup(p => p.IssueLink).Returns(new Uri("https://www.google.com"));
                return mockIssueResult.Object;
            };
        }
#endif
    }
}
