// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Actions.Sarif;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Fingerprint;
using AccessibilityInsights.Core.Results;
using AccessibilityInsights.Desktop.Utility;
using Microsoft.CodeAnalysis.Sarif;
using Microsoft.CodeAnalysis.Sarif.Writers;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;

namespace AccessibilityInsights.Actions.Misc
{
    public static class ResultsFileSarifMapper
    {
        // The following two templates are meant to be templates to generate a unique key value pair per run and hold no significance beyond that
        static string ScreenshotTemplateValue = @"file:///C:/ScanOutput/{fileGUID}.png";
        static string ToolOutputTemplateValue = @"file:///C:/ScanOutput/{fileGUID}.a11ytest";

        const string ToolSemVer = "1.2.0";
        private const string StandardsKey = "standards";
        private const string ToolName = "Accessibility Insights for Windows";
        private const string DownloadURIKey = "downloadUri";
        private const string TagsKey = "tags";
        private const string AccessibilityKey = "accessibility";
        private const string ScreenShotRectangleKey = "screenshotRect";
        private const string ScreenShotKey = "screenshot";
        private const string ToolOutputKey = "toolOutput";
        private const string MimeType = "application/a11y";
        private const string ResultsTag = "results";
        private const string SarifExtension = ".Sarif";
        private const string ResultsExtension = ".a11ytest";
        private const string ImageMimeType = "image/png";
        private const string PathSeperator = @" / ";
        private const string UniqueIdKey = "uniqueId";
        private const string DownloadURI = @"https://go.microsoft.com/fwlink/?linkid=2077926";
        internal static Dictionary<string, Rule> RuleList = new Dictionary<string, Rule>();
        internal static Dictionary<string, A11yCriteria> A11yCriteriaList = new Dictionary<string, A11yCriteria>();

        /// <summary>
        /// Aggregator to generate all files that need to be embedded.
        /// </summary>
        /// <param name="path"> Path at which the generated results file exists </param>
        /// <param name="ecId"> Root element </param>
        /// <param name="deleteResultsFile">If true, will delete the results file at the end</param>
        /// <param name="fileGUID">Guid to use with the file</param>
        /// <returns> A dictionary of all the files that need to be embedded </returns>
        internal static Dictionary<string, FileData> GenerateEmbeddedFiles(string path, Guid ecId, Boolean deleteResultsFile, string fileGUID)
        {
            return new Dictionary<string, FileData>()
            {
                { ScreenshotTemplateValue.Replace("{fileGUID}", fileGUID), GetScreenShotData(ecId) },
                { ToolOutputTemplateValue.Replace("{fileGUID}", fileGUID), GenerateResultsData(path, deleteResultsFile) }
            };
        }

        /// <summary>
        /// Will read the results file at the path and will generate a FileData object that can be used in a Sariflog. Will delete the results file at that location after conversion
        /// </summary>
        /// <param name="path"> Location where the results file can be found</param>
        /// <param name="deleteFile"> Whether the results should be deleted</param>
        /// <returns> FileData with the results file contents embedded in Base64 encoded format </returns>
        internal static FileData GenerateResultsData(string path, bool deleteFile)
        {
            // Open the results and attach the file
            FileData fileData = new FileData();
            string[] tags = new string[] { ResultsTag };
            fileData.SetProperty<string[]>(TagsKey, tags);
            fileData.MimeType = MimeType;
            FileContent resultsFileContent = new FileContent();
            using (MemoryStream stream = new MemoryStream())
            {
                path = path.Replace(SarifExtension, ResultsExtension);
                FileStream str = File.Open(path, FileMode.Open);
                str.CopyTo(stream);
                str.Close();
                if (deleteFile)
                {
                    File.Delete(path);
                }

                byte[] resultFileBytes = stream.ToArray();
                string encodedResultsFile = Convert.ToBase64String(resultFileBytes);
                resultsFileContent.Binary = encodedResultsFile;
            }
            fileData.Contents = resultsFileContent;
            return fileData;
        }

        /// <summary>
        /// Helper function to serialize the image snapshot.
        /// </summary>
        /// <param name="ecId"> GUID to look up the Context that contains the screenshot </param>
        /// <returns></returns>
        internal static FileData GetScreenShotData(Guid ecId)
        {
            FileData snapShotFile = new FileData();

            snapShotFile.MimeType = ImageMimeType;
            FileContent snapShotFileContent = new FileContent();
            // Attach the screenshot file
            var ec = DataManager.GetDefaultInstance().GetElementContext(ecId);
            using (MemoryStream stream = new MemoryStream())
            {
                ec.DataContext.Screenshot.Save(stream, ImageFormat.Png);
                stream.Seek(0, SeekOrigin.Begin);

                byte[] screenShotBytes = stream.ToArray();
                string encodedScreenShot = Convert.ToBase64String(screenShotBytes);

                snapShotFileContent.Binary = encodedScreenShot;
            }
            snapShotFile.Contents = snapShotFileContent;
            return snapShotFile;
        }

