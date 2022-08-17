// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using AccessibilityInsights.Extensions.Helpers;
using AccessibilityInsights.Extensions.Interfaces.IssueReporting;
using HtmlAgilityPack;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using static System.FormattableString;

namespace AccessibilityInsights.Extensions.AzureDevOps.FileIssue
{
    /// <summary>
    /// Class with static functions used for filing issues
    /// </summary>
    internal class FileIssueHelpers
    {
        private readonly IDevOpsIntegration _devOpsIntegration;

        internal FileIssueHelpers(IDevOpsIntegration devOpsIntegration)
        {
            _devOpsIntegration = devOpsIntegration;
        }

        /// <summary>
        /// Opens issue filing window with prepopulated data
        /// </summary>
        /// <param name="issueInfo">Dictionary of issue info from with which to populate the issue</param>
        /// <param name="connection">connection info</param>
        /// <param name="onTop">Is window always on top</param>
        /// <param name="zoomLevel">Zoom level for issue file window</param>
        /// <param name="updateZoom">Callback to update configuration with zoom level</param>
        /// <returns></returns>
        internal (int? issueId, string newIssueId) FileNewIssue(IssueInformation issueInfo, ConnectionInfo connection, bool onTop, int zoomLevel, Action<int> updateZoom, string configurationPath)
        {
            return FileNewIssueTestable(issueInfo, connection, onTop, zoomLevel, updateZoom, configurationPath, null);
        }

