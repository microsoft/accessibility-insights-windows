// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.AzureDevOps.Fakes;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests
{
    [TestClass]
    public class AzureDevOpsBugReportingUnitTests
    {
        private const string TestMessage = "Test message";
        private const string RuleDescription = "Rule Description";


        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringIsNull_ReturnsNull()
        {
            const string originalString = null;
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.IsNull(result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringIsWithinLimit_ReturnsOriginalString()
        {
            const string originalString = "abc";
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringExceedsLimit_ReturnsTruncatedOriginalStringWithSuffix()
        {
            const string originalString = "abcd";
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.AreEqual("abc!!!", result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringIsNull_ReturnsNull()
        {
            const string originalString = null;
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.IsNull(result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasNoLeadingBracket_ReturnsOriginalString()
        {
            const string originalString = "abc]";
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasNoTrailingBracket_ReturnsOriginalString()
        {
            const string originalString = "[abc";
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasLeadingAndTrailingBrackets_ReturnsSubstring()
        {
            const string originalString = "[abc]";
            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            string result = privateAzureDevOpsBugReporting.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_AllFieldsNull_AddsNullValues()
        {
            Dictionary<BugField, string> bugFieldPairs = new Dictionary<BugField, string>();
            BugInformation bugInfo = new BugInformation();

            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            privateAzureDevOpsBugReporting.InvokeStatic("TruncateSelectedFields", bugInfo, bugFieldPairs);

            Assert.AreEqual(4, bugFieldPairs.Count);
            Assert.IsNull(bugFieldPairs[BugField.ProcessName]);
            Assert.IsNull(bugFieldPairs[BugField.Glimpse]);
            Assert.IsNull(bugFieldPairs[BugField.TestMessages]);
            Assert.IsNull(bugFieldPairs[BugField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_ProcessNameIsSpecified_AddsProcessNameValue()
        {
            string originalProcessName = new string('x', 60);
            Dictionary<BugField, string> bugFieldPairs = new Dictionary<BugField, string>();
            BugInformation bugInfo = new BugInformation(processName: originalProcessName);

            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            privateAzureDevOpsBugReporting.InvokeStatic("TruncateSelectedFields", bugInfo, bugFieldPairs);

            Assert.AreEqual(4, bugFieldPairs.Count);
            string modifiedProcessName = bugFieldPairs[BugField.ProcessName];
            Assert.AreEqual(new string('x', 50) + ".exe", modifiedProcessName);
            Assert.IsNull(bugFieldPairs[BugField.Glimpse]);
            Assert.IsNull(bugFieldPairs[BugField.TestMessages]);
            Assert.IsNull(bugFieldPairs[BugField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_GlimpseIsSpecified_AddsGlimpseValue()
        {
            string originalGlimpse = new string('y', 60);
            Dictionary<BugField, string> bugFieldPairs = new Dictionary<BugField, string>();
            BugInformation bugInfo = new BugInformation(glimpse: originalGlimpse);

            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            privateAzureDevOpsBugReporting.InvokeStatic("TruncateSelectedFields", bugInfo, bugFieldPairs);

            Assert.AreEqual(4, bugFieldPairs.Count);
            string modifiedGlimpse = bugFieldPairs[BugField.Glimpse];
            Assert.AreEqual(new string('y', 50) + "...", modifiedGlimpse);
            Assert.IsNull(bugFieldPairs[BugField.ProcessName]);
            Assert.IsNull(bugFieldPairs[BugField.TestMessages]);
            Assert.IsNull(bugFieldPairs[BugField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_TestMessagesIsSpecified_AddsTestMessagesValue()
        {
            string originalTestMessages = new string('z', 200);
            Dictionary<BugField, string> bugFieldPairs = new Dictionary<BugField, string>();
            BugInformation bugInfo = new BugInformation(testMessages: originalTestMessages);

            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            privateAzureDevOpsBugReporting.InvokeStatic("TruncateSelectedFields", bugInfo, bugFieldPairs);

            Assert.AreEqual(4, bugFieldPairs.Count);
            string modifiedTestMessages = bugFieldPairs[BugField.TestMessages];
            Assert.AreEqual(new string('z', 150) + "...open attached", modifiedTestMessages.Substring(0, 166));
            Assert.IsNull(bugFieldPairs[BugField.ProcessName]);
            Assert.IsNull(bugFieldPairs[BugField.Glimpse]);
            Assert.IsNull(bugFieldPairs[BugField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_RuleSourceIsSpecified_AddsRuleSourceValue()
        {
            const string ruleSourceContent = "weather: cold and clear";

            string originalRuleSource = '[' + ruleSourceContent + ']';
            Dictionary<BugField, string> bugFieldPairs = new Dictionary<BugField, string>();
            BugInformation bugInfo = new BugInformation(ruleSource: originalRuleSource);

            PrivateType privateAzureDevOpsBugReporting = new PrivateType(typeof(AzureDevOpsBugReporting));
            privateAzureDevOpsBugReporting.InvokeStatic("TruncateSelectedFields", bugInfo, bugFieldPairs);

            Assert.AreEqual(4, bugFieldPairs.Count);
            string modifiedRuleSource = bugFieldPairs[BugField.RuleSource];
            Assert.AreEqual(ruleSourceContent, modifiedRuleSource);
            Assert.IsNull(bugFieldPairs[BugField.ProcessName]);
            Assert.IsNull(bugFieldPairs[BugField.Glimpse]);
            Assert.IsNull(bugFieldPairs[BugField.TestMessages]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void CreateBugPreviewAsync_TeamNameIsNull_ChainsThroughCorrectly()
        {
            using (ShimsContext.Create())
            {
                const string expectedProjectName = "Super Project";
                const string expectedTeamName = null;
                string actualProjectName = null;
                string actualTeamName = null;
                Uri expectedUri = new Uri("https://www.bing.com");

                AzureDevOpsIntegration integration = new ShimAzureDevOpsIntegration
                {
                    CreateBugPreviewStringStringIReadOnlyDictionaryOfAzureDevOpsFieldString = (p, t, f) =>
                    {
                        actualProjectName = p;
                        actualTeamName = t;
                        return expectedUri;
                    },
                    ConnectedToAzureDevOpsGet = () => true,
                };

                ShimAzureDevOpsIntegration.GetCurrentInstance = () => integration;

                IBugReporting bugReporting = new AzureDevOpsBugReporting();
                IConnectionInfo connectionInfo = new ConnectionInfo(expectedUri,
                    new TeamProject(expectedProjectName, Guid.Empty),
                    new Team(expectedTeamName, Guid.Empty));
                BugInformation bugInfo = new BugInformation();

                Uri actualUri = bugReporting.CreateBugPreviewAsync(connectionInfo, bugInfo).Result;

                Assert.AreEqual(expectedUri, actualUri);
                Assert.AreEqual(expectedProjectName, actualProjectName);
                Assert.IsNull(actualTeamName);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void CreateBugPreviewAsync_TeamNameIsNotNull_ChainsThroughCorrectly()
        {
            using (ShimsContext.Create())
            {
                const string expectedProjectName = "Ultra Project";
                const string expectedTeamName = "Ultra Team";
                string actualProjectName = null;
                string actualTeamName = null;
                Uri expectedUri = new Uri("https://www.bing.com");

                AzureDevOpsIntegration integration = new ShimAzureDevOpsIntegration
                {
                    CreateBugPreviewStringStringIReadOnlyDictionaryOfAzureDevOpsFieldString = (p, t, f) =>
                    {
                        actualProjectName = p;
                        actualTeamName = t;
                        return expectedUri;
                    },
                    ConnectedToAzureDevOpsGet = () => true,
                };

                ShimAzureDevOpsIntegration.GetCurrentInstance = () => integration;

                IBugReporting bugReporting = new AzureDevOpsBugReporting();
                IConnectionInfo connectionInfo = new ConnectionInfo(expectedUri,
                    new TeamProject(expectedProjectName, Guid.Empty),
                    new Team(expectedTeamName, Guid.Empty));
                BugInformation bugInfo = new BugInformation();

                Uri actualUri = bugReporting.CreateBugPreviewAsync(connectionInfo, bugInfo).Result;

                Assert.AreEqual(expectedUri, actualUri);
                Assert.AreEqual(expectedProjectName, actualProjectName);
                Assert.AreEqual(expectedTeamName, actualTeamName);
            }
        }
    }
}
