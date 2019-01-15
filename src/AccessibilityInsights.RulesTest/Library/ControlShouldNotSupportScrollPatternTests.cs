// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Enums;
using AccessibilityInsights.Core.Types;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    [TestCategory("AccessibilityInsights.Rules")]
    public class ControlShouldNotSupportScrollPatternTests
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.ControlShouldNotSupportScrollPattern();

        private static readonly int[] ExpectedControlTypes = { ControlType.ScrollBar };
        private static readonly IEnumerable<int> UnexpectedControlTypes = ControlType.All.Difference(ExpectedControlTypes);

        [TestMethod]
        public void IsExpectedControlType_Condition_True()
        {
            var e = new MockA11yElement();

            foreach (var controlType in ExpectedControlTypes)
            {
                e.ControlTypeId = controlType;
                Assert.IsTrue(Rule.Condition.Matches(e));
            } // for each control type
        }

        [TestMethod]
        public void IsUnexpectedControlType_Condition_True()
        {
            var e = new MockA11yElement();

            foreach (var controlType in UnexpectedControlTypes)
            {
                e.ControlTypeId = controlType;
                Assert.IsFalse(Rule.Condition.Matches(e));
            } // for each control type
        }

        [TestMethod]
        public void HasScrollPattern_EvaluateReturnsError()
        {
            var e = new MockA11yElement();
            e.Patterns.Add(new Core.Bases.A11yPattern(e, PatternType.UIA_ScrollPatternId));

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void DoesNotHaveScrollPattern_EvaluateReturnsPass()
        {
            var e = new MockA11yElement();

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }
    } // class
} // namespace
