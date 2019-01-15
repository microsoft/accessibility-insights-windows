// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AccessibilityInsights.Rules;

namespace AccessibilityInsights.RulesTest.Conditions
{
    [TestClass]
    public class ConditionTest
    {
        [TestMethod]
        public void TestTrue()
        {
            Assert.IsTrue(Condition.True.Matches(null));
        }

        [TestMethod]
        public void TestFalse()
        {
            Assert.IsFalse(Condition.False.Matches(null));
        }

        [TestMethod]
        public void TestAndOperator()
        {
            var test = Condition.True & Condition.True;
            Assert.IsInstanceOfType(test, typeof(AndCondition));
        }

        [TestMethod]
        public void TestOrOperator()
        {
            var test = Condition.True | Condition.True;
            Assert.IsInstanceOfType(test, typeof(OrCondition));
        }

        [TestMethod]
        public void TestNotOperator()
        {
            var test = ~Condition.True;
            Assert.IsInstanceOfType(test, typeof(NotCondition));
        }
    } // class
} // namespace
