// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Rules;

namespace AccessibilityInsights.RulesTest.Conditions
{
    [TestClass]
    public class AndConditionTest
    {
        [TestMethod]
        public void TestTrueTrue()
        {
            var test = new AndCondition(Condition.True, Condition.True);
            Assert.IsTrue(test.Matches(null));
        }

        [TestMethod]
        public void TestTrueFalse()
        {
            var test = new AndCondition(Condition.True, Condition.False);
            Assert.IsFalse(test.Matches(null));
        }

        [TestMethod]
        public void TestFalseTrue()
        {
            var test = new AndCondition(Condition.False, Condition.True);
            Assert.IsFalse(test.Matches(null));
        }

        [TestMethod]
        public void TestFalseFalse()
        {
            var test = new AndCondition(Condition.False, Condition.False);
            Assert.IsFalse(test.Matches(null));
        }
    } // class
} // namespace
