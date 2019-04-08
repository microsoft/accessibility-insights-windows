// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class BoundingRectangleSizeReasonableTest
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.BoundingRectangleSizeReasonable();

        [TestMethod]
        public void TestBoundingRectangleSizeReasonablePass()
        {
            Rectangle rect = new Rectangle(0, 0, 13, 2);

            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = rect;
                Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleSizeReasonableEmptyFail()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty;
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleSizeReasonableNotMinAreaFail()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 12, 2);
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleSizeReasonableArgumentException()
        {
            Action action = () => Rule.Evaluate(null);
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void BoundingRectangleSizeReasonable_EmptyTextNonApplicable()
        {
            var e = new MockA11yElement();

            // valid rectangle, no name, IsKeyboardFocusable false, no children

            e.BoundingRectangle = new Rectangle(0, 0, 2, 2);
            e.ControlTypeId = ControlType.Text;

            Assert.IsFalse(Rule.Condition.Matches(e));
        }
    }// class
} // namespace
