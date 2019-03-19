// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;

namespace AccessibilityInsights.Extensions.Interfaces.IssueReporting
{
    /// <summary>
    /// Class to communicate Issue information across the extension interface
    /// </summary>
    public class IssueInformation
    {
        /// <summary>
        /// The title of the window where a Issue is located
        /// </summary>
        public string WindowTitle { get; }

        /// <summary>
        /// The Glimpse to the specific element
        /// </summary>
        public string Glimpse { get; }

        /// <summary>
        /// The link of how to fix the issue
        /// </summary>
        public Uri HowToFixLink { get; }

        /// <summary>
        /// The link to help about this issue
        /// </summary>
        public Uri HelpUri { get; }

        /// <summary>
        /// The name of the process where the issue was detected
        /// </summary>
        public string ProcessName { get; }

        /// <summary>
        /// The source of the rule
        /// </summary>
        public string RuleSource { get; }

        /// <summary>
        /// The description of the rule
        /// </summary>
        public string RuleDescription { get; }

        /// <summary>
        /// Test-specific information about the issue
        /// </summary>
        public string TestMessages { get; }

        /// <summary>
        /// A unique Id to this issue (is not stored in the Issue database)
        /// </summary>
        public Guid? InternalGuid { get; }

        /// <summary>
        /// The path to the element
        /// </summary>
        public string ElementPath { get; }

        /// <summary>
        /// The rule to include for telemetry purposes
        /// </summary>
        public string RuleForTelemetry { get; }

        /// <summary>
        /// The UI Framework
        /// </summary>
        public string UIFramework { get; }

        /// <summary>
        /// The type of Issue being reported
        /// </summary>
        public IssueType? IssueType { get; }

        /// <summary>
        /// Screenshot of element
        /// </summary>
        public Bitmap Screenshot { get; set; }

        /// <summary>
        /// Path to saved test file
        /// </summary>
        public string TestFileName { get; set; }

        public IssueInformation(string windowTitle = null, string glimpse = null, Uri howToFixLink = null,
            Uri helpUri = null, string ruleSource = null, string ruleDescription = null,
            string testMessages = null, Guid? internalGuid = null, string testFileName = null,
            string elementPath = null, string ruleForTelemetry = null, string uiFramework = null,
            string processName = null, IssueType? issueType = null, Bitmap screenshot = null)
        {
            WindowTitle = GetStringValue(windowTitle);
            Glimpse = GetStringValue(glimpse);
            HowToFixLink = howToFixLink;
            HelpUri = helpUri;
            RuleSource = GetStringValue(ruleSource);
            RuleDescription = GetStringValue(ruleDescription);
            TestMessages = GetStringValue(testMessages);
            ProcessName = GetStringValue(processName);
            InternalGuid = internalGuid;
            TestFileName = testFileName;
            ElementPath = GetStringValue(elementPath);
            RuleForTelemetry = GetStringValue(ruleForTelemetry);
            UIFramework = GetStringValue(uiFramework);
            IssueType = issueType;
            Screenshot = screenshot;
        }

        public IssueInformation OverwritingUnion(string windowTitle = null, string glimpse = null, Uri howToFixLink = null,
            Uri helpUri = null, string ruleSource = null, string ruleDescription = null,
            string testMessages = null, Guid? internalGuid = null, string testFileName = null,
            string elementPath = null, string ruleForTelemetry = null, string uiFramework = null,
            string processName = null, IssueType? issueType = null, Bitmap screenshot = null)
        {
            return new IssueInformation(
                windowTitle: GetReplacementString(windowTitle, WindowTitle),
                glimpse: GetReplacementString(glimpse, Glimpse),
                howToFixLink: howToFixLink ?? HowToFixLink,
                helpUri: helpUri ?? HelpUri,
                ruleSource: GetReplacementString(ruleSource, RuleSource),
                ruleDescription: GetReplacementString(ruleDescription, RuleDescription),
                testMessages: GetReplacementString(testMessages, TestMessages),
                processName: GetReplacementString(processName, ProcessName),
                internalGuid: internalGuid ?? InternalGuid,
                testFileName: testFileName ?? TestFileName,
                elementPath: GetReplacementString(elementPath, ElementPath),
                ruleForTelemetry: GetReplacementString(ruleForTelemetry, RuleForTelemetry),
                uiFramework: GetReplacementString(uiFramework, UIFramework),
                issueType: issueType ?? IssueType,
                screenshot: screenshot ?? Screenshot
            );
        }

        private static string GetStringValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            return value;
        }

        private static string GetReplacementString(string newValue, string oldValue)
        {
            if (newValue == null)
                return oldValue;

            // This allows an empty string to force the override to null
            if (string.IsNullOrEmpty(newValue))
                return null;

            return newValue;
        }
    }
}
