// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    [TestCategory("AccessibilityInsights.Rules")]
    public class ParentChildShouldNotHaveSameNameAndLocalizedControlType
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.ParentChildShouldNotHaveSameNameAndLocalizedControlType();

        [TestMethod]
        public void ElementIsNotKeyboardFocusable_RuleNotApplicable()
        {
            var e = new MockA11yElement();
            e.BoundingRectangle = new System.Drawing.Rectangle(10, 10, 20, 30);
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.IsKeyboardFocusable = false;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ElementIsKeyboardFocusable_RuleApplicable()
        {
            var e = new MockA11yElement();
            e.BoundingRectangle = new System.Drawing.Rectangle(10, 10, 20, 30);
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.IsKeyboardFocusable = true;

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ParentChildHaveSameNameLocalizedControlType_RuleError()
        {
            var e = new MockA11yElement();
            e.Name = "name";
            e.LocalizedControlType = "controltype";
            var p = new MockA11yElement();
            p.Name = "name";
            p.LocalizedControlType = "controltype";
            e.Parent = p;

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
        }

        [TestMethod]
        public void ParentChildHaveSameNameDifferentLocalizedControlType_RuleError()
        {
            var e = new MockA11yElement();
            e.Name = "name";
            e.LocalizedControlType = "controltype";
            var p = new MockA11yElement();
            p.Name = "name";
            p.LocalizedControlType = "controltype2";
            e.Parent = p;

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }

        [TestMethod]
        public void ParentChildHaveDifferentNameSameLocalizedControlType_RuleError()
        {
            var e = new MockA11yElement();
            e.Name = "name";
            e.LocalizedControlType = "controltype";
            var p = new MockA11yElement();
            p.Name = "name1";
            p.LocalizedControlType = "controltype";
            e.Parent = p;

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
        }
    } // class
} // namespace
