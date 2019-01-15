// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class IsKeyboardFocusableOnEmptyContainer
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.IsKeyboardFocusableOnEmptyContainer();

        [TestMethod]
        public void ConditionMatch_ListControl_ReturnTrue()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ListControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMatch_DataGridControl_ReturnTrue()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_DataGridControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMatch_TableControl_ReturnTrue()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_TableControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMatch_TreeControl_ReturnTrue()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_TreeControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void Scan_TreeControlWithoutTreeItem_ReturnOpen()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_TreeControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.AreEqual(EvaluationCode.Open, Rule.Evaluate(e));
        }

        [TestMethod]
        public void ConditionMismatch_TreeControlWithTreeItem_ReturnFalse()
        {
            var e = new MockA11yElement();
            var ec = new MockA11yElement();
            
            e.ControlTypeId = Core.Types.ControlType.UIA_TreeControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);
            ec.ControlTypeId = Core.Types.ControlType.UIA_TreeItemControlTypeId;

            e.Children.Add(ec);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ButtonControl_ReturnFalse()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsKeyboardFocusable = false;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