        /// <summary>
        /// Testable version of FileNewIssue, allows caller to specify an issueId instead of going off-box
        /// </summary>
        internal (int? issueId, string newIssueId) FileNewIssueTestable(IssueInformation issueInfo, ConnectionInfo connection, bool onTop, int zoomLevel, Action<int> updateZoom, string configurationPath, int? testIssueId)
        {
            if (issueInfo == null)
                throw new ArgumentNullException(nameof(issueInfo));

            try
            {
                // Create a A11y-specific Guid for this issue to verify that we are uploading
                //  attachment to the correct issue
                var a11yIssueId = issueInfo.InternalGuid.HasValue
                    ? issueInfo.InternalGuid.Value.ToString()
                    : string.Empty;
                Uri url = CreateIssuePreviewAsync(connection, issueInfo).Result;

                int? issueId = testIssueId ?? FileIssueWindow(url, onTop, zoomLevel, updateZoom, configurationPath);

                return (issueId, a11yIssueId);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                e.ReportException();
                return (null, string.Empty);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        /// <summary>
        /// Attaches screenshot and results file to existing issue
        /// Default case - resulting test file will open in A11yFileMode.Inspect mode,
        ///     no additional loading parameters needed
        /// </summary>
        /// <param name="rect">Bounding rect of element for screenshot</param>
        /// <param name="a11yIssueId">Issue's A11y-specific id</param>
        /// <param name="issueId">Issue's server-side id</param>
        /// <returns>Success or failure</returns>
        internal async Task<bool> AttachIssueData(IssueInformation issueInfo, string a11yIssueId, int issueId)
        {
            if (issueInfo == null)
                throw new ArgumentNullException(nameof(issueInfo));

            return await AttachIssueDataInternal(issueInfo.TestFileName, issueInfo.Screenshot, a11yIssueId, issueId).ConfigureAwait(false);
        }

        /// <summary>
        /// Attempt to determine if we expect an exception to be transient.
        /// Will check children of an aggregate exception.
        /// Based on https://docs.microsoft.com/en-us/azure/architecture/patterns/retry
        /// </summary>
        /// <param name="ex">The exception to check</param>
        /// <returns></returns>
        private static bool IsTransient(Exception ex)
        {
            switch (ex)
            {
                case null:
                    return false;
                case AggregateException agEx:
                    foreach (var inner in agEx.InnerExceptions)
                    {
                        if (!IsTransient(inner))
                        {
                            return false;
                        }
                    }
                    return true;
                case WebException webEx:
                    return TransientWebExceptions.Contains(webEx.Status);
                // This is what we saw happen to issue attachments in our telemetry
                case TimeoutException _:
                    return true;
                default:
                    return false;
            }
        }

        // If a web exception contains one of the following status values, it might be transient.
        private static readonly HashSet<WebExceptionStatus> TransientWebExceptions = new HashSet<WebExceptionStatus>()
        {
            WebExceptionStatus.ConnectionClosed,
            WebExceptionStatus.Timeout,
            WebExceptionStatus.RequestCanceled,
            WebExceptionStatus.NameResolutionFailure
        };

        /// <summary>
        /// Saves screenshot, attaches screenshot and already saved results file to existing issue
        /// </summary>
        /// <param name="rect">Bounding rect of element for screenshot</param>
        /// <param name="a11yIssueId">Issue's A11y-specific id</param>
        /// <param name="issueId">Issue's server-side id</param>
        /// <param name="snapshotFileName">saved snapshot file name</param>
        /// <returns>Success or failure</returns>
        private async Task<bool> AttachIssueDataInternal(string snapshotFileName, Bitmap bitmap, string a11yIssueId, int issueId)
        {
            var imageFileName = GetTempFileName(".png");
            var filedIssueReproSteps = await _devOpsIntegration.GetExistingIssueDescription(issueId).ConfigureAwait(false);

            if (GuidsMatchInReproSteps(a11yIssueId, filedIssueReproSteps))
            {
                int? attachmentResponse = null;
                const int maxAttempts = 2;

                // Attempt to attach the results file twice
                for (int attempts = 0; attempts < maxAttempts; attempts++)
                {
                    try
                    {
                        attachmentResponse = await _devOpsIntegration.AttachTestResultToIssue(snapshotFileName, issueId).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (!IsTransient(ex))
                        {
                            throw;
                        }
                    }
                }

                // Save local screenshot for HTML preview in browser
                bitmap?.Save(imageFileName);

                var htmlDescription = "";

                if (imageFileName != null)
                {
                    var imgUrl = await _devOpsIntegration.AttachScreenshotToIssue(imageFileName, issueId).ConfigureAwait(false);
                    htmlDescription = $"<img src=\"{imgUrl}\" alt=\"screenshot\"></img>";
                }

                var scrubbedHTML = RemoveInternalHTML(filedIssueReproSteps, a11yIssueId);
                var updatedHTML = WrapInHtmlBody(scrubbedHTML) + htmlDescription;
                await _devOpsIntegration.ReplaceIssueDescription(updatedHTML, issueId).ConfigureAwait(false);
                File.Delete(snapshotFileName);
                if (imageFileName != null)
                {
                    File.Delete(imageFileName);
                }

                // if the issue failed to attach, return false
                return attachmentResponse != null;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the path to a newly created temporary directory
        /// </summary>
        /// <returns></returns>
        private static string GetTempDir()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDir);
            return tempDir;
        }

        /// <summary>
        /// Remove internal text from repro step.
        /// since the internal text is wrapped in a "div", find a div with matching key text and remove the child nodes of div.
        /// generally, keyText is guid value.
        /// </summary>
        /// <param name="inputHTML"></param>
        /// <param name="keyText"></param>
        /// <returns></returns>
        internal static string RemoveInternalHTML(string inputHTML, string keyText)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(inputHTML);

            HtmlNode documentNode = doc.DocumentNode;
            HtmlNode node = null;

            // search div with matching issueguid.
            // remove any of it if there is matched one.
            var divnodes = documentNode.Elements("div");

            if (divnodes != null)
            {
                foreach (var divnode in divnodes)
                {
                    foreach (var child in divnode.ChildNodes)
                    {
                        string nodevalue = child.NextSibling?.InnerHtml?.ToString();
                        if (nodevalue != null && nodevalue.Contains(keyText) == true)
                        {
                            node = divnode;
                            break;
                        }
                    }

                    if (node != null)
                    {
                        break;
                    }
                }
            }

            if (node != null)
            {
                foreach (var child in node.ChildNodes.ToList())
                {
                    node.RemoveChild(child);
                }
            }

            return documentNode.WriteContentTo();
        }

        internal static string WrapInHtmlBody(string innerHtml)
        {
            return $"<body>{innerHtml}</body>";
        }

        /// <summary>
        /// Returns true if the target guid is found in the given reprosteps string
        /// </summary>
        /// <param name="targetGuid"></param>
        /// <param name="reproSteps"></param>
        /// <returns></returns>
        private static bool GuidsMatchInReproSteps(string targetGuid, string reproSteps)
        {
            int filedIssueIdIndex = reproSteps.IndexOf(targetGuid, StringComparison.Ordinal);
            return filedIssueIdIndex >= 0;
        }

        /// <summary>
        /// Load the issue filing web browser in a blocking window and return the issue number (null or id)
        /// Change the configuration zoom level for the embedded browser
        /// </summary>
        /// <param name="url"></param>
        private static int? FileIssueWindow(Uri url, bool onTop, int zoomLevel, Action<int> updateZoom, string configurationPath)
        {
            if (!IsWebView2RuntimeInstalled())
            {
                var webView = new WebviewRuntimeNotInstalled(onTop);
                webView.ShowDialog();
                return null;
            }

            Trace.WriteLine(Invariant($"Url is {url.AbsoluteUri.Length} long: {url}"));
            var dlg = new IssueFileForm(url, onTop, zoomLevel, updateZoom, configurationPath)
            {
                ScriptToRun = "window.onerror = function(msg,url,line) { window.external.Log(msg); return true; };"
            };

            dlg.ShowDialog();

            return dlg.IssueId;
        }

        private static bool IsWebView2RuntimeInstalled()
        {
            try
            {
                CoreWebView2Environment.GetAvailableBrowserVersionString();
                return true;
            }
            catch (WebView2RuntimeNotFoundException e)
            {
                e.ReportException();
                return false;
            }
        }

        /// <summary>
        /// Creates a temp file with the given extension and returns its path
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private static string GetTempFileName(string extension)
        {
            return Path.Combine(GetTempDir(), Path.GetRandomFileName() + extension);
        }

        internal Task<Uri> CreateIssuePreviewAsync(ConnectionInfo connectionInfo, IssueInformation issueInfo)
        {
            if (issueInfo == null)
                throw new ArgumentNullException(nameof(issueInfo));

            string templateName = GetTemplateName(issueInfo);
            Dictionary<IssueField, string> issueFieldPairs = issueInfo.ToAzureDevOpsIssueFields();
            TruncateSelectedFields(issueInfo, issueFieldPairs);

            Dictionary<AzureDevOpsField, string> fieldPairs = GenerateIssueTemplate(issueFieldPairs, templateName);
            AddAreaAndIterationPathFields(connectionInfo, fieldPairs);

            return Task<Uri>.Run(() => _devOpsIntegration.CreateIssuePreview(connectionInfo.Project.Name, connectionInfo.Team?.Name, fieldPairs));
        }

        private Task<string> GetAreaPathAsync(ConnectionInfo connectionInfo)
        {
            return Task<string>.Run(() => _devOpsIntegration.GetAreaPath(connectionInfo));
        }

        private Task<string> GetIterationPathAsync(ConnectionInfo connectionInfo)
        {
            return Task<string>.Run(() => _devOpsIntegration.GetIteration(connectionInfo));
        }

        /// <summary>
        /// Extract the template name from an IssueInformation object
        /// </summary>
        /// <param name="issueInfo"></param>
        /// <returns>The name of the template</returns>
        private static string GetTemplateName(IssueInformation issueInfo)
        {
            if (issueInfo.IssueType.HasValue)
            {
                switch (issueInfo.IssueType.Value)
                {
                    case IssueType.SingleFailure: return "IssueSingleFailure";
                }
            }
            return "IssueNoFailures";
        }

        /// <summary>
        /// Extract the area and path fields from the IConnectionInfo and incorporate them into AzureDevOpsFieldPairs
        /// </summary>
        /// <param name="connectionInfo">The source of the data to extract</param>
        /// <param name="fieldPairs">The destination of the extracted data</param>
        private void AddAreaAndIterationPathFields(ConnectionInfo connectionInfo, IDictionary<AzureDevOpsField, string> fieldPairs)
        {
            var areaPathTask = GetAreaPathAsync(connectionInfo);
            var iterationPathTask = GetIterationPathAsync(connectionInfo);

            Task[] tasks = new Task[] { areaPathTask, iterationPathTask };
            Task.WaitAll(tasks);

            if (areaPathTask.Result != null)
            {
                fieldPairs.Add(AzureDevOpsField.AreaPath, areaPathTask.Result);
            }
            if (iterationPathTask.Result != null)
            {
                fieldPairs.Add(AzureDevOpsField.IterationPath, iterationPathTask.Result);
            }
        }

        /// <summary>
        /// Generates a dictionary of AzureDevOpsField/string pairs for creating an issue in AzureDevOps
        /// </summary>
        /// <param name="issueFieldPairs">The collection of IssueField/string pairs that describe this issue</param>
        /// <returns>The dictionary of known pairs</returns>
        private static Dictionary<AzureDevOpsField, string> GenerateIssueTemplate(Dictionary<IssueField, string> issueFieldPairs, string templateName)
        {
            Dictionary<AzureDevOpsField, string> templatedAzureDevOpsFieldPairs = GetIssueFieldMappings(issueFieldPairs, templateName);

            // Template & add description field
            string issueDescTemplate = string.Concat(File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"IssueTemplates\\{templateName}.html")));
            templatedAzureDevOpsFieldPairs.Add(AzureDevOpsField.ReproSteps, PopulateIssueTemplateString(issueDescTemplate, issueFieldPairs));

