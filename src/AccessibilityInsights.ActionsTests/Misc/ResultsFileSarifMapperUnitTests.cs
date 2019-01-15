// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Contexts;
using AccessibilityInsights.Actions.Fakes;
using AccessibilityInsights.Actions.Misc;
using AccessibilityInsights.Actions.Misc.Fakes;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.HelpLinks;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Core.Results.Fakes;
using AccessibilityInsights.Desktop.Utility;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessibilityInsights.ActionsTests.Misc
{

    [TestClass]
    public class ResultsFileSarifMapperUnitTests
    {
        Guid guid = new Guid("a5091b5f-3aca-4455-8606-6ca02f57e8f9");
        const string ScreenshotKey = @"file:///C:/ScanOutput/image001.png";
        const string Key = @"file:///C:/ScanOutput/result01.a11ytest";
        const string SystemPath = @"C:\Documents";
        private const string TestRuleName = "TestRule";
        private const string TestMessage = "Test Message";
        private const string FrameworkKey = "FrameworkId";
        private const string FrameworkValue = "WPF";
        private const string Standard = "4_1_2";
        private const string StandardsKey = "standards";
        private const string ScreenShotKey = "screenshot";
        private const string ToolOutputKey = "toolOutput";
        private const string ScreenShotPathValue = @"D:\\Path\\To\\ScreenShot";
        private const string ToolOutputPathValue = @"D:\\Path\\To\\ToolOutput";
        private const string Name = "Accessibility Insights for Windows";
        private const string RuleDescription = "Rule Description";
        private const string Source = "[4.1.2]";
        private const string TestHelpURL = "https://www.bing.com/";
        private const string TestGlimpse = "Testing";
        static string ScreenshotTemplateValue = @"file:///C:/ScanOutput/{fileGUID}.png";
        static string ToolOutputTemplateValue = @"file:///C:/ScanOutput/{fileGUID}.a11ytest";

        [TestMethod]
        [Timeout(1000)]
        public void GenerateEmbeddedFiles_ReceivesExpectedParameters()
        {
            Boolean resultsKey = false;
            Boolean imageResultKey= false;
            FileData resultsContent = null;
            FileData screenshotContent = null;

            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.GetScreenShotDataGuid = (ecID) =>
                {
                    return GenerateFileData(ScreenshotKey);
                };

                ShimResultsFileSarifMapper.GenerateResultsDataStringBoolean = (path, deleteA11yTestFile) =>
                {
                    return GenerateFileData(Key);
                };

                string runGuid = Guid.NewGuid().ToString();
                Dictionary<string, FileData> result = ResultsFileSarifMapper.GenerateEmbeddedFiles(null, new Guid(), false, runGuid);

                foreach (string key in result.Keys)
                {
                    if (!resultsKey)
                    {
                        resultsKey = key.Equals(ToolOutputTemplateValue.Replace("{fileGUID}", runGuid)) && result.TryGetValue(key, out resultsContent);
                    }

                    if (!imageResultKey)
                    {
                        imageResultKey = key.Equals(ScreenshotTemplateValue.Replace("{fileGUID}", runGuid)) && result.TryGetValue(key, out screenshotContent);
                    }
                }

                Assert.IsTrue(resultsKey, "Does not contain the result key / value or correct guid");
                Assert.IsTrue(imageResultKey, "Does not contain the imageResultKey key / value or correct guid");

                Assert.AreEqual(ScreenshotKey, Encoding.Default.GetString((Convert.FromBase64String(screenshotContent.Contents.Binary))));
                Assert.AreEqual(Key, Encoding.Default.GetString((Convert.FromBase64String(resultsContent.Contents.Binary))));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void TraverseTreeDepthFirst_NoScanResultsInElement_EmptyResult()
        {
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.GetScanResultsA11yElementString = (_, __) =>
                {
                    return new List<Tuple<Rule, Result>>();
                };

                ShimDataManager.GetDefaultInstance = () =>
                {
                    ShimDataManager shimDM = new ShimDataManager
                    {
                        GetElementContextGuid = (_) =>
                        {
                            return new ElementContext(new A11yElement());
                        }
                    };
                    return shimDM;
                };

                List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.TraverseTreeDepthFirst(guid);
                Assert.IsNotNull(elementResults);
                Assert.AreEqual(0, elementResults.Count);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void TraverseTreeDepthFirst_ElementContextNull_EmptyResult()
        {
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.GetScanResultsA11yElementString = (_, __) =>
                {
                    return new List<Tuple<Rule, Result>>();
                };

                ShimDataManager.GetDefaultInstance = () =>
                {
                    ShimDataManager shimDM = new ShimDataManager
                    {
                        GetElementContextGuid = (_) =>
                        {
                            return new ElementContext(new A11yElement());
                        }
                    };
                    return shimDM;
                };

                List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.TraverseTreeDepthFirst(guid);
                Assert.IsNotNull(elementResults);
                Assert.AreEqual(0, elementResults.Count);
            }
        }

        [TestMethod]
        [Timeout(2000)]
        public void TraverseTreeDepthFirst_ReceivesRequiredParameters_NonEmptyResult()
        {
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.GetScanResultsA11yElementString = (_, __) =>
                {
                    List<Tuple<Rule, Result>> scanResults = new List<Tuple<Rule, Result>>();
                    Rule rule = GenerateRule();
                    Result result = GenerateResult();
                    scanResults.Add(new Tuple<Rule, Result>(rule, result));
                    return scanResults;
                };

                ShimDataManager.GetDefaultInstance = () =>
                {
                    ShimDataManager shimDM = new ShimDataManager
                    {
                        GetElementContextGuid = (_) =>
                        {
                            A11yElement element = GenerateA11yElement();
                            A11yElement childElement = new A11yElement();
                            element.Children.Add(childElement);
                            return new ElementContext(element);
                        }
                    };
                    return shimDM;
                };

                List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.TraverseTreeDepthFirst(guid);

                Assert.IsNotNull(elementResults);
                Assert.AreEqual(2, elementResults.Count);
                Tuple<Rule, Result> elementResult = elementResults[0];
                Assert.IsNotNull(elementResult);
                Assert.IsNotNull(elementResult.Item1);
                Assert.IsNotNull(elementResult.Item2);
                Assert.AreEqual(TestRuleName, elementResult.Item1.Name.Text);
                Assert.AreEqual(ResultLevel.Open, elementResult.Item2.Level);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_A11yElementNull_EmptyResult()
        {
            List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(null, String.Empty);
            Assert.IsNotNull(elementResults);
            Assert.AreEqual(0, elementResults.Count);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_A11yElementScanResultsNull_EmptyResult()
        {
            A11yElement element = GenerateA11yElement();
            element.ScanResults = null;
            List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);
            Assert.IsNotNull(elementResults);
            Assert.AreEqual(0, elementResults.Count);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_A11yElementScanResultsItemEmpty_EmptyResult()
        {
            A11yElement element = GenerateA11yElement();
            List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);
            Assert.IsNotNull(elementResults);
            Assert.AreEqual(0, elementResults.Count);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_NullScanResultUnderItems_EmptyResult()
        {
            // The structure is a little confusing. ScanResults has a list of Items (RuleResults)
            A11yElement element = GenerateA11yElement();
            element.ScanResults.Items.Add(null);
            List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);
            Assert.IsNotNull(elementResults);
            Assert.AreEqual(0, elementResults.Count);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_NullRuleResultUnderScanItems_EmptyResult()
        {
            // The structure is a little confusing. ScanResults has a list of Items (RuleResults)
            A11yElement element = GenerateA11yElement();
            element.ScanResults.Items[0].Items.Add(null);
            List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);
            Assert.IsNotNull(elementResults);
            Assert.AreEqual(0, elementResults.Count);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_NullRuleResultListUnderScanItems_EmptyResult()
        {
            // The structure is a little confusing. ScanResults has a list of Items (RuleResults)
            A11yElement element = GenerateA11yElement();
            element.ScanResults.Items[0].Items = null;
            List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);
            Assert.IsNotNull(elementResults);
            Assert.AreEqual(0, elementResults.Count);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_NoRuleResults_EmptyResult()
        {
            A11yElement element = GenerateA11yElement();
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.FetchOrAddRuleRuleResult = (_) =>
                {
                    return GenerateRule();
                };

                ShimResultsFileSarifMapper.GetFingerPrintContributionsA11yElementRuleResult = (_, __) =>
                {
                    Dictionary<string, string> fingerPrintContributions = new Dictionary<string, string>();
                    fingerPrintContributions.Add(FrameworkKey, FrameworkValue);
                    fingerPrintContributions.Add("Level", "open");
                    fingerPrintContributions.Add("LocalizedControlType", "page");
                    fingerPrintContributions.Add("Name", "Welcome page");
                    fingerPrintContributions.Add("RuleId", "BoundingRectangleSizeReasonable");
                    return fingerPrintContributions;
                };

                ShimResultsFileSarifMapper.GetResultMessagesListOfString = (_) =>
                {
                    return new Message(TestMessage, null, TestMessage, null, null, null);
                };
                List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);
                Assert.IsNotNull(elementResults);
                Assert.AreEqual(0, elementResults.Count);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetScanResults_ReceivesNormalParameters_ProducesScanResultList()
        {
            A11yElement element = GenerateA11yElement();
            RuleResult result = new RuleResult();
            List<RuleResult> ruleResults = new List<RuleResult>();
            ruleResults.Add(result);
            element.ScanResults.Items[0].Items = ruleResults;
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.FetchOrAddRuleRuleResult = (_) =>
                {
                    return GenerateRule();
                };

                ShimScanStatusExtensions.ToResultLevelScanStatus = (_) =>
                {
                    return ResultLevel.Open;
                };

                ShimResultsFileSarifMapper.GetFingerPrintContributionsA11yElementRuleResult = (_, __) =>
                {
                    Dictionary<string, string> fingerPrintContributions = new Dictionary<string, string>();
                    fingerPrintContributions.Add(FrameworkKey, FrameworkValue);
                    fingerPrintContributions.Add("Level", "open");
                    fingerPrintContributions.Add("LocalizedControlType", "page");
                    fingerPrintContributions.Add("Name", "Welcome page");
                    fingerPrintContributions.Add("RuleId", "BoundingRectangleSizeReasonable");
                    return fingerPrintContributions;
                };

                ShimResultsFileSarifMapper.GetResultMessagesListOfString = (_) =>
                {
                    return new Message(TestMessage, null, null, null, null, null);
                };
                List<Tuple<Rule, Result>> elementResults = ResultsFileSarifMapper.GetScanResults(element, String.Empty);

                Assert.IsNotNull(elementResults);
                Assert.AreEqual(1, elementResults.Count);
                Tuple<Rule, Result> elementScanOutput = elementResults[0];
                Assert.IsNotNull(elementScanOutput);

                Rule rule = elementScanOutput.Item1;
                Result res = elementScanOutput.Item2;
                Assert.IsNotNull(rule);
                Assert.IsNotNull(res);

                Assert.AreEqual(TestRuleName, rule.Name.Text);
                Assert.AreEqual(ResultLevel.Open, res.Level);

                Assert.IsNotNull(res.PartialFingerprints);
                Assert.IsTrue(res.PartialFingerprints.ContainsKey(FrameworkKey));
                string partialfingerprintFramework = String.Empty;
                Assert.IsTrue(res.PartialFingerprints.TryGetValue(FrameworkKey, out partialfingerprintFramework));
                Assert.AreEqual(FrameworkValue, partialfingerprintFramework);
                Assert.AreEqual(res.Locations[0].Annotations[0].Snippet.Text, TestGlimpse);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void FetchOrAddRule_RuleExists_ReturnRule()
        {
            RuleResult ruleResult = GenerateRuleResult();

            Rule sarifRule = GenerateRuleFromRuleResult(ruleResult);

            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.FetchOrAddStandardsRuleResult = (_) =>
                {
                    return new string[] { Standard };
                };

                Rule returnedRule = ResultsFileSarifMapper.FetchOrAddRule(ruleResult);

                AssertRulesEqual(sarifRule, returnedRule);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void FetchOrAddRule_NewRule_ReturnRule()
        {
            RuleResult ruleResult = GenerateRuleResult();
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.FetchOrAddStandardsRuleResult = (_) =>
                {
                    return new string[] { Standard };
                };

                Rule addedRule = GenerateRuleFromRuleResult(ruleResult);

                Rule returnedRule = ResultsFileSarifMapper.FetchOrAddRule(ruleResult);

                AssertRulesEqual(addedRule, returnedRule);

                Assert.IsTrue(ResultsFileSarifMapper.RuleList.ContainsKey(ruleResult.Rule.ToString()));
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void FetchOrAddRule_NullRuleResult_ReturnRule()
        {
            RuleResult ruleResult = GenerateRuleResult();
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.FetchOrAddStandardsRuleResult = (_) =>
                {
                    return new string[] { Standard };
                };

                Rule returnedRule = ResultsFileSarifMapper.FetchOrAddRule(null);
                Assert.IsNull(returnedRule);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GenerateAndAddRule_ReceivesAllParameters_AddsRule()
        {
            ResultsFileSarifMapper.RuleList.Clear();
            RuleResult ruleResult = GenerateRuleResult();
            using (ShimsContext.Create())
            {
                ShimResultsFileSarifMapper.FetchOrAddStandardsRuleResult = (_) =>
                {
                    return new string[] { Standard };
                };
                Rule returnedRule = ResultsFileSarifMapper.GenerateAndAddRule(ruleResult);
                Assert.IsNotNull(returnedRule);
                Assert.IsTrue(ResultsFileSarifMapper.RuleList.ContainsKey(ruleResult.Rule.ToString()));
                Assert.AreEqual(ruleResult.Rule.ToString(), returnedRule.Id);
                Assert.AreEqual(ruleResult.Description, returnedRule.Name.Text);
                Assert.AreEqual(ruleResult.Description, returnedRule.FullDescription.Text);
                Assert.AreEqual(ruleResult.HelpUrl.Url, returnedRule.HelpUri.ToString());
                string[] standards = null;
                Assert.IsTrue(returnedRule.TryGetProperty<string[]>(StandardsKey, out standards));
                Assert.AreEqual(Standard, standards[0]);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void FetchOrAddStandard_StandardExists_ReturnStandard()
        {
            RuleResult ruleResult = GenerateRuleResult();
            string[] addedStandard = ResultsFileSarifMapper.FetchOrAddStandards(ruleResult);
            string[] retrievedStandards = ResultsFileSarifMapper.FetchOrAddStandards(ruleResult);
            Assert.AreEqual(addedStandard[0], retrievedStandards[0]);
            string standardKey = "4.1.2";
            Assert.IsTrue(ResultsFileSarifMapper.A11yCriteriaList.ContainsKey(standardKey));
        }

        [TestMethod]
        [Timeout(1000)]
        public void FetchOrAddStandard_NewStandard_ReturnStandard()
        {
            RuleResult ruleResult = GenerateRuleResult();
            string[] addedStandard = ResultsFileSarifMapper.FetchOrAddStandards(ruleResult);
            string standardKey = "4.1.2";
            Assert.IsTrue(ResultsFileSarifMapper.A11yCriteriaList.ContainsKey(standardKey));
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetResultAttachments_ReceivesValidParameters_GeneratesAttachmentList()
        {
            List<string> fileKeyList = new List<string>() { ScreenShotPathValue, ToolOutputPathValue };
            List<Attachment> attachments = ResultsFileSarifMapper.GetResultAttachments(fileKeyList);
            Assert.IsNotNull(attachments);
            Assert.AreEqual(2, attachments.Count);
            Assert.AreEqual(ScreenShotKey, attachments[0].Description.Text);
            Assert.AreEqual(ToolOutputKey, attachments[1].Description.Text);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GenerateInvocationInfo_ReceivesValidParameters_GeneratesInvocationInfo()
        {
            using (ShimsContext.Create())
            {
                DateTime expectedDate = new DateTime(1989, 9, 11);
                System.Fakes.ShimDateTime.UtcNowGet = () => expectedDate;
                List<Invocation> invocations = ResultsFileSarifMapper.GenerateInvocationInfo();
                Invocation invocationOfInterest = invocations[0];
                Assert.AreEqual(invocationOfInterest.EndTimeUtc.Ticks, expectedDate.Ticks);
                Assert.AreEqual(invocationOfInterest.StartTimeUtc.Ticks, expectedDate.Ticks);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GenerateToolInfo_ReceivesValidParameters_GeneratesToolInfo()
        {
            Tool toolInfo = ResultsFileSarifMapper.GenerateToolInfo();
            Assert.AreEqual(Name, toolInfo.Name);
            Assert.IsNotNull(toolInfo.SemanticVersion);
            Assert.IsNotNull(toolInfo.Version);
        }

        [TestMethod]
        [Timeout(1000)]
        public void GenerateAndPersistSarifFile_ReceivesNormalParameters_WritesToFile()
        {
            using (ShimsContext.Create())
            {
                Boolean fileWriteInvoked = false;

                System.IO.Fakes.ShimFile.WriteAllTextStringString = (_, __) =>
                {
                    fileWriteInvoked = true;
                };

                ShimResultsFileSarifMapper.GenerateEmbeddedFilesStringGuidBooleanString= (_, __, ___, ____) =>
                {
                    return new Dictionary<string, FileData>()
                    {
                        { ScreenShotPathValue, new FileData() },
                        { ToolOutputPathValue, new FileData() }
                    };
                };

                ShimResultsFileSarifMapper.TraverseTreeDepthFirstGuid = (_) =>
                {
                    List<Tuple<Rule, Result>> elementResults = new List<Tuple<Rule, Result>>();
                    Tuple<Rule, Result> resultTuple = new Tuple<Rule, Result>(GenerateRule(), GenerateResult());
                    elementResults.Add(resultTuple);
                    return elementResults;
                };

                Microsoft.CodeAnalysis.Sarif.Writers.Fakes.ShimSarifLogger.AllInstances.LogIRuleResult = (_, __, ___) => { };
                ResultsFileSarifMapper.GenerateAndPersistSarifFile(SystemPath, guid, false);
                Assert.IsTrue(fileWriteInvoked);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GenerateAndPersistSarifFile_NoResults_WritesToFile()
        {
            using (ShimsContext.Create())
            {
                Boolean fileWriteInvoked = false;

                System.IO.Fakes.ShimFile.WriteAllTextStringString = (_, __) =>
                {
                    fileWriteInvoked = true;
                };

                ShimResultsFileSarifMapper.GenerateEmbeddedFilesStringGuidBooleanString = (_, __, ___,____) =>
                {
                    return new Dictionary<string, FileData>()
                    {
                        { ScreenShotPathValue, new FileData() },
                        { ToolOutputPathValue, new FileData() }
                    };
                };

                ShimResultsFileSarifMapper.TraverseTreeDepthFirstGuid = (_) =>
                {
                    return new List<Tuple<Rule, Result>>(); ;
                };

                Microsoft.CodeAnalysis.Sarif.Writers.Fakes.ShimSarifLogger.AllInstances.LogIRuleResult = (_, __, ___) => { };
                ResultsFileSarifMapper.GenerateAndPersistSarifFile(SystemPath, guid, false);
                Assert.IsTrue(fileWriteInvoked);
            }
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetResultMessages_ReceivesNormalParameters_ProducesMessages()
        {
            List<string> messages = new List<string>();
            string messageOne = "one";
            string messageTwo = "two";
            string messageThree = "three";

            messages.Add(messageOne);
            messages.Add(messageTwo);
            messages.Add(messageThree);

            string expectedMessage = messageOne + " " + messageTwo + " " + messageThree;
            Message sarifMessage = ResultsFileSarifMapper.GetResultMessages(messages);
            Assert.AreEqual(expectedMessage, sarifMessage.Text.Trim());
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetResultMessages_EmptyMessageList_ProducesMessages()
        {
            List<string> messages = new List<string>();
            Message sarifMessage = ResultsFileSarifMapper.GetResultMessages(messages);
            string returnedMessage = sarifMessage.Text.Trim();
            Assert.IsTrue(string.IsNullOrEmpty(returnedMessage));
        }

        [TestMethod]
        [Timeout(1000)]
        public void GetResultMessages_NullMessages_ProducesMessages()
        {
            Message sarifMessage = ResultsFileSarifMapper.GetResultMessages(null);
            Assert.IsTrue(string.IsNullOrEmpty(sarifMessage.Text));
        }

        private static Result GenerateResult()
        {
            Result result = new Result();
            result.Level = ResultLevel.Open;
            return result;
        }

        private static Rule GenerateRule()
        {
            Rule rule = new Rule();
            rule.Name = new Message(TestRuleName, null, null, null, null, null);
            return rule;
        }

        private static RuleResult GenerateRuleResult()
        {
            RuleResult ruleResult = new RuleResult();
            ruleResult.Rule = Core.Enums.RuleId.BoundingRectangleContainedInParent;
            ruleResult.Description = RuleDescription;
            HelpUrl helpUrl = new HelpUrl();
            helpUrl.Url = TestHelpURL;
            ruleResult.HelpUrl = helpUrl;
            ruleResult.Source = Source;
            return ruleResult;
        }

        private static A11yElement GenerateA11yElement()
        {
            A11yElement element = new A11yElement();
            element.Glimpse = TestGlimpse;
            element.Children = new List<A11yElement>();
            element.ScanResults = new ScanResults();
            ScanResult scanResult = new ScanResult();
            scanResult.Items = new List<RuleResult>();
            element.ScanResults.Items.Add(scanResult);
            return element;
        }

        private static FileData GenerateFileData(string contents)
        {
            FileData fileData = new FileData();
            FileContent fileContent = new FileContent();
            fileContent.Binary = Convert.ToBase64String(Encoding.ASCII.GetBytes(contents));
            fileData.Contents = fileContent;
            return fileData;
        }

        private static void AssertRulesEqual(Rule sarifRule, Rule returnedRule)
        {
            Assert.AreEqual(sarifRule.HelpUri, returnedRule.HelpUri);
            Assert.AreEqual(sarifRule.Id, returnedRule.Id);
            Assert.AreEqual(sarifRule.Name.Text, returnedRule.Name.Text);
            Assert.AreEqual(sarifRule.Name.RichText, returnedRule.Name.RichText);
            Assert.AreEqual(sarifRule.FullDescription.Text, returnedRule.FullDescription.Text);
            Assert.AreEqual(sarifRule.FullDescription.RichText, returnedRule.FullDescription.RichText);
        }

        private static Rule GenerateRuleFromRuleResult(RuleResult ruleResult)
        {
            Rule rule = new Rule();
            rule.Id = ruleResult.Rule.ToString();
            rule.Name = new Message(ruleResult.Description, null, ruleResult.Description, null, null, null);
            rule.FullDescription = new Message(ruleResult.Description, null, ruleResult.Description, null, null, null);
            rule.HelpUri = ruleResult.HelpUrl?.Url.ToUri();
            return rule;
        }
    }
}