        /// <summary>
        /// Traverses the hierarchy tree down from the given element. Generates the results for the current element and then traverses all its children in a depth first fashion.
        /// </summary>
        /// <param name="ecId"> GUID of the element the traversal should start from </param>
        /// <returns> List of <Rule, Result> tuples for all the scan results under the element with the passed in GUID</Rule></returns>
        internal static List<Tuple<Rule, Result>> TraverseTreeDepthFirst(Guid ecId)
        {
            var elementContext = DataManager.GetDefaultInstance().GetElementContext(ecId);
            Stack<Tuple<A11yElement, string>> traversalStack = new Stack<Tuple<A11yElement, string>>();
            string qualifiedPath = "";
            traversalStack.Push(new Tuple<A11yElement, string>(elementContext?.Element, qualifiedPath));
            A11yElement currElement = null;
            List<Tuple<Rule, Result>> elementResults = new List<Tuple<Rule, Result>>();
            while (traversalStack.Count != 0)
            {
                Tuple<A11yElement, string> currentElement = traversalStack.Pop();
                currElement = currentElement?.Item1;
                qualifiedPath = currentElement?.Item2 ?? "";
                qualifiedPath += PathSeperator;
                if (currElement?.Glimpse != null)
                {
                    qualifiedPath += currElement.Glimpse;
                }
                elementResults.AddRange(GetScanResults(currElement, qualifiedPath));
                foreach (A11yElement child in currElement?.Children ?? new List<A11yElement>())
                {
                    traversalStack.Push(new Tuple<A11yElement, string>(child, qualifiedPath));
                }
            }
            return elementResults;
        }

        /// <summary>
        /// Traverses the element scan results and massages them into the SARIF Rule, result format
        /// </summary>
        /// <param name="currElement"> The element whose scan results need to be aggregated and massaged</param>
        /// <param name="qualifiedPath"> The path till the parent of this element </param>
        /// <returns> Returns a list of Tuples of Rules run and the corresponding results for the current item </returns>
        internal static List<Tuple<Rule, Result>> GetScanResults(A11yElement currElement, string qualifiedPath)
        {
            List<Tuple<Rule, Result>> elementScanOutput = new List<Tuple<Rule, Result>>();

            // All the stuff shared across all the results under one element
            if (currElement?.ScanResults?.Items != null && currElement.ScanResults.Items.Count != 0)
            {
                List<Location> location = GenerateLocations(currElement, qualifiedPath);

                // Data that needs to be parsed per result basis
                foreach (ScanResult scanResult in currElement.ScanResults.Items)
                {
                    // Traverse the Scan result items list
                    if (scanResult?.Items != null && scanResult.Items.Count != 0)
                    {
                        Rule rule = null;
                        Result sarifResult = null;
                        foreach (RuleResult ruleResult in scanResult.Items)
                        {
                            if (ruleResult != null)
                            {
                                rule = FetchOrAddRule(ruleResult);
                                sarifResult = new Result();
                                sarifResult.RuleId = ruleResult.Rule.ToString();
                                sarifResult.Level = ruleResult.Status.ToResultLevel();
                                sarifResult.PartialFingerprints = GetFingerPrintContributions(currElement, ruleResult);
                                sarifResult.Message = GetResultMessages(ruleResult.Messages);
                                sarifResult.Locations = location;
                                // Snippet has been moved to locations->physical locations -> annotations -> Snippet.
                                sarifResult.SetProperty<string[]>(TagsKey, new string[] { AccessibilityKey });
                                if (currElement.BoundingRectangle != null)
                                {
                                    System.Drawing.Rectangle rect = currElement.BoundingRectangle;
                                    SarifRectangle cusRec = new SarifRectangle(rect.Left, rect.Top, rect.Width, rect.Height);
                                    sarifResult.SetProperty<SarifRectangle>(ScreenShotRectangleKey, cusRec);
                                }
                                elementScanOutput.Add(new Tuple<Rule, Result>(rule, sarifResult));
                            }
                        }
                    }
                }
            }
            return elementScanOutput;
        }

