// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class BoundingRectangleNotNullTest
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.BoundingRectangleNotNull();

        [TestMethod]
        public void TestBoundingRectangleNotNullPass()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = Rectangle.Empty;
                Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleNotNullFail()
        {
            using (var e = new MockA11yElement())
            {
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }
    } // class
} // namespace
