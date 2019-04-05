// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Core.Bases;
using Axe.Windows.Core.Types;
using EvaluationCode = Axe.Windows.Rules.EvaluationCode;

namespace Axe.Windows.RulesTest.Library
{
    [TestClass]
    public class BoundingRectangleDataFormatCorrectTest
    {
        private static Axe.Windows.Rules.IRule Rule = new Axe.Windows.Rules.Library.BoundingRectangleDataFormatCorrect();

        [TestMethod]
        public void TestBoundingRectangleDataFormatCorrectPass()
        {
            using (var e = new MockA11yElement())
            {
                var p = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new double[] {  1, 2, 3, 4 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, p);
                Assert.AreEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleDataFormatCorrectArrayLengthFaile()
        {
            using (var e = new MockA11yElement())
            {
                var p = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new double[] { 1, 2, 3 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, p);
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleDataFormatCorrectArrayTypeFaile()
        {
            using (var e = new MockA11yElement())
            {
                var p = new A11yProperty(PropertyType.UIA_BoundingRectanglePropertyId, new int[] { 1, 2, 3, 4 });
                e.Properties.Add(PropertyType.UIA_BoundingRectanglePropertyId, p);
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }

        [TestMethod]
        public void TestBoundingRectangleDataFormatCorrectNullPropertyFail()
        {
            using (var e = new MockA11yElement())
            {
                Assert.AreNotEqual(Rule.Evaluate(e), EvaluationCode.Pass);
            } // using
        }
    } // class
} // namespace
