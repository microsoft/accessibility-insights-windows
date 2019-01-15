// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    [Export(typeof(IBugReporting))]
    public class AzureDevOpsBugReporting : IBugReporting
    {
        private AzureDevOpsIntegration AzureDevOps = AzureDevOpsIntegration.GetCurrentInstance();

        public IEnumerable<byte> Avatar => AzureDevOps.Avatar;

        public string DisplayName => AzureDevOps.DisplayName;

        public string Email => AzureDevOps.Email;

        public bool IsConnected => AzureDevOps.ConnectedToAzureDevOps;

        public Task<string> AttachScreenshotToBugAsync(string path, int bugId)
        {
            return AzureDevOps.AttachScreenshotToBug(path, bugId);
        }

        public Task<int?> AttachTestResultToBugAsync(string path, int bugId)
        {
            return AzureDevOps.AttachTestResultToBug(path, bugId);
        }

        public Task ConnectAsync(Uri uri, bool prompt)
        {
            return AzureDevOps.ConnectToAzureDevOpsAccount(uri, prompt);
        }

        public IConnectionCache CreateConnectionCache(string configString)
        {
            return new ConnectionCache(configString);
        }

        public IConnectionInfo CreateConnectionInfo(Uri serverUri, IProject project, ITeam team)
        {
            return new ConnectionInfo(serverUri, project, team);
        }

        public IConnectionInfo CreateConnectionInfo(string configString)
        {
            try
            {
                return new ConnectionInfo(configString);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Disconnect()
        {
            AzureDevOps.Disconnect();
        }

        public void FlushToken(Uri uri)
        {
            AzureDevOps.FlushToken(uri);
        }

        public Task<Uri> CreateBugPreviewAsync(IConnectionInfo connectionInfo, BugInformation bugInfo)
        {
            string templateName = GetTemplateName(bugInfo);
            Dictionary<BugField, string> bugFieldPairs = bugInfo.ToAzureDevOpsBugFields();
            TruncateSelectedFields(bugInfo, bugFieldPairs);

            Dictionary<AzureDevOpsField, string> fieldPairs = GenerateBugTemplate(bugFieldPairs, templateName);
            AddAreaAndIterationPathFields(connectionInfo, fieldPairs);

            return Task<Uri>.Run(() => AzureDevOps.CreateBugPreview(connectionInfo.Project.Name, connectionInfo.Team?.Name, fieldPairs));
        }

        public Task<string> GetAreaPathAsync(IConnectionInfo connectionInfo)
        {
            return Task<string>.Run(() => AzureDevOps.GetAreaPath(connectionInfo));
        }

        public Task<string> GetExistingBugDescriptionAsync(int bugId)
        {
            return AzureDevOps.GetExistingBugDescription(bugId);
        }

        public Task<Uri> GetExistingBugUriAsync(int bugId)
        {
            return Task<Uri>.Run(() => AzureDevOps.GetExistingBugUrl(bugId));
        }

        public Task<string> GetIterationPathAsync(IConnectionInfo connectionInfo)
        {
            return Task<string>.Run(() => AzureDevOps.GetIteration(connectionInfo));
        }

        public Task<IEnumerable<IProject>> GetProjectsAsync()
        {
            return Task<IEnumerable<IProject>>.Run(() => AzureDevOps.GetTeamProjects());
        }

        public Task PopulateUserProfileAsync()
        {
            return AzureDevOps.PopulateUserProfile();
        }

        public Task<int?> ReplaceBugDescriptionAsync(string description, int bugId)
        {
            return AzureDevOps.ReplaceBugDescription(description, bugId);
        }

        /// <summary>
        /// Extract the template name from a BugInformation object
        /// </summary>
        /// <param name="bugInfo"></param>
        /// <returns>The name of the template</returns>
        private static string GetTemplateName(BugInformation bugInfo)
        {
            if (bugInfo.BugType.HasValue)
            {
                switch (bugInfo.BugType.Value)
                {
                    case BugType.ColorContrast: return "BugColorContrast";
                    case BugType.SingleFailure: return "BugSingleFailure";
                }
            }
            return "BugNoFailures";
        }

        /// <summary>
        /// Extract the area and path fields from the IConnectionInfo and incorporate them into AzureDevOpsFieldPairs
        /// </summary>
        /// <param name="connectionInfo">The source of the data to extract</param>
        /// <param name="fieldPairs">The destination of the extracted data</param>
        private void AddAreaAndIterationPathFields(IConnectionInfo connectionInfo, IDictionary<AzureDevOpsField, string> fieldPairs)
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
        /// Generates a dictionary of AzureDevOpsField/string pairs for creating a bug in AzureDevOps
        /// </summary>
        /// <param name="bugFieldPairs">The collection of BugField/string pairs that describe this bug</param>
        /// <returns>The dictionary of known pairs</returns>
        private static Dictionary<AzureDevOpsField, string> GenerateBugTemplate(Dictionary<BugField, string> bugFieldPairs, string templateName)
        {
            Dictionary<AzureDevOpsField, string> templatedAzureDevOpsFieldPairs = GetBugFieldMappings(bugFieldPairs, templateName);

            // Template & add description field
            string bugDescTemplate = string.Concat(File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"BugTemplates\\{templateName}.html")));
            templatedAzureDevOpsFieldPairs.Add(AzureDevOpsField.ReproSteps, PopulateBugTemplateString(bugDescTemplate, bugFieldPairs));

            return templatedAzureDevOpsFieldPairs;
        }

        /// <summary>
        /// Returns a dictionary that maps from BugField names to string values that should be used
        /// to file a bug. Substitutions can be defined in a separate json file.
        /// </summary>
        /// <param name="bugFieldPairs">The collection of BugField/string pairs to apply to the template</param>
        /// <param name="templateName">The name of the template to use</param>
        /// <returns></returns>
        private static Dictionary<AzureDevOpsField, string> GetBugFieldMappings(Dictionary<BugField, string> bugFieldPairs, string templateName)
        {
            string json = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"BugTemplates\\{templateName}.json"));
            var templateDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            Dictionary<AzureDevOpsField, string> fieldPairs = new Dictionary<AzureDevOpsField, string>();

            foreach (var pair in templateDictionary)
            {
                if (Enum.TryParse(pair.Key, out AzureDevOpsField field))
                {
                    fieldPairs[field] = PopulateBugTemplateString(pair.Value, bugFieldPairs);
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
        ///                             the BugFields enum</param>
        /// <param name="bugFieldPairs">The collection of BugField/string pairs to apply to the template</param>
        /// <returns></returns>
        private static string PopulateBugTemplateString(string inputTemplate, Dictionary<BugField, string> bugFieldPairs)
        {
            StreamlineBugFieldPairs(bugFieldPairs);
            foreach (var pair in bugFieldPairs)
            {
                var name = Enum.GetName(typeof(BugField), pair.Key);
                var value = pair.Value ?? "[unknown]";
                inputTemplate = inputTemplate.Replace($"@[{name}]@", value);
            }
            return inputTemplate;
        }

        /// <summary>
        /// Avoids an experience where the bug description has duplicated information. 
        /// Currently geared to handle duplicate test message and rule description.
        /// </summary>
        /// <param name="bugFieldPairs">The collection of BugField/string pairs to streamline</param>
        private static void StreamlineBugFieldPairs(IDictionary<BugField, string> bugFieldPairs)
        {
            if (bugFieldPairs.TryGetValue(BugField.TestMessages, out string testMessages))
            {
                bugFieldPairs.Remove(BugField.TestMessages);
                if (bugFieldPairs.TryGetValue(BugField.RuleDescription, out string ruleDescription))
                {
                    if (!string.Equals(ruleDescription, testMessages, StringComparison.OrdinalIgnoreCase)
                        && !string.IsNullOrWhiteSpace(ruleDescription)
                        && !string.IsNullOrWhiteSpace(testMessages))
                    {
                        ruleDescription = string.Concat(ruleDescription, " <br /> ", testMessages);
                        bugFieldPairs.Remove(BugField.TestMessages);
                        bugFieldPairs[BugField.RuleDescription] = ruleDescription;
                    }
                }
            }
        }

        /// <summary>
        /// Truncate key fields from the input BugInformation and return a revised object
        ///     certain values are truncated to {length} characters long,
        ///     certain values are formatted to be more readable
        /// </summary>
        /// <param name="bugInfo">Non-truncated information from caller</param>
        /// <param name="bugFieldPairs">The collection of BugField/string pairs to truncate</param>
        /// <returns></returns>
        private static void TruncateSelectedFields(BugInformation bugInfo, IDictionary<BugField, string> bugFieldPairs)
        {
            bugFieldPairs[BugField.ProcessName] = TruncateString(bugInfo.ProcessName, 50, ".exe");
            bugFieldPairs[BugField.Glimpse] = TruncateString(bugInfo.Glimpse, 50);
            bugFieldPairs[BugField.TestMessages] = TruncateString(bugInfo.TestMessages, 150, "...open attached A11y test file for full details.");
            bugFieldPairs[BugField.RuleSource] = RemoveSurroundingBrackets(bugInfo.RuleSource);
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
