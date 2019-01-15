// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;
using static AccessibilityInsights.RulesTest.ControlType;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class NameIsEmptyButElementIsNotKeyboardFocusableTest
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.NameIsEmptyButElementIsNotKeyboardFocusable();

        [TestMethod]
        public void TestNameIsEmptyButElementIsNotKeyboardFocusablePass()
        {
            using (var e = new MockA11yElement())
            {
                e.Name = "Bob";
                Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestNameIsEmptyButElementIsNotKeyboardFocusableOpen()
        {
            using (var e = new MockA11yElement())
            {
                e.Name = "";
                Assert.AreEqual(EvaluationCode.Open, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestNameIsEmptyButElementIsNotKeyboardFocusableNullArgument()
        {
            Assert.AreEqual(EvaluationCode.RuleExecutionError, Rule.Evaluate(null));
        }

        [TestMethod]
        public void TestNameIsEmptyButElementIsNotKeyboardFocusableProgressBar()
        {
            // progress bars are processed by NameIsNotEmpty

            var e = new MockA11yElement();
            e.ControlTypeId = CheckBox;
            e.Name = "Bob";
            e.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            Assert.IsTrue(Rule.Condition.Matches(e));

            e.ControlTypeId = ProgressBar;
            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