        /// <summary>
        /// Generates the fingerprint contributions
        /// </summary>
        /// <param name="element"> The element for which the fingerprint is being generated</param>
        /// <param name="ruleResult"> Used to determine the rule and the ruke result </param>
        /// <returns>Fingerprint contributions for the element</returns>
        internal static IDictionary<string, string> GetFingerPrintContributions(A11yElement element, RuleResult ruleResult)
        {
            Dictionary<string, string> fingerPrintContributions = new Dictionary<string, string>();
            IFingerprint scanResultFingerprint = FingerprintFactory.GetFingerPrint(element, ruleResult.Rule, ruleResult.Status);
            foreach (FingerprintContribution contribution in scanResultFingerprint.Contributions)
            {
                fingerPrintContributions.Add(contribution.Key, contribution.Value);
            }
            return fingerPrintContributions;
        }

        /// <summary>
        /// Fetches or adds a rule if doesn't already exist
        /// </summary>
        /// <param name="ruleResult"> RuleResult </param>
        /// <returns> Returns a Rule object if a valid RuleResult is passed in. Null if invalid RuleResult passed in </returns>
        internal static Rule FetchOrAddRule(RuleResult ruleResult)
        {
            Rule rule = null;
            if (ruleResult != null && !RuleList.TryGetValue(ruleResult.Rule.ToString(), out rule))
            {
                rule = GenerateAndAddRule(ruleResult);
            }
            return rule;
        }

        /// <summary>
        /// Generates a rule and stores it for further retrieval
        /// </summary>
        /// <param name="ruleResult"> RuleResult </param>
        /// <returns></returns>
        internal static Rule GenerateAndAddRule(RuleResult ruleResult)
        {
            Rule rule = new Rule();
            rule.Id = ruleResult.Rule.ToString();
            rule.Name = new Message(ruleResult.Description, null, ruleResult.Description, null, null, null);
            rule.FullDescription = new Message(ruleResult.Description, null, ruleResult.Description, null, null, null);
            rule.SetProperty<string[]>(StandardsKey, FetchOrAddStandards(ruleResult));
            rule.HelpUri = ruleResult.HelpUrl?.Url?.ToUri();
            RuleList.Add(rule.Id, rule);
            return rule;
        }

        /// <summary>
        /// Fetches or adds a standard if doesn't already exist. Standard name is transformed from "[A11yCriteria-4.1.2]" to "A11yCriteria 4.1.2" 
        /// </summary>
        /// <param name="ruleResult"> RuleResult </param>
        /// <returns> Array of transformed A11yCriteria keys </returns>
        internal static string[] FetchOrAddStandards(RuleResult ruleResult)
        {
            A11yCriteria standardName = null;
            string standardKey = ruleResult.Source?.Replace("[", "").Replace("]", "").Replace("-", " ");
            if (!A11yCriteriaList.TryGetValue(standardKey, out standardName))
            {
                standardName = GenerateAndAddA11yCriteria(ruleResult, standardKey);
            }
            return new string[] { standardName.standardKey ?? String.Empty };
        }

        /// <summary>
        /// Generates and adds an A11yCriteria object.
        /// </summary>
        /// <param name="ruleResult"></param>
        /// <param name="standardName"> The key that is used to store that particular standard </param>
        /// <returns></returns>
        internal static A11yCriteria GenerateAndAddA11yCriteria(RuleResult ruleResult, string criteriaName)
        {
            A11yCriteria newA11yCriteria = new A11yCriteria(criteriaName, new Message(criteriaName, null, null, null, null, null), null, null, ruleResult.HelpUrl?.Url);
            A11yCriteriaList.Add(criteriaName, newA11yCriteria);
            return newA11yCriteria;
        }

        /// <summary>
        ///  Returns a list of attachments including screenshot and the tooloutput.
        /// </summary>
        /// <returns></returns>
        internal static List<Attachment> GetResultAttachments(List<string> fileKeys)
        {
            List<Attachment> attachments = new List<Attachment>();
            Attachment screenShotAttachment = GetAttachment(ScreenShotKey, fileKeys[0]);
            Attachment attachment = GetAttachment(ToolOutputKey, fileKeys[1]);
            attachments.Add(screenShotAttachment);
            attachments.Add(attachment);
            return attachments;
        }

        /// <summary>
        /// Generates the attachment objects
        /// </summary>
        /// <param name="messageString"> Description for the attachment </param>
        /// <param name="fileLocationURI"></param>
        /// <returns></returns>
        internal static Attachment GetAttachment(string messageString, string fileLocationURI)
        {
            Attachment attachment = new Attachment();
            attachment.Description = new Message(messageString, null, null, null, null, null);
            FileLocation fileLocation = new FileLocation();
            fileLocation.Uri = fileLocationURI.ToUri();
            attachment.FileLocation = fileLocation;
            return attachment;
        }

