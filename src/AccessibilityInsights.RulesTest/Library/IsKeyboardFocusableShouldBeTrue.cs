// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class IsKeyboardFocusableShouldBeTrue
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.IsKeyboardFocusableShouldBeTrue();

        [TestMethod]
        public void TestIsKeyboardFocusableShouldBeTruePass()
        {
            using (var e = new MockA11yElement())
            {
                e.IsKeyboardFocusable = true;
                Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestIsKeyboardFocusableShouldBeTrueError()
        {
            using (var e = new MockA11yElement())
            {
                Assert.AreNotEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestIsKeyboardFocusableShouldBeTrueArgumentException()
        {
            Action action = () => Rule.Evaluate(null);
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void ConditionMismatch_ExcludingCustomControl_ReturnFalse()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_CustomControlTypeId;
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ExcludingUIExpandoButtonDirectUIControl_ReturnFalse()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.Framework = "DirectUI";
            e.ClassName = "UIExpandoButton";
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_DropDownDirectUIControl_ReturnFalse()
        {
            var e = new MockA11yElement();
            var ep = new MockA11yElement();
            ep.ControlTypeId = Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.Parent = ep;

            e.Framework = "DirectUI";
            e.AutomationId = "Dropdown";
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMatch_ChildSplitButtonOfGroupControlWithoutSplitButtonSibling_ReturnTrue()
        {
            var e = new MockA11yElement();
            var ep = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            ep.ControlTypeId = Core.Types.ControlType.UIA_GroupControlTypeId;
            e.Parent = ep;

            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMatch_ChildSplitButtonOfGroupControlWithSplitButtonAsFirstSibling_ReturnTrue()
        {
            var e = new MockA11yElement();
            e.UniqueId = 2;
            var ep = new MockA11yElement();
            ep.UniqueId = 0;
            var efs = new MockA11yElement();
            efs.UniqueId = 1;

            e.ControlTypeId = Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            ep.ControlTypeId = Core.Types.ControlType.UIA_GroupControlTypeId;
            efs.ControlTypeId = Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            e.Parent = ep;
            e.Children.Add(efs);
            e.Children.Add(e);

            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMatch_ChildSplitButtonOfGroupControlWithButtonAsFirstSibling_ReturnTrue()
        {
            var e = new MockA11yElement();
            e.UniqueId = 2;
            var ep = new MockA11yElement();
            ep.UniqueId = 0;
            var efs = new MockA11yElement();
            efs.UniqueId = 1;

            e.ControlTypeId = Core.Types.ControlType.UIA_SplitButtonControlTypeId;
            ep.ControlTypeId = Core.Types.ControlType.UIA_GroupControlTypeId;
            efs.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.Parent = ep;
            e.Children.Add(efs);
            e.Children.Add(e);

            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsTrue(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ContainerLikeList_ReturnFalse()
        {
            var e = new MockA11yElement();

            e.ControlTypeId = Core.Types.ControlType.UIA_ListControlTypeId;

            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ExcludingNetUIAppFrameHelperButton_False()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.ClassName = "NetUIAppFrameHelper";
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ExcludingNetUIFolderBarRootButton_False()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.ClassName = "NetUIFolderBarRoot";
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ExcludingNetUIStickyButton_False()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = Core.Types.ControlType.UIA_ButtonControlTypeId;
            e.ClassName = "NetUIStickyButton";
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }

        [TestMethod]
        public void ConditionMismatch_ExcludeQuickHelpMenuItem_False()
        {
            var e = new MockA11yElement();
            e.ControlTypeId = ControlType.MenuItem;
            e.AutomationId = "TellMeControlAnchor";
            e.IsEnabled = true;
            e.IsOffScreen = false;
            e.IsContentElement = true;
            e.BoundingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);

            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    } // class
} // namespace