            return templatedAzureDevOpsFieldPairs;
        }

        /// <summary>
        /// Returns a dictionary that maps from IssueField names to string values that should be used
        /// to file an issue. Substitutions can be defined in a separate json file.
        /// </summary>
        /// <param name="issueFieldPairs">The collection of IssueField/string pairs to apply to the template</param>
        /// <param name="templateName">The name of the template to use</param>
        /// <returns></returns>
        private static Dictionary<AzureDevOpsField, string> GetIssueFieldMappings(Dictionary<IssueField, string> issueFieldPairs, string templateName)
        {
            string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"IssueTemplates\\{templateName}.json"));
            var templateDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            Dictionary<AzureDevOpsField, string> fieldPairs = new Dictionary<AzureDevOpsField, string>();

            foreach (var pair in templateDictionary)
            {
                if (Enum.TryParse(pair.Key, out AzureDevOpsField field))
                {
                    fieldPairs[field] = PopulateIssueTemplateString(pair.Value, issueFieldPairs);
                }
            }

            return fieldPairs;
        }

        /// <summary>
        /// If any key from the dictionary exists in inputTemplate surrounded by [], then
        ///     the string [key] will be replaced with its value from the dictionary
        /// If the value is null, the formatted output string will read "unknown"
        /// </summary>
        /// <param name="inputTemplate">The string to modify, will contain [key], where key can take on values from
        ///                             the IssueFields enum</param>
        /// <param name="issueFieldPairs">The collection of IssueField/string pairs to apply to the template</param>
        /// <returns></returns>
        private static string PopulateIssueTemplateString(string inputTemplate, Dictionary<IssueField, string> issueFieldPairs)
        {
            foreach (var pair in issueFieldPairs)
            {
                var name = Enum.GetName(typeof(IssueField), pair.Key);
                var value = pair.Value ?? Properties.Resources.UnknownValue;
                inputTemplate = inputTemplate.Replace($"@[{name}]@", value);
            }
            return inputTemplate;
        }

        /// <summary>
        /// Truncate key fields from the input IssueInformation and return a revised object
        ///     certain values are truncated to {length} characters long,
        ///     certain values are formatted to be more readable
        /// </summary>
        /// <param name="issueInfo">Non-truncated information from caller</param>
        /// <param name="issueFieldPairs">The collection of IssueField/string pairs to truncate</param>
        /// <returns></returns>
        private static void TruncateSelectedFields(IssueInformation issueInfo, IDictionary<IssueField, string> issueFieldPairs)
        {
            issueFieldPairs[IssueField.ProcessName] = TruncateString(issueInfo.ProcessName, 50, ".exe");
            issueFieldPairs[IssueField.Glimpse] = TruncateString(issueInfo.Glimpse, 50);
            issueFieldPairs[IssueField.TestMessages] = TruncateString(issueInfo.TestMessages, 150, Properties.Resources.ConcatenationMessage);
            issueFieldPairs[IssueField.RuleSource] = RemoveSurroundingBrackets(issueInfo.RuleSource);
        }

        private static string RemoveSurroundingBrackets(string originalString)
        {
            if (string.IsNullOrWhiteSpace(originalString))
                return null;

            if (originalString.StartsWith("[", StringComparison.Ordinal) && originalString.EndsWith("]", StringComparison.Ordinal))
            {
                return originalString.Substring(1, originalString.Length - 2);
            }

            return originalString;
        }

        private static string TruncateString(string originalString, int limit, string suffix = "...")
        {
            if (string.IsNullOrWhiteSpace(originalString))
                return null;

            if (originalString.Length > limit)
                return originalString.Substring(0, limit) + suffix;

            return originalString;
        }
    }
}
