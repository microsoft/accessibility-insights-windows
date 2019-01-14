// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AccessibilityInsights.Core.Bases;
using System;

namespace AccessibilityInsights.RuleSelection
{
    /// <summary>
    /// Class TestFactory
    /// </summary>
    public static class SuiteFactory
    {
        /// <summary>
        /// Run eligible tests for the given element
        /// </summary>
        /// <param name="element"></param>
        public static void RunRules(A11yElement element)
        {
            if (element == null) throw new ArgumentNullException(nameof(element));

            element.ScanResults?.Items.Clear();

            RuleRunner.Run(element);
        }

        public static string GetRuleVersion()
        {
            return RuleVersions.Version;
        }
    }
}