        /// <summary>
        /// Generates a list of locations. Locations contain information like the Snippet
        /// </summary>
        /// <param name="currElement"></param>
        /// <param name="currentLocation"></param>
        /// <returns></returns>
        internal static List<Location> GenerateLocations(A11yElement currElement, string currentLocation)
        {
            List<Location> locations = new List<Location>();
            Location location = new Location();
            FileContent snippet = new FileContent();
            snippet.Text = currElement.Glimpse;
            Region region = new Region();
            region.Snippet = snippet;
            location.Annotations = new List<Region>() { region };
            location.FullyQualifiedLogicalName = currentLocation;
            location.SetProperty<int>(UniqueIdKey, currElement.UniqueId);
            locations.Add(location);
            return locations;
        }

        /// <summary>
        /// Aggregates all the messages into one big message
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        internal static Message GetResultMessages(List<string> messages)
        {
            StringBuilder messageText = new StringBuilder();
            if (messages != null && messages.Count > 0)
            {
                foreach (string message in messages)
                {
                    messageText.Append(message).Append(" ");
                }
            }
            string finalMessage = messageText.ToString();
            return new Message(finalMessage, null, finalMessage, null, null, null);
        }

        /// <summary>
        /// Will generate the tool info for a SARIF file. The app version is defined as variable ToolSemVer in this class
        /// </summary>
        /// <returns>Tool information</returns>
        internal static Tool GenerateToolInfo()
        {
            Tool tool = new Tool();
            tool.Name = ToolName;
            tool.FullName = ToolName + " v" + ToolSemVer;
            tool.SemanticVersion = ToolSemVer;
            tool.Version = ToolSemVer;
            tool.SetProperty<String>(DownloadURIKey, DownloadURI);
            return tool;
        }

        /// <summary>
        /// Generates the invocation info generating and adding an invocation object with the current time as the end time.
        /// </summary>
        /// <returns> List of Invocations with the end time set.</returns>
        internal static List<Invocation> GenerateInvocationInfo()
        {
            Invocation invocation = new Invocation();
            invocation.StartTimeUtc = DateTime.UtcNow;
            invocation.EndTimeUtc = invocation.StartTimeUtc;
            List<Invocation> invocations = new List<Invocation>();
            invocations.Add(invocation);
            return invocations;
        }

        /// <summary>
        /// Used to generate and persist the SARIF file.
        /// </summary>
        /// <param name="path"> The path to store the SARIF file </param>
        /// <param name="ecId"> The guid of the element to traverse from </param>
        /// <param name="deleteResultsFile"> Should the results file be deleted </param>
        public static void GenerateAndPersistSarifFile(string path, Guid ecId, bool deleteResultsFile)
        {
            Run run = new Run();
            run.BaselineInstanceGuid = Guid.NewGuid().ToString();

            run.Files = GenerateEmbeddedFiles(path, ecId, deleteResultsFile, run.BaselineInstanceGuid);
            List<String> fileKeys = new List<string>(run.Files.Keys);

            // Generate tool info
            Tool tool = GenerateToolInfo();

            // Traverse the snapshot info to get the info we need.
            List<Tuple<Rule, Result>> elementResults = TraverseTreeDepthFirst(ecId);

            // Set the standards list. Standards temporarily not to be set till sarif sdk can provide a fix.
            //run.SetProperty<Dictionary<string, A11yCriteria>>(StandardsKey, ResultsFileSarifMapper.A11yCriteriaList);

            run.Invocations = GenerateInvocationInfo();

            // Add all rule, result pairs to the log file.
            var sb = new StringBuilder();
            List<Attachment> attachmentList = GetResultAttachments(fileKeys);
            using (var textWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (var sarifLogger = new SarifLogger(
                textWriter,
                analysisTargets: null,
                loggingOptions: LoggingOptions.PrettyPrint | LoggingOptions.Verbose, // <-- use PrettyPrint to generate readable (multi-line, indented) JSON
                run: run,
                tool: tool,
                invocationTokensToRedact: null,
                invocationPropertiesToLog: null))
                {
                    foreach (Tuple<Rule, Result> scanResult in elementResults)
                    {
                        if (scanResult.Item1 != null && scanResult.Item2 != null)
                        {
                            scanResult.Item2.Attachments = attachmentList;
                            sarifLogger.Log(scanResult.Item1, scanResult.Item2);
                        }
                    }
                }
            }

            // Persist the log file
            File.WriteAllText(path, sb.ToString());
        }
    }
}
