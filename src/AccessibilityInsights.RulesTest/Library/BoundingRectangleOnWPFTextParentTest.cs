// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class BoundingRectangleOnWPFTextParentTest
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.BoundingRectangleOnWPFTextParent();

        [TestMethod]
        public void TestBoundingRectangleOnWPFTextParentPass()
        {
            using (var e = new MockA11yElement())
            {
                e.BoundingRectangle = new Rectangle(0, 0, 2, 2);

                Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleOnWPFTextParentNotPass()
        {
            using (var e = new MockA11yElement())
            {
                Assert.AreNotEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }
    } // class
} // namespace
