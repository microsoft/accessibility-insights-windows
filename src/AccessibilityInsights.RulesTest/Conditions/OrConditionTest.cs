// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Axe.Windows.Rules;

namespace Axe.Windows.RulesTest.Conditions
{
    [TestClass]
    public class OrConditionTest
    {
        [TestMethod]
        public void TestTrueTrue()
        {
            var test = new OrCondition(Condition.True, Condition.True);
            Assert.IsTrue(test.Matches(null));
        }

        [TestMethod]
        public void TestTrueFalse()
        {
            var test = new OrCondition(Condition.True, Condition.False);
            Assert.IsTrue(test.Matches(null));
        }

        [TestMethod]
        public void TestFalseTrue()
        {
            var test = new OrCondition(Condition.False, Condition.True);
            Assert.IsTrue(test.Matches(null));
        }

        [TestMethod]
        public void TestFalseFalse()
        {
            var test = new OrCondition(Condition.False, Condition.False);
            Assert.IsFalse(test.Matches(null));
        }
    } // class
} // namespace
