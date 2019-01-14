// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Bases;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    [TestCategory("AccessibilityInsights.Rules")]
    public class SelectionPatternSelectionRequired
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.SelectionPatternSelectionRequired();

        [TestMethod]
        public void TabControlWithSelectionPatternButNotEdgeFramework_Applicable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_TabControlTypeId;
            e.Patterns.Add(new A11yPattern(e, PatternType.UIA_SelectionPatternId));
            e.Framework = Framework.Win32;

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void TabControlWithSelectionPatternEdgeFramework_NotApplicable()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_TabControlTypeId;
            e.Patterns.Add(new A11yPattern(e, PatternType.UIA_SelectionPatternId));
            e.Framework = Framework.Edge;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
