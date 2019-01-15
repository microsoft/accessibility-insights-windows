// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Media;

namespace AccessibilityInsights.Extensions.Interfaces.BugReporting
{
    /// <summary>
    /// Class to communicate bug information across the extension interface
    /// </summary>
    public class BugInformation
    {
        /// <summary>
        /// The title of the window where a bug is located
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
        /// A unique Id to this issue (is not stored in the bug database)
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
        /// The computed contrast ratio between FirstColor and SecondColor
        /// </summary>
        public double? ContrastRatio { get; }
        /// <summary>
        /// The first color used when computing ContrastRatio
        /// </summary>
        public Color? FirstColor { get; }
        /// <summary>
        /// The second color used when computing ContrastRatio
        /// </summary>
        public Color? SecondColor { get; }
        /// <summary>
        /// Additional information about a color contrast failure
        /// </summary>
        public string ContrastFailureText { get; }
        /// <summary>
        /// The type of bug being reported
        /// </summary>
        public BugType? BugType { get; }

        public BugInformation(string windowTitle = null, string glimpse = null, Uri howToFixLink = null,
            Uri helpUri = null, string ruleSource = null, string ruleDescription = null,
            string testMessages = null, Guid? internalGuid = null,
            string elementPath = null, string ruleForTelemetry = null, string uiFramework = null,
            string processName = null, double? contrastRatio = null, Color? firstColor = null,
            Color? secondColor = null, string contrastFailureText = null, BugType? bugType = null)
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
            ElementPath = GetStringValue(elementPath);
            RuleForTelemetry = GetStringValue(ruleForTelemetry);
            UIFramework = GetStringValue(uiFramework);
            ContrastRatio = contrastRatio;
            FirstColor = firstColor;
            SecondColor = secondColor;
            ContrastFailureText = GetStringValue(contrastFailureText);
            BugType = bugType;
        }

        public BugInformation OverwritingUnion(string windowTitle = null, string glimpse = null, Uri howToFixLink = null,
            Uri helpUri = null, string ruleSource = null, string ruleDescription = null,
            string testMessages = null, Guid? internalGuid = null,
            string elementPath = null, string ruleForTelemetry = null, string uiFramework = null,
            string processName = null, double? contrastRatio = null, Color? firstColor = null,
            Color? secondColor = null, string contrastFailureText = null, BugType? bugType = null)
        {
            return new BugInformation(
                windowTitle: GetReplacementString(windowTitle, WindowTitle),
                glimpse: GetReplacementString(glimpse, Glimpse),
                howToFixLink: howToFixLink ?? HowToFixLink,
                helpUri: helpUri ?? HelpUri,
                ruleSource: GetReplacementString(ruleSource, RuleSource),
                ruleDescription: GetReplacementString(ruleDescription, RuleDescription),
                testMessages: GetReplacementString(testMessages, TestMessages),
                processName: GetReplacementString(processName, ProcessName),
                internalGuid: internalGuid ?? InternalGuid,
                elementPath: GetReplacementString(elementPath, ElementPath),
                ruleForTelemetry: GetReplacementString(ruleForTelemetry, RuleForTelemetry),
                uiFramework: GetReplacementString(uiFramework, UIFramework),
                contrastRatio: contrastRatio ?? ContrastRatio,
                firstColor: firstColor ?? FirstColor,
                secondColor: secondColor ?? SecondColor,
                contrastFailureText: GetReplacementString(contrastFailureText, ContrastFailureText),
                bugType: bugType ?? BugType
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
