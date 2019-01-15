// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class SiblingUniqueAndFocusableTest
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.SiblingUniqueAndFocusable();

        [TestMethod]
        public void TestTypeMismatchPass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.IsContentElement = true;
            child2.IsContentElement = true;
            child1.LocalizedControlType = "MyType1";
            child2.LocalizedControlType = "MyType2";
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
        public void TestNameMismatchPass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.IsContentElement = true;
            child2.IsContentElement = true;
            child1.LocalizedControlType = "MyType";
            child2.LocalizedControlType = "MyType";
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
        public void TestFocusableMismatchPass()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.IsContentElement = true;
            child2.IsContentElement = true;
            child1.LocalizedControlType = "MyType";
            child2.LocalizedControlType = "MyType";
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
        public void TestMatchError()
        {
            var parent = new MockA11yElement();
            var child1 = new MockA11yElement();
            var child2 = new MockA11yElement();
            child1.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child2.BoundingRectangle = new Rectangle(0, 0, 25, 25);
            child1.IsContentElement = true;
            child2.IsContentElement = true;
            child1.LocalizedControlType = "MyType";
            child2.LocalizedControlType = "MyType";
            child1.Name = "Alice";
            child2.Name = "Alice";
            child1.IsKeyboardFocusable = true;
            child2.IsKeyboardFocusable = true;
            child1.Parent = parent;
            child2.Parent = parent;
            parent.Children.Add(child1);
            parent.Children.Add(child2);

            Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(child2));
        }
    } // class
} // namespace
