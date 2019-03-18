// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.AzureDevOps.Fakes;
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.AzureDevOps.Models;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests
{
    [TestClass]
    public class AzureBoardsIssueReportingTests
    {
        private const string TestMessage = "Test message";
        private const string RuleDescription = "Rule Description";


        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringIsNull_ReturnsNull()
        {
            const string originalString = null;
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.IsNull(result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringIsWithinLimit_ReturnsOriginalString()
        {
            const string originalString = "abc";
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringExceedsLimit_ReturnsTruncatedOriginalStringbWithSuffix()
        {
            const string originalString = "abcd";
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.AreEqual("abc!!!", result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringIsNull_ReturnsNull()
        {
            const string originalString = null;
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.IsNull(result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasNoLeadingBracket_ReturnsOriginalString()
        {
            const string originalString = "abc]";
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasNoTrailingBracket_ReturnsOriginalString()
        {
            const string originalString = "[abc";
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasLeadingAndTrailingBrackets_ReturnsSubstring()
        {
            const string originalString = "[abc]";
            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            string result = privateFileIssueAction.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_AllFieldsNull_AddsNullValues()
        {
            Dictionary<IssueField, string> issueFieldPairs = new Dictionary<IssueField, string>();
            IssueInformation issueInfo = new IssueInformation();

            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            privateFileIssueAction.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

            Assert.AreEqual(4, issueFieldPairs.Count);
            Assert.IsNull(issueFieldPairs[IssueField.ProcessName]);
            Assert.IsNull(issueFieldPairs[IssueField.Glimpse]);
            Assert.IsNull(issueFieldPairs[IssueField.TestMessages]);
            Assert.IsNull(issueFieldPairs[IssueField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_ProcessNameIsSpecified_AddsProcessNameValue()
        {
            string originalProcessName = new string('x', 60);
            Dictionary<IssueField, string> issueFieldPairs = new Dictionary<IssueField, string>();
            IssueInformation issueInfo = new IssueInformation(processName: originalProcessName);

            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            privateFileIssueAction.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

            Assert.AreEqual(4, issueFieldPairs.Count);
            string modifiedProcessName = issueFieldPairs[IssueField.ProcessName];
            Assert.AreEqual(new string('x', 50) + ".exe", modifiedProcessName);
            Assert.IsNull(issueFieldPairs[IssueField.Glimpse]);
            Assert.IsNull(issueFieldPairs[IssueField.TestMessages]);
            Assert.IsNull(issueFieldPairs[IssueField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_GlimpseIsSpecified_AddsGlimpseValue()
        {
            string originalGlimpse = new string('y', 60);
            Dictionary<IssueField, string> issueFieldPairs = new Dictionary<IssueField, string>();
            IssueInformation issueInfo = new IssueInformation(glimpse: originalGlimpse);

            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            privateFileIssueAction.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

            Assert.AreEqual(4, issueFieldPairs.Count);
            string modifiedGlimpse = issueFieldPairs[IssueField.Glimpse];
            Assert.AreEqual(new string('y', 50) + "...", modifiedGlimpse);
            Assert.IsNull(issueFieldPairs[IssueField.ProcessName]);
            Assert.IsNull(issueFieldPairs[IssueField.TestMessages]);
            Assert.IsNull(issueFieldPairs[IssueField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_TestMessagesIsSpecified_AddsTestMessagesValue()
        {
            string originalTestMessages = new string('z', 200);
            Dictionary<IssueField, string> issueFieldPairs = new Dictionary<IssueField, string>();
            IssueInformation issueInfo = new IssueInformation(testMessages: originalTestMessages);

            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            privateFileIssueAction.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

            Assert.AreEqual(4, issueFieldPairs.Count);
            string modifiedTestMessages = issueFieldPairs[IssueField.TestMessages];
            Assert.AreEqual(new string('z', 150) + "...open attached", modifiedTestMessages.Substring(0, 166));
            Assert.IsNull(issueFieldPairs[IssueField.ProcessName]);
            Assert.IsNull(issueFieldPairs[IssueField.Glimpse]);
            Assert.IsNull(issueFieldPairs[IssueField.RuleSource]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_RuleSourceIsSpecified_AddsRuleSourceValue()
        {
            const string ruleSourceContent = "weather: cold and clear";

            string originalRuleSource = '[' + ruleSourceContent + ']';
            Dictionary<IssueField, string> issueFieldPairs = new Dictionary<IssueField, string>();
            IssueInformation issueInfo = new IssueInformation(ruleSource: originalRuleSource);

            PrivateType privateFileIssueAction = new PrivateType(typeof(FileIssueAction));
            privateFileIssueAction.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

            Assert.AreEqual(4, issueFieldPairs.Count);
            string modifiedRuleSource = issueFieldPairs[IssueField.RuleSource];
            Assert.AreEqual(ruleSourceContent, modifiedRuleSource);
            Assert.IsNull(issueFieldPairs[IssueField.ProcessName]);
            Assert.IsNull(issueFieldPairs[IssueField.Glimpse]);
            Assert.IsNull(issueFieldPairs[IssueField.TestMessages]);
        }

        [TestMethod]
        [Timeout(1000)]
        public void CreateIssuePreviewAsync_TeamNameIsNull_ChainsThroughCorrectly()
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
                    CreateIssuePreviewStringStringIReadOnlyDictionaryOfAzureDevOpsFieldString = (p, t, f) =>
                    {
                        actualProjectName = p;
                        actualTeamName = t;
                        return expectedUri;
                    },
                    ConnectedToAzureDevOpsGet = () => true,
                };

                ShimAzureDevOpsIntegration.GetCurrentInstance = () => integration;

                ConnectionInfo connectionInfo = new ConnectionInfo(expectedUri,
                    new TeamProject(expectedProjectName, Guid.Empty),
                    new Team(expectedTeamName, Guid.Empty));
                IssueInformation issueInfo = new IssueInformation();

                Uri actualUri = FileIssueAction.CreateIssuePreviewAsync(connectionInfo, issueInfo).Result;

                Assert.AreEqual(expectedUri, actualUri);
                Assert.AreEqual(expectedProjectName, actualProjectName);
                Assert.IsNull(actualTeamName);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void CreateIssuePreviewAsync_TeamNameIsNotNull_ChainsThroughCorrectly()
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
                    CreateIssuePreviewStringStringIReadOnlyDictionaryOfAzureDevOpsFieldString = (p, t, f) =>
                    {
                        actualProjectName = p;
                        actualTeamName = t;
                        return expectedUri;
                    },
                    ConnectedToAzureDevOpsGet = () => true,
                };

                ShimAzureDevOpsIntegration.GetCurrentInstance = () => integration;

                ConnectionInfo connectionInfo = new ConnectionInfo(expectedUri,
                    new TeamProject(expectedProjectName, Guid.Empty),
                    new Team(expectedTeamName, Guid.Empty));
                IssueInformation issueInfo = new IssueInformation();

                Uri actualUri = FileIssueAction.CreateIssuePreviewAsync(connectionInfo, issueInfo).Result;

                Assert.AreEqual(expectedUri, actualUri);
                Assert.AreEqual(expectedProjectName, actualProjectName);
                Assert.AreEqual(expectedTeamName, actualTeamName);
            }
        }
    }
}
