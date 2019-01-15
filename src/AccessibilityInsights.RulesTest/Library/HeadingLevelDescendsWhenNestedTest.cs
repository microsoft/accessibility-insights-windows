// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Core.Types;
using EvaluationCode = AccessibilityInsights.Rules.EvaluationCode;

namespace AccessibilityInsights.RulesTest.Library
{
    [TestClass]
    public class HeadingLevelDescendsWhenNestedTest
    {
        private static AccessibilityInsights.Rules.IRule Rule = new AccessibilityInsights.Rules.Library.HeadingLevelDescendsWhenNested();

        [TestMethod]
        public void TestHeadingLevelLowerBoundTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel1;

                Assert.IsTrue(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestHeadingLevelUpperBoundTrue()
        {
            using (var e = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel9;

                Assert.IsTrue(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestHeadingLevelOutsideLowerBoundFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel1 - 1;

                Assert.IsFalse(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestHeadingLevelOutsideUpperBoundFalse()
        {
            using (var e = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel9 + 1;

                Assert.IsFalse(Rule.Condition.Matches(e));
            } // using
        }

        [TestMethod]
        public void TestHeadingLevelDescendsWhenNestedPass()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel2;
                parent.HeadingLevel = HeadingLevelType.HeadingLevel1;
                e.Parent = parent;

                Assert.AreEqual(EvaluationCode.Pass, Rule.Evaluate(e));
            } // using
        }

        [TestMethod]
        public void TestHeadingLevelDescendsWhenNestedError()
        {
            using (var e = new MockA11yElement())
            using (var parent = new MockA11yElement())
            {
                e.HeadingLevel = HeadingLevelType.HeadingLevel8;
                parent.HeadingLevel = HeadingLevelType.HeadingLevel9;
                e.Parent = parent;

                Assert.AreEqual(EvaluationCode.Error, Rule.Evaluate(e));
            } // using
        }
    } // class
} // namespace
