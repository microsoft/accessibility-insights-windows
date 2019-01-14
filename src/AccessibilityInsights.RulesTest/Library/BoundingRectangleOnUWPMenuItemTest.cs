// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class BoundingRectangleOnUWPMenuItemTest
    {
            private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.BoundingRectangleOnUWPMenuItem();
[TestMethod]
        public void TestBoundingRectangleOnUWPMenuItemPass()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 2, 2);

                Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleOnUWPMenuItemNotPass()
        {
            using (var e = new MockA11yElement())
            {
                Assert.AreNotEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }
    } // class
} // namespace
