// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Extensions.Interfaces.BugReporting;
using AccessibilityInsights.Extensions.AzureDevOps.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace AccessibilityInsights.Extensions.AzureDevOps
{
    internal static class BugInformationHelpers
    {
        internal static Dictionary<BugField, string> ToAzureDevOpsBugFields(this BugInformation bugInfo)
        {
            return new Dictionary<BugField, string>
            {
                { BugField.WindowTitle, GetStringValue(bugInfo.WindowTitle) },
                { BugField.Glimpse, GetStringValue(bugInfo.Glimpse) },
                { BugField.HowToFixLink, GetStringValue(bugInfo.HowToFixLink) },
                { BugField.HelpURL, GetStringValue(bugInfo.HelpUri) },
                { BugField.RuleSource, GetStringValue(bugInfo.RuleSource) },
                { BugField.RuleDescription, GetStringValue(bugInfo.RuleDescription) },
                { BugField.TestMessages, GetStringValue(bugInfo.TestMessages) },
                { BugField.ProcessName, GetStringValue(bugInfo.ProcessName) },
                { BugField.InternalGuid, GetStringValue(bugInfo.InternalGuid) },
                { BugField.ElementPath, GetStringValue(bugInfo.ElementPath) },
                { BugField.RuleForTelemetry, GetStringValue(bugInfo.RuleForTelemetry) },
                { BugField.UIFramework, GetStringValue(bugInfo.UIFramework) },
                { BugField.ContrastRatio, GetStringValue(bugInfo.ContrastRatio) },
                { BugField.FirstColorHex, GetStringValue(bugInfo.FirstColor) },
                { BugField.SecondColorHex, GetStringValue(bugInfo.SecondColor) },
                { BugField.ContrastFailureText, GetStringValue(bugInfo.ContrastFailureText) },
            };
        }

        private static string GetStringValue(Uri value)
        {
            return value?.ToString();
        }

        private static string GetStringValue(BugType? value)
        {
            return value?.ToString();
        }

        private static string GetStringValue(Guid? value)
        {
            return value?.ToString("D", CultureInfo.InvariantCulture);
        }

        private static string GetStringValue(double? value)
        {
            return value?.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetStringValue(Color? value)
        {
            return value?.ToString(CultureInfo.InvariantCulture);
        }

        private static string GetStringValue(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value;
        }
    }
}
