﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps;
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.AzureDevOps.FileIssue;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace AccessibilityInsights.Extensions.AzureDevOpsTests.FileIssue
{
    [TestClass]
    public class FileIssueHelpersTests
    {
        const int TestIssueId = 99999;
        const string TestProjectName = "ProjectName";
        const string TestIteration = "Some Iteration";
        const string TestTeamName = "Some Team";
        const string TestArea = "Some Area";
        const string TestIssueGuidString = "73e16008-4166-4305-b9ca-5621a38e04d0";
        static readonly Guid testIssueGuid = new Guid(TestIssueGuidString);

        Mock<IDevOpsIntegration> _adoIntegrationMock;
        FileIssueHelpers _fileIssueHelpers;

        [TestInitialize]
        public void BeforeEach()
        {
            _adoIntegrationMock = new Mock<IDevOpsIntegration>(MockBehavior.Strict);
            _fileIssueHelpers = new FileIssueHelpers(_adoIntegrationMock.Object);
        }

        [TestMethod]
        [Timeout(2000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateIssuePreviewAsync_IssueInfoIsNull_ThrowsArgumentNullException()
        {
            _fileIssueHelpers.CreateIssuePreviewAsync(new ConnectionInfo(), null);
        }

        [TestMethod]
        [Timeout(10000)]
        public void RemoveInternalFromIssueText_MatchingTextExists()
        {
            var guid = Guid.NewGuid().ToString();
            // Internal id doesn't exist if the text is modified by user in edit pane. this scenario simulate the case.
            string original = $"<br><br><div><hr>{guid}<hr></div>";
            string expected = "<br><br><div></div>";

            Assert.AreEqual(expected, FileIssueHelpers.RemoveInternalHTML(original, guid));
        }

        [TestMethod]
        [Timeout(10000)]
        public void RemoveInternalFromIssueText_NoMatchingText()
        {
            var guid = Guid.NewGuid().ToString();

            string original = "<br><br><div><hr>should not be removed<hr></div>";
            string expected = original;

            Assert.AreEqual(expected, FileIssueHelpers.RemoveInternalHTML(original, guid));
        }

        [TestMethod]
        [Timeout(1000)]
        public void WrapInHtmlBody_AddsExtraData()
        {
            string original = "<br><br><div><hr>This is the original text<hr></div>";
            string expected = "<body>" + original + "</body>";

            Assert.AreEqual(expected, FileIssueHelpers.WrapInHtmlBody(original));
        }

        [TestMethod]
        [Timeout(1000)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FileNewIssue_FileIssueIsNull_ThrowsArgumentNullException()
        {
            _fileIssueHelpers.FileNewIssue(null, new ConnectionInfo(), false, 100, (unused) => { Assert.Fail("This method should never be called"); }, "sample_configuration_path");
        }

        [TestMethod]
        [Timeout(1000)]
        public void FileNewIssue_GetAreaPathReturnsNull_ReturnsPlaceholder()
        {
            var connInfo = new ConnectionInfo(new Uri("https://accessibilityinsights.io"),
                new AzureDevOps.Models.TeamProject(TestProjectName, Guid.Empty), null);
            _adoIntegrationMock.Setup(x => x.GetAreaPath(connInfo))
                .Returns<string>(null);
            _adoIntegrationMock.Setup(x => x.GetIteration(connInfo))
                .Returns(TestIteration);

            var configPath = "placeholderConfigurationPath";

            var issueInfo = new IssueInformation(internalGuid: testIssueGuid);
            var output = _fileIssueHelpers.FileNewIssue(issueInfo,
                connInfo, false, 0, (_) => { }, configPath);

            Assert.IsNull(output.issueId);
            Assert.AreEqual(string.Empty, output.newIssueId);
            _adoIntegrationMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FileNewIssue_GetIterationReturnsNull_ReturnsPlaceholder()
        {
            var connInfo = new ConnectionInfo(new Uri("https://accessibilityinsights.io"),
                new AzureDevOps.Models.TeamProject(TestProjectName, Guid.Empty), null);
            _adoIntegrationMock.Setup(x => x.GetAreaPath(connInfo))
                .Returns(TestArea);
            _adoIntegrationMock.Setup(x => x.GetIteration(connInfo))
                .Returns<string>(null);

            var issueInfo = new IssueInformation(internalGuid: testIssueGuid);
            var configPath = "placeholderConfigurationPath";

            var output = _fileIssueHelpers.FileNewIssue(issueInfo,
                connInfo, false, 0, (_) => { }, configPath);

            Assert.IsNull(output.issueId);
            Assert.AreEqual(string.Empty, output.newIssueId);
            _adoIntegrationMock.VerifyAll();
        }

        [TestMethod]
        [Timeout(1000)]
        public void FileNewIssue_FieldsAreValid_ReturnsExpectedData()
        {
            AzureDevOps.Models.AdoTeam expectedAdoTeam = new AzureDevOps.Models.AdoTeam(TestTeamName, Guid.Empty);
            IReadOnlyDictionary<AzureDevOpsField, string> actualFields = null;

            var connInfo = new ConnectionInfo(new Uri("https://accessibilityinsights.io"),
                new AzureDevOps.Models.TeamProject(TestProjectName, Guid.Empty), expectedAdoTeam);
            _adoIntegrationMock.Setup(x => x.GetAreaPath(connInfo))
                .Returns(TestArea);
            _adoIntegrationMock.Setup(x => x.GetIteration(connInfo))
                .Returns(TestIteration);
            _adoIntegrationMock.Setup(x => x.CreateIssuePreview(TestProjectName, TestTeamName, It.IsAny<IReadOnlyDictionary<AzureDevOpsField, string>>()))
                .Callback<string, string, IReadOnlyDictionary<AzureDevOpsField, string>>((_, __, fields) => actualFields = fields)
                .Returns(new Uri("https://www.bing.com"));

            var issueInfo = new IssueInformation(internalGuid: testIssueGuid);
            var configPath = "placeholderConfigurationPath";

            var output = _fileIssueHelpers.FileNewIssueTestable(issueInfo,
                connInfo, false, 0, (_) => { }, configPath, TestIssueId);

            Assert.AreEqual(TestIssueId, output.issueId);
            Assert.IsNotNull(output.newIssueId);
            Assert.AreEqual(TestIssueGuidString, output.newIssueId);
            Assert.AreEqual(5, actualFields.Count);
            Assert.IsFalse(string.IsNullOrEmpty(actualFields[AzureDevOpsField.Title]));
            Assert.IsFalse(string.IsNullOrEmpty(actualFields[AzureDevOpsField.Tags]));
            Assert.IsFalse(string.IsNullOrEmpty(actualFields[AzureDevOpsField.ReproSteps]));
            Assert.AreEqual(TestArea, actualFields[AzureDevOpsField.AreaPath]);
            Assert.AreEqual(TestIteration, actualFields[AzureDevOpsField.IterationPath]);
            _adoIntegrationMock.VerifyAll();
        }


        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringIsNull_ReturnsNull()
        {
            const string originalString = null;
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.IsNull(result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringIsWithinLimit_ReturnsOriginalString()
        {
            const string originalString = "abc";
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateString_OriginalStringExceedsLimit_ReturnsTruncatedOriginalStringbWithSuffix()
        {
            const string originalString = "abcd";
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("TruncateString", originalString, 3, "!!!") as string;
            Assert.AreEqual("abc!!!", result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringIsNull_ReturnsNull()
        {
            const string originalString = null;
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.IsNull(result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasNoLeadingBracket_ReturnsOriginalString()
        {
            const string originalString = "abc]";
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasNoTrailingBracket_ReturnsOriginalString()
        {
            const string originalString = "[abc";
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual(originalString, result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void RemoveSurroundingBrackets_OriginalStringHasLeadingAndTrailingBrackets_ReturnsSubstring()
        {
            const string originalString = "[abc]";
            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            string result = privateFileIssueHelpers.InvokeStatic("RemoveSurroundingBrackets", originalString) as string;
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        [Timeout(1000)]
        public void TruncateSelectedFields_AllFieldsNull_AddsNullValues()
        {
            Dictionary<IssueField, string> issueFieldPairs = new Dictionary<IssueField, string>();
            IssueInformation issueInfo = new IssueInformation();

            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            privateFileIssueHelpers.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

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

            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            privateFileIssueHelpers.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

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

            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            privateFileIssueHelpers.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

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

            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            privateFileIssueHelpers.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

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

            PrivateType privateFileIssueHelpers = new PrivateType(typeof(FileIssueHelpers));
            privateFileIssueHelpers.InvokeStatic("TruncateSelectedFields", issueInfo, issueFieldPairs);

            Assert.AreEqual(4, issueFieldPairs.Count);
            string modifiedRuleSource = issueFieldPairs[IssueField.RuleSource];
            Assert.AreEqual(ruleSourceContent, modifiedRuleSource);
            Assert.IsNull(issueFieldPairs[IssueField.ProcessName]);
            Assert.IsNull(issueFieldPairs[IssueField.Glimpse]);
            Assert.IsNull(issueFieldPairs[IssueField.TestMessages]);
        }
    }
}
