// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Rules.PropertyConditions;

namespace AccessibilityInsights.RulesTest.PropertyConditions
{
    [TestClass]
    public class PatternsTest
    {
        [TestMethod]
        public void TestTextSelectionSupportedTrue()
        {
            using (var e = new MockA11yElement())
            {
                var pattern = new A11yPattern(e, PatternIDs.Text);
                var property = new A11yPatternProperty {  Name = "SupportedTextSelection", Value = UIAutomationClient.SupportedTextSelection.SupportedTextSelection_Single};
                pattern.Properties.Add(property);
                e.Patterns.Add(pattern);
                Assert.IsTrue(Patterns.TextSelectionSupported.Matches(e));
;            } // using
        }

        [TestMethod]
        public void TestTextSelectionSupportedFalse()
        {
            using (var e = new MockA11yElement())
            {
                var pattern = new A11yPattern(e, PatternIDs.Text);
                var property = new A11yPatternProperty { Name = "SupportedTextSelection", Value = UIAutomationClient.SupportedTextSelection.SupportedTextSelection_None };
                pattern.Properties.Add(property);
                e.Patterns.Add(pattern);
                Assert.IsFalse(Patterns.TextSelectionSupported.Matches(e));
                ;
            } // using
        }
    } // class
} // namespace
