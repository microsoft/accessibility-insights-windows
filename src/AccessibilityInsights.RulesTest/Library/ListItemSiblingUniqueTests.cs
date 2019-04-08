// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class ListItemSiblingUniqueTests
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.ListItemSiblingsUnique();

        [TestMethod]
        public void ElementsMatch_Warning()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.ControlTypeId = ControlType.ListItem;
            child2.ControlTypeId = ControlType.ListItem;
            child1.LocalizedControlType = "ListItem";
            child2.LocalizedControlType = "ListItem";
            child1.Name = "Alice";
            child2.Name = "Alice";
            child1.IsKeyboardFocusable = true;
            child2.IsKeyboardFocusable = true;
            child1.Parent = parent;
            child2.Parent = parent;
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(EvaluationCode.Warning, Rule.Evaluate(child2));
        }

        [TestMethod]
        public void ControlTypeMismatch_Pass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.ControlTypeId = ControlType.ListItem;
            child2.ControlTypeId = ControlType.Button;
            child1.LocalizedControlType = "ListItem";
            child2.LocalizedControlType = "ListItem";
            child1.Name = "Alice";
            child2.Name = "Alice";
            child1.IsKeyboardFocusable = true;
            child2.IsKeyboardFocusable = true;
            child1.Parent = parent;
            child2.Parent = parent;
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(child2));
        }

        [TestMethod]
        public void NameMismatchPass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.ControlTypeId = ControlType.ListItem;
            child2.ControlTypeId = ControlType.ListItem;
            child1.LocalizedControlType = "ListItem";
            child2.LocalizedControlType = "ListItem";
            child1.Name = "Alice";
            child2.Name = "Bob";
            child1.IsKeyboardFocusable = true;
            child2.IsKeyboardFocusable = true;
            child1.Parent = parent;
            child2.Parent = parent;
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(child2));
        }

        [TestMethod]
        public void IsKeyboardFocusableMismatch_Pass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.ControlTypeId = ControlType.ListItem;
            child2.ControlTypeId = ControlType.ListItem;
            child1.LocalizedControlType = "ListItem";
            child2.LocalizedControlType = "ListItem";
            child1.Name = "Alice";
            child2.Name = "Alice";
            child1.IsKeyboardFocusable = true;
            child2.IsKeyboardFocusable = false;
            child1.Parent = parent;
            child2.Parent = parent;
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(child2));
        }

        [TestMethod]
        public void LocalizedControlTypeMismatch_Pass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.ControlTypeId = ControlType.ListItem;
            child2.ControlTypeId = ControlType.ListItem;
            child1.LocalizedControlType = "ListItem";
            child2.LocalizedControlType = "Button";
            child1.Name = "Alice";
            child2.Name = "Alice";
            child1.IsKeyboardFocusable = true;
            child2.IsKeyboardFocusable = true;
            child1.Parent = parent;
            child2.Parent = parent;
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(child2));
        }

        [TestMethod]
        public void ConditionMatch_True()
        {
            var parent = new MockA11yElement();
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child.IsContentElement = true;
            child.ControlTypeId = ControlType.ListItem;
            child.LocalizedControlType = "ListItem";
            child.Name = "Alice";
            child.IsKeyboardFocusable = true;
            child.Parent = parent;
            parent.Children.Add(child);

            Assert.IsTrue(Rule.Condition.Matches(child));
        }

        [TestMethod]
        public void ConditionMatch_InvalidBoundingRect_False()
        {
            var parent = new MockA11yElement();
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 0, 0);
            child.IsContentElement = true;
            child.ControlTypeId = ControlType.ListItem;
            child.LocalizedControlType = "ListItem";
            child.Name = "Alice";
            child.Parent = parent;
            parent.Children.Add(child);

            Assert.IsFalse(Rule.Condition.Matches(child));
        }

        [TestMethod]
        public void ConditionMatch_NotListItem_False()
        {
            var parent = new MockA11yElement();
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child.IsContentElement = true;
            child.ControlTypeId = ControlType.Button;
            child.LocalizedControlType = "ListItem";
            child.Name = "Alice";
            child.Parent = parent;
            parent.Children.Add(child);

            Assert.IsFalse(Rule.Condition.Matches(child));
        }

        [TestMethod]
        public void ConditionMatch_NotContentOrControl_False()
        {
            var parent = new MockA11yElement();
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child.ControlTypeId = ControlType.ListItem;
            child.LocalizedControlType = "ListItem";
            child.Name = "Alice";
            child.Parent = parent;
            parent.Children.Add(child);

            Assert.IsFalse(Rule.Condition.Matches(child));
        }

        [TestMethod]
        public void ConditionMatch_NoName_False()
        {
            var parent = new MockA11yElement();
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child.IsContentElement = true;
            child.ControlTypeId = ControlType.ListItem;
            child.LocalizedControlType = "ListItem";
            child.Parent = parent;
            parent.Children.Add(child);

            Assert.IsFalse(Rule.Condition.Matches(child));
        }

        [TestMethod]
        public void ConditionMatch_NoLocalizedControlType_False()
        {
            var parent = new MockA11yElement();
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child.IsContentElement = true;
            child.ControlTypeId = ControlType.ListItem;
            child.Name = "Alice";
            child.Parent = parent;
            parent.Children.Add(child);

            Assert.IsFalse(Rule.Condition.Matches(child));
        }

        [TestMethod]
        public void ConditionMatch_NoParent_False()
        {
            var child = new MockA11yElement();
            child.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child.IsContentElement = true;
            child.ControlTypeId = ControlType.ListItem;
            child.LocalizedControlType = "ListItem";
            child.Name = "Alice";

            Assert.IsFalse(Rule.Condition.Matches(child));
        }
    } // class
} // namespace
