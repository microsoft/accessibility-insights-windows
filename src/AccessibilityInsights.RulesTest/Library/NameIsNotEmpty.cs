// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;
using static AccessibilityInsights.RulesTest.ControlType;
using System.Drawing;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    [TestCategory("AccessibilityInsights.Rules")]
    public class NameIsNotEmpty
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.NameIsNotEmpty();

        [TestMethod]
        public void TestNameEmpty()
        {
            var e = new MockA11yElement();
            e.Name = "";

            Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
        }

        [TestMethod]
        public void TestNameNotEmpty()
        {
            var e = new MockA11yElement();
            e.Name = " ";

            Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
        }

        [TestMethod]
        public void TestNameIsNotEmptyProgressBar()
        {
            var e = new MockA11yElement();
            e.IsKeyboardFocusable = false;
            e.ControlTypeId = CheckBox;
            e.Name = "";
            e.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            Assert.IsFalse(Rule.Condition.Matches(e));

            e.ControlTypeId = ProgressBar;
            Assert.IsTrue(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
