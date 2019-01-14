// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    [TestCategory("AccessibilityInsights.Rules")]
    public class LocalizedControlTypeIsNotNull
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.LocalizedControlTypeIsNotNull();

        [TestMethod]
        public void IsKeyboardFocusableTrue_LocalizedControlTypeNull_ScanError()
        {
            var e = new MockA11yElement();
            e.LocalizedControlType = null;
            e.IsKeyboardFocusable = true;

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void ConditionMismatch_IsKeyboardFocusableFalse_ReturnFalse()
        {
            var e = new MockA11yElement();
            e.IsKeyboardFocusable = false;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void LocalizedControlTypeNotNull_ScanPass()
        {
            var e = new MockA11yElement();
            e.IsKeyboardFocusable = true;
            e.LocalizedControlType = "abc";

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }
    } // class
} // namespace
