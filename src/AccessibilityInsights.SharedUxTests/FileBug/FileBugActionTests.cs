// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Fakes;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Desktop.Telemetry;
using AccessibilityInsights.Desktop.Telemetry.Fakes;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.Interfaces.BugReporting.Fakes;
using AccessibilityInsights.SharedUx.FileBug;
using AccessibilityInsights.SharedUx.FileBug.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccessibilityInsights.SharedUxTests.FileBug
{
    [TestClass]
    public class FileBugActionTests
    {
        static readonly Uri FAKE_SERVER_URL = new Uri("https://myaccount.visualstudio.com/");

        [TestMethod]
        public void FileNewBug_IsNotEnabled_ReturnsPlaceholder()
        {
            using (ShimsContext.Create())
            {
                ShimBugReporter.IsEnabledGet = () => false;
                var bugInfo = new BugInformation();
                var connInfo = new StubIConnectionInfo();
                var output = FileBugAction.FileNewBug(bugInfo,
                    connInfo, false, 0, (_) => { });

                Assert.IsNull(output.bugId);
                Assert.IsNotNull(output.newBugId);
                Assert.IsTrue(string.IsNullOrEmpty(output.newBugId));
            }
        }

        /// <summary>
        /// Tests the shape of the telemetry we send when we file a bug with no rule
        /// </summary>
        [TestMethod]
        public void TestBugFilingTelemetryNoRule()
        {
            using (ShimsContext.Create())
            {
                SetUpShims();

                // Save telemetry locally
                List<Tuple<TelemetryAction, TelemetryProperty, string>> telemetryLog = new List<Tuple<TelemetryAction, TelemetryProperty, string>>();
                ShimLogger.PublishTelemetryEventTelemetryActionTelemetryPropertyString = (action, property, value) =>
                {
                    telemetryLog.Add(new Tuple<TelemetryAction, TelemetryProperty, string>(action, property, value));
                };

                var bugInfo = new BugInformation();
                var connInfo = new StubIConnectionInfo
                {
                    ServerUriGet = () => FAKE_SERVER_URL,
                };
                (int? bugId, string newBugId) = FileBugAction.FileNewBug(bugInfo,
                    connInfo, false, 0, (_) => { } );

                Assert.AreEqual(new Tuple<TelemetryAction, TelemetryProperty, string>(TelemetryAction.Bug_Save, TelemetryProperty.Url, FAKE_SERVER_URL.AbsoluteUri), telemetryLog[0]);
            }
        }

        /// <summary>
        /// Tests the shape of the telemetry we send when there is a specific rule failure
        /// </summary>
        [TestMethod]
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

                var bugInfo = new BugInformation(ruleForTelemetry: RuleId.BoundingRectangleContainedInParent.ToString());
                var connInfo = new StubIConnectionInfo
                {
                    ServerUriGet = () => FAKE_SERVER_URL,
                };
                (int? bugId, string newBugId) = FileBugAction.FileNewBug(bugInfo,
                    connInfo, false, 0, (_) => { } );

                Assert.AreEqual(RuleId.BoundingRectangleContainedInParent.ToString(), telemetryLog[0].Item2[TelemetryProperty.RuleId]);
                Assert.AreEqual("", telemetryLog[0].Item2[TelemetryProperty.UIFramework]);
                Assert.AreEqual(FAKE_SERVER_URL, telemetryLog[0].Item2[TelemetryProperty.Url]);
            }
        }

        [TestMethod]
        [Timeout(10000)]
        public void RemoveInternalFromBugText_MatchingTextExists()
        {
            var guid = Guid.NewGuid().ToString();
            // Internal id doesn't exist if the text is modified by user in edit pane. this scenario simulate the case. 
            string original = $"<br><br><div><hr>{guid}<hr></div>";
            string expected = "\r\n<BODY><BR><BR>\r\n<DIV></DIV></BODY>";

            Assert.AreEqual(expected, FileBugAction.RemoveInternalHTML(original, guid));
        }

        [TestMethod]
        [Timeout(10000)]
        public void RemoveInternalFromBugText_NoMatchingText()
        {
            var guid = Guid.NewGuid().ToString();

            string original = "<br><br><div><hr>should not be removed<hr></div>";
            string expected = "\r\n<BODY><BR><BR>\r\n<DIV>\r\n<HR>\r\nshould not be removed\r\n<HR>\r\n</DIV></BODY>";

            Assert.AreEqual(expected, FileBugAction.RemoveInternalHTML(original, guid));
        }

        public static void SetUpShims()
        {
            ShimGetDataAction.GetElementDataContextGuid = (_) =>
            {
                return null;
            };
            ShimBugReporter.IsEnabledGet = () => true;
            ShimBugReporter.CreateBugPreviewAsyncIConnectionInfoBugInformation =  (_, __) =>
            {
                return Task.FromResult(FAKE_SERVER_URL);
            };
            ShimFileBugAction.FileBugWindowUriBooleanInt32ActionOfInt32 = (_, __, ___, ____) =>
            {
                int? retBugId = 5;
                return retBugId;
            };
        }
    }
}
